using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Dtos;
using OpenIga.Api.Models;

namespace OpenIga.Api.Services;

public class AccessRequestService(OpenIgaDbContext dbContext, IAuditLogService auditLogService) : IAccessRequestService
{
    public async Task<IReadOnlyCollection<AccessRequestDto>> GetAccessRequestsAsync()
    {
        return await dbContext.AccessRequests
            .AsNoTracking()
            .Select(accessRequest => accessRequest.ToDto())
            .ToListAsync();
    }

    public async Task<AccessRequestDto?> GetAccessRequestAsync(Guid id)
    {
        var accessRequest = await dbContext.AccessRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(request => request.Id == id);

        return accessRequest?.ToDto();
    }

    public async Task<ServiceResult<AccessRequestDto>> CreateAccessRequestAsync(CreateAccessRequestRequest request)
    {
        var userExists = await dbContext.Users.AnyAsync(user => user.Id == request.UserId);
        var roleExists = await dbContext.Roles.AnyAsync(role => role.Id == request.RoleId);
        if (!userExists || !roleExists)
        {
            return ServiceResult<AccessRequestDto>.Failure(ServiceError.NotFound, "User or role was not found.");
        }

        var alreadyAssigned = await dbContext.UserRoles.AnyAsync(userRole =>
            userRole.UserId == request.UserId && userRole.RoleId == request.RoleId);
        if (alreadyAssigned)
        {
            return ServiceResult<AccessRequestDto>.Failure(ServiceError.Conflict, "User already has this role.");
        }

        var existingPendingRequest = await dbContext.AccessRequests.AnyAsync(accessRequest =>
            accessRequest.UserId == request.UserId
            && accessRequest.RoleId == request.RoleId
            && accessRequest.Status == AccessRequestStatus.Pending);
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

        dbContext.AccessRequests.Add(accessRequest);
        await dbContext.SaveChangesAsync();
        await auditLogService.LogAsync(AuditAction.AccessRequestCreated, request.PerformedBy, request.UserId);

        return ServiceResult<AccessRequestDto>.Success(accessRequest.ToDto());
    }

    public async Task<ServiceResult<AccessRequestDto>> ApproveAccessRequestAsync(Guid id, ReviewAccessRequestRequest request)
    {
        var accessRequest = await dbContext.AccessRequests.FindAsync(id);
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

        var alreadyAssigned = await dbContext.UserRoles.AnyAsync(userRole =>
            userRole.UserId == accessRequest.UserId && userRole.RoleId == accessRequest.RoleId);
        if (!alreadyAssigned)
        {
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = accessRequest.UserId,
                RoleId = accessRequest.RoleId
            });
        }

        await dbContext.SaveChangesAsync();
        await auditLogService.LogAsync(AuditAction.AccessRequestApproved, request.ReviewedBy, accessRequest.UserId);
        await auditLogService.LogAsync(AuditAction.RoleAssignedToUser, request.ReviewedBy, accessRequest.UserId);

        return ServiceResult<AccessRequestDto>.Success(accessRequest.ToDto());
    }

    public async Task<ServiceResult<AccessRequestDto>> RejectAccessRequestAsync(Guid id, ReviewAccessRequestRequest request)
    {
        var accessRequest = await dbContext.AccessRequests.FindAsync(id);
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

        await dbContext.SaveChangesAsync();
        await auditLogService.LogAsync(AuditAction.AccessRequestRejected, request.ReviewedBy, accessRequest.UserId);

        return ServiceResult<AccessRequestDto>.Success(accessRequest.ToDto());
    }
}
