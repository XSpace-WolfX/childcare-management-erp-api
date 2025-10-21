using ChildcareManagementERP.Api.Infrastructure.Persistence;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ChildcareManagementERP.Api.Repositories
{
    public class GuardianRepository : GenericRepository<Guardian>, IGuardianRepository
    {
        public GuardianRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<Guardian?> GetWithFinancialInformationAsync(int id)
        {
            return await _dbContext.Guardians
                .Include(g => g.FinancialInformations)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Guardian?> GetWithPersonalSituationAsync(int id)
        {
            return await _dbContext.Guardians
                .Include(g => g.PersonalSituations)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Guardian?> GetWithChildrenAsync(int id)
        {
            return await _dbContext.Guardians
                .Include(g => g.GuardianChildren)
                    .ThenInclude(gc => gc.Child)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}