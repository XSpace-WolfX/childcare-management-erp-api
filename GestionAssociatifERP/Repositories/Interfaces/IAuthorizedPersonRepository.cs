using GestionAssociatifERP.Infrastructure.Persistence.Models;

namespace GestionAssociatifERP.Repositories
{
    public interface IAuthorizedPersonRepository : IGenericRepository<AuthorizedPerson>
    {
        Task<AuthorizedPerson?> GetWithChildrenAsync(int id);
    }
}