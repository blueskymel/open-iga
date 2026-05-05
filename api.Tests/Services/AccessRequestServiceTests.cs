using Moq;
using OpenIga.Api.Dtos;
using OpenIga.Api.Models;
using OpenIga.Api.Repositories;
using OpenIga.Api.Services;

namespace OpenIga.Api.Tests.Services;

public class AccessRequestServiceTests
{
    [Fact]
    public async Task ApproveAccessRequestAsync_WithPendingRequest_ApprovesAssignsRoleAndAudits()
    {
        // Arrange
        var accessRequestId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var reviewedBy = Guid.NewGuid();
        var accessRequest = new AccessRequest
        {
            Id = accessRequestId,
            UserId = userId,
            RoleId = roleId,
            Status = AccessRequestStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        var accessRequestRepository = new Mock<IAccessRequestRepository>();
        accessRequestRepository
            .Setup(repository => repository.GetByIdAsync(accessRequestId, true))
            .ReturnsAsync(accessRequest);
        accessRequestRepository
            .Setup(repository => repository.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var userRepository = new Mock<IUserRepository>();

        var provisioningService = new Mock<IProvisioningService>();
        provisioningService
            .Setup(service => service.AssignRoleToUserAsync(userId, roleId, reviewedBy))
            .ReturnsAsync(ServiceResult.Success());

        var auditService = new Mock<IAuditService>();
        auditService
            .Setup(service => service.LogAsync(AuditAction.AccessRequestApproved, reviewedBy, userId))
            .Returns(Task.CompletedTask);
        var currentUserService = new Mock<ICurrentUserService>();
        currentUserService
            .Setup(service => service.UserId)
            .Returns(reviewedBy);

        var service = new AccessRequestService(
            accessRequestRepository.Object,
            userRepository.Object,
            provisioningService.Object,
            auditService.Object,
            currentUserService.Object);

        // Act
        var result = await service.ApproveAccessRequestAsync(accessRequestId);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal(AccessRequestStatus.Approved, accessRequest.Status);
        Assert.Equal(AccessRequestStatus.Approved, result.Value.Status);
        Assert.Equal(reviewedBy, accessRequest.ApprovedBy);
        Assert.NotNull(accessRequest.ApprovedAt);

        provisioningService.Verify(
            service => service.AssignRoleToUserAsync(userId, roleId, reviewedBy),
            Times.Once);
        auditService.Verify(
            service => service.LogAsync(AuditAction.AccessRequestApproved, reviewedBy, userId),
            Times.Once);
        accessRequestRepository.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }
}
