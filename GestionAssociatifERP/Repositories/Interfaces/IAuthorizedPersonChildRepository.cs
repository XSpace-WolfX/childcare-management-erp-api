using GestionAssociatifERP.Infrastructure.Persistence.Models;

namespace GestionAssociatifERP.Repositories
{
    public interface IAuthorizedPersonChildRepository : IGenericRepository<AuthorizedPersonChild>
    {
        Task<IEnumerable<AuthorizedPersonChild>> GetAuthorizedPeopleByChildIdAsync(int childId);
        Task<IEnumerable<AuthorizedPersonChild>> GetChildrenByAuthorizedPersonIdAsync(int authorizedPersonId);
        Task<AuthorizedPersonChild?> GetLinkAsync(int authorizedPersonId, int childId);
        Task<bool> LinkExistsAsync(int authorizedPersonId, int childId);
        Task RemoveLinkAsync(int authorizedPersonId, int childId);
    }
}