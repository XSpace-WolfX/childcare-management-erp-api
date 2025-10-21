using ChildcareManagementERP.Api.Infrastructure.Persistence;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ChildcareManagementERP.Api.Repositories
{
    public class ChildRepository : GenericRepository<Child>, IChildRepository
    {
        public ChildRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<Child?> GetWithAdditionalDatasAsync(int id)
        {
            return await _dbContext.Children
                .Include(c => c.AdditionalData)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Child?> GetWithAuthorizedPeopleAsync(int id)
        {
            return await _dbContext.Children
                .Include(c => c.AuthorizedPersonChildren)
                    .ThenInclude(apc => apc.AuthorizedPerson)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Child?> GetWithGuardiansAsync(int id)
        {
            return await _dbContext.Children
                .Include(c => c.GuardianChildren)
                    .ThenInclude(gc => gc.Guardian)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}