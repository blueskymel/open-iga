using OpenIga.Api.Models;

namespace OpenIga.Api.Dtos;

public record AccessRequestDto(
    Guid Id,
    Guid UserId,
    Guid RoleId,
    AccessRequestStatus Status,
    DateTime RequestedAt,
    Guid? ApprovedBy,
    DateTime? ApprovedAt);

public record CreateAccessRequestRequest(Guid UserId, Guid RoleId, Guid? PerformedBy);

public record ReviewAccessRequestRequest(Guid ReviewedBy);
