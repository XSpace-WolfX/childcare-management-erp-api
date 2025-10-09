using GestionAssociatifERP.Infrastructure.Persistence.Models;

namespace GestionAssociatifERP.Repositories
{
    public interface IChildRepository : IGenericRepository<Child>
    {
        Task<Child?> GetWithAdditionalDatasAsync(int id);
        Task<Child?> GetWithAuthorizedPeopleAsync(int id);
        Task<Child?> GetWithGuardiansAsync(int id);
    }
}