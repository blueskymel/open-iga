using Microsoft.AspNetCore.Mvc;
using OpenIga.Api.Dtos;
using OpenIga.Api.Services;

namespace OpenIga.Api.Controllers;

[ApiController]
[Route("audit-logs")]
public class AuditLogsController(IAuditLogService auditLogService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogs()
    {
        return Ok(await auditLogService.GetAuditLogsAsync());
    }
}
