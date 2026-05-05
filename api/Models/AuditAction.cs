namespace OpenIga.Api.Models;

public enum AuditAction
{
    UserCreated,
    RoleAssignedToUser,
    RolePermissionAssigned,
    AccessRequestCreated,
    AccessRequestApproved,
    AccessRequestRejected
}
