using OpenIga.Api.Dtos;
using OpenIga.Api.Models;

namespace OpenIga.Api.Services;

public static class EntityMappers
{
    public static UserDto ToDto(this User user) =>
        new(user.Id, user.Email, user.Name, user.Status, user.CreatedAt);

    public static RoleDto ToDto(this Role role) =>
        new(role.Id, role.Name, role.Description);

    public static PermissionDto ToDto(this Permission permission) =>
        new(permission.Id, permission.Name);

    public static AccessRequestDto ToDto(this AccessRequest accessRequest) =>
        new(
            accessRequest.Id,
            accessRequest.UserId,
            accessRequest.RoleId,
            accessRequest.Status,
            accessRequest.RequestedAt,
            accessRequest.ApprovedBy,
            accessRequest.ApprovedAt);

    public static AuditLogDto ToDto(this AuditLog auditLog) =>
        new(auditLog.Id, auditLog.Action, auditLog.PerformedBy, auditLog.TargetUser, auditLog.Timestamp);
}
