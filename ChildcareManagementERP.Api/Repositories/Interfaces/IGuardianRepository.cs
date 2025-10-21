using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

namespace ChildcareManagementERP.Api.Repositories
{
    public interface IGuardianRepository : IGenericRepository<Guardian>
    {
        public Task<Guardian?> GetWithFinancialInformationAsync(int id);
        public Task<Guardian?> GetWithPersonalSituationAsync(int id);
        public Task<Guardian?> GetWithChildrenAsync(int id);
    }
}