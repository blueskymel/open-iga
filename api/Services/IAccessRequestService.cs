using OpenIga.Api.Dtos;

namespace OpenIga.Api.Services;

public interface IAccessRequestService
{
    Task<IReadOnlyCollection<AccessRequestDto>> GetAccessRequestsAsync();
    Task<AccessRequestDto?> GetAccessRequestAsync(Guid id);
    Task<ServiceResult<AccessRequestDto>> CreateAccessRequestAsync(Guid userId, CreateAccessRequestRequest request);
    Task<ServiceResult<AccessRequestDto>> ApproveAccessRequestAsync(Guid id);
    Task<ServiceResult<AccessRequestDto>> RejectAccessRequestAsync(Guid id);
}
