using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenIga.Api.Models;
using OpenIga.Api.Data;

namespace OpenIga.Api.Repositories
{
    public class AttestationRepository : IAttestationRepository
    {
        private readonly OpenIgaDbContext _db;
        public AttestationRepository(OpenIgaDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Attestation>> GetAllAsync()
        {
            return await _db.Attestations.ToListAsync();
        }

        public async Task<Attestation?> GetByIdAsync(Guid id)
        {
            return await _db.Attestations.FindAsync(id);
        }

        public async Task<Attestation> CreateAsync(Attestation attestation)
        {
            _db.Attestations.Add(attestation);
            await _db.SaveChangesAsync();
            return attestation;
        }

        public async Task UpdateAsync(Attestation attestation)
        {
            _db.Attestations.Update(attestation);
            await _db.SaveChangesAsync();
        }
    }
}
