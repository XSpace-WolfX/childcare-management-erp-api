using GestionAssociatifERP.Infrastructure.Persistence;
using GestionAssociatifERP.Infrastructure.Persistence.Models;

namespace GestionAssociatifERP.Repositories
{
    public class FinancialInformationRepository : GenericRepository<FinancialInformation>, IFinancialInformationRepository
    {
        public FinancialInformationRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}