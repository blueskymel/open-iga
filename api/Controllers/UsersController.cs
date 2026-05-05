using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Models;
using OpenIga.Api.Services;

namespace OpenIga.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController(OpenIgaDbContext dbContext, IPermissionResolutionService permissionResolutionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await dbContext.Users.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id);

        return user is null ? NotFound() : user;
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        user.Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id;
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, User updatedUser)
    {
        var user = await dbContext.Users.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        user.Email = updatedUser.Email;
        user.Name = updatedUser.Name;
        user.Status = updatedUser.Status;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await dbContext.Users.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<IActionResult> AssignRole(Guid id, AssignRoleRequest request)
    {
        var userExists = await dbContext.Users.AnyAsync(user => user.Id == id);
        var roleExists = await dbContext.Roles.AnyAsync(role => role.Id == request.RoleId);
        if (!userExists || !roleExists)
        {
            return NotFound();
        }

        var alreadyAssigned = await dbContext.UserRoles.AnyAsync(userRole =>
            userRole.UserId == id && userRole.RoleId == request.RoleId);
        if (alreadyAssigned)
        {
            return NoContent();
        }

        dbContext.UserRoles.Add(new UserRole { UserId = id, RoleId = request.RoleId });
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id:guid}/permissions")]
    public async Task<ActionResult<IReadOnlyCollection<EffectivePermissionDto>>> GetEffectivePermissions(Guid id)
    {
        var userExists = await dbContext.Users.AnyAsync(user => user.Id == id);
        if (!userExists)
        {
            return NotFound();
        }

        var permissions = await permissionResolutionService.GetEffectivePermissionsAsync(id);
        return Ok(permissions);
    }
}

public record AssignRoleRequest(Guid RoleId);
