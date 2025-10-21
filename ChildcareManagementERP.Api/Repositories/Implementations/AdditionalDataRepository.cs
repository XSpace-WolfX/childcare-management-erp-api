using ChildcareManagementERP.Api.Infrastructure.Persistence;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

namespace ChildcareManagementERP.Api.Repositories
{
    public class AdditionalDataRepository : GenericRepository<AdditionalData>, IAdditionalDataRepository
    {
        public AdditionalDataRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}