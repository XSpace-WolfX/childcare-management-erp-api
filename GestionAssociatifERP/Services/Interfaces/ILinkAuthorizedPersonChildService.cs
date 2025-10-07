using GestionAssociatifERP.Dtos.V1;

namespace GestionAssociatifERP.Services
{
    public interface ILinkAuthorizedPersonChildService
    {
        Task<IEnumerable<LinkAuthorizedPersonChildDto>> GetAuthorizedPeopleByChildIdAsync(int childId);
        Task<IEnumerable<LinkAuthorizedPersonChildDto>> GetChildrenByAuthorizedPersonIdAsync(int authorizedPersonId);
        Task<bool> ExistsLinkAuthorizedPersonChildAsync(int childId, int authorizedPersonId);
        Task<LinkAuthorizedPersonChildDto> CreateLinkAuthorizedPersonChildAsync(CreateLinkAuthorizedPersonChildDto authorizedPersonChildDto);
        Task UpdateLinkAuthorizedPersonChildAsync(UpdateLinkAuthorizedPersonChildDto authorizedPersonChildDto);
        Task RemoveLinkAuthorizedPersonChildAsync(int childId, int authorizedPersonId);
    }
}