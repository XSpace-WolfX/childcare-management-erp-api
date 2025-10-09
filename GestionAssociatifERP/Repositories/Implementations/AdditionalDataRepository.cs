using GestionAssociatifERP.Infrastructure.Persistence;
using GestionAssociatifERP.Infrastructure.Persistence.Models;

namespace GestionAssociatifERP.Repositories
{
    public class AdditionalDataRepository : GenericRepository<AdditionalData>, IAdditionalDataRepository
    {
        public AdditionalDataRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}