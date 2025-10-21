using ChildcareManagementERP.Api.Infrastructure.Persistence;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ChildcareManagementERP.Api.Repositories
{
    public class GuardianChildRepository : GenericRepository<GuardianChild>, IGuardianChildRepository
    {
        public GuardianChildRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<GuardianChild>> GetGuardiansByChildIdAsync(int childId)
            => await _dbContext.GuardianChildren
                .Include(gc => gc.Guardian)
                .Where(gc => gc.ChildId == childId)
                .ToListAsync();

        public async Task<IEnumerable<GuardianChild>> GetChildrenByGuardianIdAsync(int guardianId)
            => await _dbContext.GuardianChildren
                .Include(gc => gc.Child)
                .Where(gc => gc.GuardianId == guardianId)
                .ToListAsync();

        public async Task<GuardianChild?> GetLinkAsync(int guardianId, int childId)
            => await _dbContext.GuardianChildren
                .FirstOrDefaultAsync(gc => gc.GuardianId == guardianId && gc.ChildId == childId);

        public async Task<bool> LinkExistsAsync(int guardianId, int childId)
            => await _dbContext.GuardianChildren
                .AnyAsync(gc => gc.GuardianId == guardianId && gc.ChildId == childId);

        public async Task RemoveLinkAsync(int guardianId, int childId)
        {
            var guardianChild = await GetLinkAsync(guardianId, childId);
            if (guardianChild != null)
            {
                _dbContext.GuardianChildren.Remove(guardianChild);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}