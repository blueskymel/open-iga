namespace OpenIga.Api.Dtos;

public record PermissionDto(Guid Id, string Name);

public record CreatePermissionRequest(string Name);

public record EffectivePermissionDto(Guid Id, string Name);
