using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

namespace ChildcareManagementERP.Api.Repositories
{
    public interface IAuthorizedPersonRepository : IGenericRepository<AuthorizedPerson>
    {
        Task<AuthorizedPerson?> GetWithChildrenAsync(int id);
    }
}