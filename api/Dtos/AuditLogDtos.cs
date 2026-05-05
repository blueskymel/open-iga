using OpenIga.Api.Models;

namespace OpenIga.Api.Dtos;

public record AuditLogDto(Guid Id, AuditAction Action, Guid? PerformedBy, Guid? TargetUser, DateTime Timestamp);
