using OpenIga.Api.Dtos;
using OpenIga.Api.Models;

namespace OpenIga.Api.Services;

public interface IAuditLogService
{
    Task LogAsync(AuditAction action, Guid? performedBy, Guid? targetUser);
    Task<IReadOnlyCollection<AuditLogDto>> GetAuditLogsAsync();
}
