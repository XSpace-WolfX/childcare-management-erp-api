using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

namespace ChildcareManagementERP.Api.Repositories
{
    public interface IChildRepository : IGenericRepository<Child>
    {
        Task<Child?> GetWithAdditionalDatasAsync(int id);
        Task<Child?> GetWithAuthorizedPeopleAsync(int id);
        Task<Child?> GetWithGuardiansAsync(int id);
    }
}