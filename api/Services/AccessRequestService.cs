using OpenIga.Api.Dtos;
using OpenIga.Api.Models;
using OpenIga.Api.Repositories;

namespace OpenIga.Api.Services;

public class AccessRequestService(
    IAccessRequestRepository accessRequestRepository,
    IUserRepository userRepository,
    IProvisioningService provisioningService,
    IAuditService auditService) : IAccessRequestService
{
    public async Task<IReadOnlyCollection<AccessRequestDto>> GetAccessRequestsAsync()
    {
        var accessRequests = await accessRequestRepository.GetAllAsync();
        return accessRequests.Select(accessRequest => accessRequest.ToDto()).ToList();
    }

    public async Task<AccessRequestDto?> GetAccessRequestAsync(Guid id)
    {
        var accessRequest = await accessRequestRepository.GetByIdAsync(id);
        return accessRequest?.ToDto();
    }

    public async Task<ServiceResult<AccessRequestDto>> CreateAccessRequestAsync(CreateAccessRequestRequest request)
    {
        var userExists = await userRepository.ExistsAsync(request.UserId);
        var roleExists = await provisioningService.RoleExistsAsync(request.RoleId);
        if (!userExists || !roleExists)
        {
            return ServiceResult<AccessRequestDto>.Failure(ServiceError.NotFound, "User or role was not found.");
        }

        var alreadyAssigned = await provisioningService.UserHasRoleAsync(request.UserId, request.RoleId);
        if (alreadyAssigned)
        {
            return ServiceResult<AccessRequestDto>.Failure(ServiceError.Conflict, "User already has this role.");
        }

        var existingPendingRequest = await accessRequestRepository.PendingRequestExistsAsync(request.UserId, request.RoleId);
        if (existingPendingRequest)
        {
            return ServiceResult<AccessRequestDto>.Failure(
                ServiceError.Conflict,
                "A pending access request already exists for this user and role.");
        }

        var accessRequest = new AccessRequest
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            RoleId = request.RoleId,
            Status = AccessRequestStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        accessRequestRepository.Add(accessRequest);
        await accessRequestRepository.SaveChangesAsync();
        await auditService.LogAsync(AuditAction.AccessRequestCreated, request.PerformedBy, request.UserId);

        return ServiceResult<AccessRequestDto>.Success(accessRequest.ToDto());
    }

    public async Task<ServiceResult<AccessRequestDto>> ApproveAccessRequestAsync(Guid id, ReviewAccessRequestRequest request)
    {
        var accessRequest = await accessRequestRepository.GetByIdAsync(id, trackChanges: true);
        if (accessRequest is null)
        {
            return ServiceResult<AccessRequestDto>.Failure(ServiceError.NotFound, "Access request was not found.");
        }

        if (accessRequest.Status != AccessRequestStatus.Pending)
        {
            return ServiceResult<AccessRequestDto>.Failure(
                ServiceError.Conflict,
                $"Only {AccessRequestStatus.Pending} access requests can be approved.");
        }

        accessRequest.Status = AccessRequestStatus.Approved;
        accessRequest.ApprovedBy = request.ReviewedBy;
        accessRequest.ApprovedAt = DateTime.UtcNow;

        var provisioningResult = await provisioningService.AssignRoleToUserAsync(
            accessRequest.UserId,
            accessRequest.RoleId,
            request.ReviewedBy);
        if (!provisioningResult.Succeeded)
        {
            return ServiceResult<AccessRequestDto>.Failure(
                provisioningResult.Error!.Value,
                provisioningResult.Message ?? "Role assignment failed.");
        }

        await accessRequestRepository.SaveChangesAsync();
        await auditService.LogAsync(AuditAction.AccessRequestApproved, request.ReviewedBy, accessRequest.UserId);

        return ServiceResult<AccessRequestDto>.Success(accessRequest.ToDto());
    }

    public async Task<ServiceResult<AccessRequestDto>> RejectAccessRequestAsync(Guid id, ReviewAccessRequestRequest request)
    {
        var accessRequest = await accessRequestRepository.GetByIdAsync(id, trackChanges: true);
        if (accessRequest is null)
        {
            return ServiceResult<AccessRequestDto>.Failure(ServiceError.NotFound, "Access request was not found.");
        }

        if (accessRequest.Status != AccessRequestStatus.Pending)
        {
            return ServiceResult<AccessRequestDto>.Failure(
                ServiceError.Conflict,
                $"Only {AccessRequestStatus.Pending} access requests can be rejected.");
        }

        accessRequest.Status = AccessRequestStatus.Rejected;
        accessRequest.ApprovedBy = request.ReviewedBy;
        accessRequest.ApprovedAt = DateTime.UtcNow;

        await accessRequestRepository.SaveChangesAsync();
        await auditService.LogAsync(AuditAction.AccessRequestRejected, request.ReviewedBy, accessRequest.UserId);

        return ServiceResult<AccessRequestDto>.Success(accessRequest.ToDto());
    }
}
