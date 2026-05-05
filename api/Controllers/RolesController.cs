using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIga.Api.Dtos;
using OpenIga.Api.Services;

namespace OpenIga.Api.Controllers;

[ApiController]
[Authorize]
[Route("roles")]
public class RolesController(IRoleService roleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        return Ok(await roleService.GetRolesAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoleDto>> GetRole(Guid id)
    {
        var role = await roleService.GetRoleAsync(id);

        return role is null ? NotFound() : role;
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleRequest request)
    {
        var result = await roleService.CreateRoleAsync(request);
        if (!result.Succeeded)
        {
            return ToErrorResult(result);
        }

        return CreatedAtAction(nameof(GetRole), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("{id:guid}/permissions")]
    public async Task<IActionResult> AssignPermission(Guid id, AssignPermissionRequest request)
    {
        var result = await roleService.AssignPermissionAsync(id, request);
        return result.Succeeded ? NoContent() : ToErrorResult(result);
    }

    private ActionResult ToErrorResult(ServiceResult result) =>
        result.Error switch
        {
            ServiceError.NotFound => NotFound(result.Message),
            ServiceError.Conflict => Conflict(result.Message),
            ServiceError.Validation => BadRequest(result.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };

    private ActionResult ToErrorResult<T>(ServiceResult<T> result) =>
        result.Error switch
        {
            ServiceError.NotFound => NotFound(result.Message),
            ServiceError.Conflict => Conflict(result.Message),
            ServiceError.Validation => BadRequest(result.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
}
