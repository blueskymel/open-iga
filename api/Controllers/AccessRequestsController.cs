using Microsoft.AspNetCore.Mvc;
using OpenIga.Api.Dtos;
using OpenIga.Api.Services;

namespace OpenIga.Api.Controllers;

[ApiController]
[Route("access-requests")]
public class AccessRequestsController(IAccessRequestService accessRequestService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccessRequestDto>>> GetAccessRequests()
    {
        return Ok(await accessRequestService.GetAccessRequestsAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccessRequestDto>> GetAccessRequest(Guid id)
    {
        var accessRequest = await accessRequestService.GetAccessRequestAsync(id);

        return accessRequest is null ? NotFound() : accessRequest;
    }

    [HttpPost]
    public async Task<ActionResult<AccessRequestDto>> CreateAccessRequest(CreateAccessRequestRequest request)
    {
        var result = await accessRequestService.CreateAccessRequestAsync(request);
        if (!result.Succeeded)
        {
            return ToErrorResult(result);
        }

        return CreatedAtAction(nameof(GetAccessRequest), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<AccessRequestDto>> ApproveAccessRequest(Guid id, ReviewAccessRequestRequest request)
    {
        var result = await accessRequestService.ApproveAccessRequestAsync(id, request);
        return result.Succeeded ? Ok(result.Value) : ToErrorResult(result);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<AccessRequestDto>> RejectAccessRequest(Guid id, ReviewAccessRequestRequest request)
    {
        var result = await accessRequestService.RejectAccessRequestAsync(id, request);
        return result.Succeeded ? Ok(result.Value) : ToErrorResult(result);
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
