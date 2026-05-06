using System.Collections.Generic;
using System.Threading.Tasks;
using OpenIga.Api.Models;

namespace OpenIga.Api.Repositories
{
    public interface IAttestationRepository
    {
        Task<IEnumerable<Attestation>> GetAllAsync();
        Task<Attestation?> GetByIdAsync(Guid id);
        Task<Attestation> CreateAsync(Attestation attestation);
        Task UpdateAsync(Attestation attestation);
    }
}
