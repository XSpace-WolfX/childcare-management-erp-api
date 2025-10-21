using ChildcareManagementERP.Api.Infrastructure.Persistence;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ChildcareManagementERP.Api.Repositories
{
    public class AuthorizedPersonRepository : GenericRepository<AuthorizedPerson>, IAuthorizedPersonRepository
    {
        public AuthorizedPersonRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<AuthorizedPerson?> GetWithChildrenAsync(int id)
        {
            return await _dbContext.AuthorizedPeople
                .Include(ap => ap.AuthorizedPersonChildren)
                    .ThenInclude(apc => apc.Child)
                .FirstOrDefaultAsync(ap => ap.Id == id);
        }
    }
}