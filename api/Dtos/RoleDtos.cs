namespace OpenIga.Api.Dtos;

public record RoleDto(Guid Id, string Name, string? Description);

public record CreateRoleRequest(string Name, string? Description);

public record AssignPermissionRequest(Guid PermissionId, Guid? PerformedBy);
