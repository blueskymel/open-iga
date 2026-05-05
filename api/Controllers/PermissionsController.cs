using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIga.Api.Dtos;
using OpenIga.Api.Services;

namespace OpenIga.Api.Controllers;

[ApiController]
[Authorize]
[Route("permissions")]
public class PermissionsController(IPermissionService permissionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
    {
        return Ok(await permissionService.GetPermissionsAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PermissionDto>> GetPermission(Guid id)
    {
        var permission = await permissionService.GetPermissionAsync(id);

        return permission is null ? NotFound() : permission;
    }

    [HttpPost]
    public async Task<ActionResult<PermissionDto>> CreatePermission(CreatePermissionRequest request)
    {
        var result = await permissionService.CreatePermissionAsync(request);
        if (!result.Succeeded)
        {
            return ToErrorResult(result);
        }

        return CreatedAtAction(nameof(GetPermission), new { id = result.Value!.Id }, result.Value);
    }

    private ActionResult ToErrorResult<T>(ServiceResult<T> result) =>
        result.Error switch
        {
            ServiceError.NotFound => NotFound(result.Message),
            ServiceError.Conflict => Conflict(result.Message),
            ServiceError.Validation => BadRequest(result.Message),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
}
