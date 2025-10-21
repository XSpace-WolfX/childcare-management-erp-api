using ChildcareManagementERP.Api.Infrastructure.Persistence;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

namespace ChildcareManagementERP.Api.Repositories
{
    public class FinancialInformationRepository : GenericRepository<FinancialInformation>, IFinancialInformationRepository
    {
        public FinancialInformationRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}