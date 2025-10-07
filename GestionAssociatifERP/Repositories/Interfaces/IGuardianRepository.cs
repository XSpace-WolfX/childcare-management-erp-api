using GestionAssociatifERP.Infrastructure.Persistence.Models;

namespace GestionAssociatifERP.Repositories
{
    public interface IGuardianRepository : IGenericRepository<Guardian>
    {
        public Task<Guardian?> GetWithFinancialInformationAsync(int id);
        public Task<Guardian?> GetWithPersonalSituationAsync(int id);
        public Task<Guardian?> GetWithChildrenAsync(int id);
    }
}