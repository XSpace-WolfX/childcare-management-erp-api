using ChildcareManagementERP.Api.Infrastructure.Persistence;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ChildcareManagementERP.Api.Repositories
{
    public class AuthorizedPersonChildRepository : GenericRepository<AuthorizedPersonChild>, IAuthorizedPersonChildRepository
    {
        public AuthorizedPersonChildRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<AuthorizedPersonChild>> GetAuthorizedPeopleByChildIdAsync(int childId)
            => await _dbContext.AuthorizedPersonChildren
                .Include(apc => apc.AuthorizedPerson)
                .Where(apc => apc.ChildId == childId)
                .ToListAsync();

        public async Task<IEnumerable<AuthorizedPersonChild>> GetChildrenByAuthorizedPersonIdAsync(int authorizedPersonId)
            => await _dbContext.AuthorizedPersonChildren
                .Include(apc => apc.Child)
                .Where(apc => apc.AuthorizedPersonId == authorizedPersonId)
                .ToListAsync();

        public async Task<AuthorizedPersonChild?> GetLinkAsync(int authorizedPersonId, int childId)
            => await _dbContext.AuthorizedPersonChildren
                .FirstOrDefaultAsync(apc => apc.AuthorizedPersonId == authorizedPersonId && apc.ChildId == childId);

        public async Task<bool> LinkExistsAsync(int authorizedPersonId, int childId)
            => await _dbContext.AuthorizedPersonChildren
                .AnyAsync(apc => apc.AuthorizedPersonId == authorizedPersonId && apc.ChildId == childId);

        public async Task RemoveLinkAsync(int authorizedPersonId, int childId)
        {
            var authorizedPersonChild = await GetLinkAsync(authorizedPersonId, childId);
            if (authorizedPersonChild != null)
            {
                _dbContext.AuthorizedPersonChildren.Remove(authorizedPersonChild);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}