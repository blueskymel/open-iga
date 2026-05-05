using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Dtos;
using OpenIga.Api.Models;

namespace OpenIga.Api.Services;

public class AuditLogService(OpenIgaDbContext dbContext) : IAuditLogService
{
    public async Task LogAsync(AuditAction action, Guid? performedBy, Guid? targetUser)
    {
        dbContext.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            PerformedBy = performedBy,
            TargetUser = targetUser,
            Timestamp = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<AuditLogDto>> GetAuditLogsAsync()
    {
        return await dbContext.AuditLogs
            .AsNoTracking()
            .OrderByDescending(log => log.Timestamp)
            .Select(log => log.ToDto())
            .ToListAsync();
    }
}
