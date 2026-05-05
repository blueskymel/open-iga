using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Models;

namespace OpenIga.Api.Controllers;

[ApiController]
[Route("permissions")]
public class PermissionsController(OpenIgaDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Permission>>> GetPermissions()
    {
        return await dbContext.Permissions.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Permission>> GetPermission(Guid id)
    {
        var permission = await dbContext.Permissions.AsNoTracking().FirstOrDefaultAsync(permission => permission.Id == id);

        return permission is null ? NotFound() : permission;
    }

    [HttpPost]
    public async Task<ActionResult<Permission>> CreatePermission(Permission permission)
    {
        permission.Id = permission.Id == Guid.Empty ? Guid.NewGuid() : permission.Id;
        dbContext.Permissions.Add(permission);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permission);
    }
}
