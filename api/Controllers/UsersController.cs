using Microsoft.AspNetCore.Mvc;
using OpenIga.Api.Dtos;
using OpenIga.Api.Services;

namespace OpenIga.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController(IUserService userService, IPermissionResolutionService permissionResolutionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        return Ok(await userService.GetUsersAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await userService.GetUserAsync(id);

        return user is null ? NotFound() : user;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserRequest request)
    {
        var result = await userService.CreateUserAsync(request);
        if (!result.Succeeded)
        {
            return ToErrorResult(result);
        }

        return CreatedAtAction(nameof(GetUser), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateUserRequest request)
    {
        var result = await userService.UpdateUserAsync(id, request);
        return result.Succeeded ? NoContent() : ToErrorResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await userService.DeleteUserAsync(id);
        return result.Succeeded ? NoContent() : ToErrorResult(result);
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<IActionResult> AssignRole(Guid id, AssignRoleRequest request)
    {
        var result = await userService.AssignRoleAsync(id, request);
        return result.Succeeded ? NoContent() : ToErrorResult(result);
    }

    [HttpGet("{id:guid}/permissions")]
    public async Task<ActionResult<IReadOnlyCollection<EffectivePermissionDto>>> GetEffectivePermissions(Guid id)
    {
        var user = await userService.GetUserAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var permissions = await permissionResolutionService.GetEffectivePermissionsAsync(id);
        return Ok(permissions);
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
