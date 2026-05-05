using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Models;

namespace OpenIga.Api.Controllers;

[ApiController]
[Route("roles")]
public class RolesController(OpenIgaDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        return await dbContext.Roles.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Role>> GetRole(Guid id)
    {
        var role = await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(role => role.Id == id);

        return role is null ? NotFound() : role;
    }

    [HttpPost]
    public async Task<ActionResult<Role>> CreateRole(Role role)
    {
        role.Id = role.Id == Guid.Empty ? Guid.NewGuid() : role.Id;
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
    }
}
