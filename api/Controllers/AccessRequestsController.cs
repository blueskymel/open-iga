using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Data;
using OpenIga.Api.Models;

namespace OpenIga.Api.Controllers;

[ApiController]
[Route("access-requests")]
public class AccessRequestsController(OpenIgaDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccessRequest>>> GetAccessRequests()
    {
        return await dbContext.AccessRequests.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccessRequest>> GetAccessRequest(Guid id)
    {
        var accessRequest = await dbContext.AccessRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(request => request.Id == id);

        return accessRequest is null ? NotFound() : accessRequest;
    }

    [HttpPost]
    public async Task<ActionResult<AccessRequest>> CreateAccessRequest(CreateAccessRequestRequest request)
    {
        var userExists = await dbContext.Users.AnyAsync(user => user.Id == request.UserId);
        var roleExists = await dbContext.Roles.AnyAsync(role => role.Id == request.RoleId);
        if (!userExists || !roleExists)
        {
            return NotFound();
        }

        var alreadyAssigned = await dbContext.UserRoles.AnyAsync(userRole =>
            userRole.UserId == request.UserId && userRole.RoleId == request.RoleId);
        if (alreadyAssigned)
        {
            return Conflict("User already has this role.");
        }

        var existingPendingRequest = await dbContext.AccessRequests.AnyAsync(accessRequest =>
            accessRequest.UserId == request.UserId
            && accessRequest.RoleId == request.RoleId
            && accessRequest.Status == AccessRequestStatuses.Pending);
        if (existingPendingRequest)
        {
            return Conflict("A pending access request already exists for this user and role.");
        }

        var accessRequest = new AccessRequest
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            RoleId = request.RoleId,
            Status = AccessRequestStatuses.Pending,
            RequestedAt = DateTime.UtcNow
        };

        dbContext.AccessRequests.Add(accessRequest);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAccessRequest), new { id = accessRequest.Id }, accessRequest);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<AccessRequest>> ApproveAccessRequest(Guid id, ReviewAccessRequestRequest request)
    {
        var accessRequest = await dbContext.AccessRequests.FindAsync(id);
        if (accessRequest is null)
        {
            return NotFound();
        }

        if (accessRequest.Status != AccessRequestStatuses.Pending)
        {
            return Conflict($"Only {AccessRequestStatuses.Pending} access requests can be approved.");
        }

        accessRequest.Status = AccessRequestStatuses.Approved;
        accessRequest.ApprovedBy = request.ReviewedBy;
        accessRequest.ApprovedAt = DateTime.UtcNow;

        var alreadyAssigned = await dbContext.UserRoles.AnyAsync(userRole =>
            userRole.UserId == accessRequest.UserId && userRole.RoleId == accessRequest.RoleId);
        if (!alreadyAssigned)
        {
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = accessRequest.UserId,
                RoleId = accessRequest.RoleId
            });
        }

        await dbContext.SaveChangesAsync();

        return Ok(accessRequest);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<AccessRequest>> RejectAccessRequest(Guid id, ReviewAccessRequestRequest request)
    {
        var accessRequest = await dbContext.AccessRequests.FindAsync(id);
        if (accessRequest is null)
        {
            return NotFound();
        }

        if (accessRequest.Status != AccessRequestStatuses.Pending)
        {
            return Conflict($"Only {AccessRequestStatuses.Pending} access requests can be rejected.");
        }

        accessRequest.Status = AccessRequestStatuses.Rejected;
        accessRequest.ApprovedBy = request.ReviewedBy;
        accessRequest.ApprovedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return Ok(accessRequest);
    }
}

public record CreateAccessRequestRequest(Guid UserId, Guid RoleId);

public record ReviewAccessRequestRequest(Guid ReviewedBy);
