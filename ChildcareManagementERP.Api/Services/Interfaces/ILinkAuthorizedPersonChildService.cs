using ChildcareManagementERP.Api.Dtos.V1;

namespace ChildcareManagementERP.Api.Services
{
    public interface ILinkAuthorizedPersonChildService
    {
        Task<IEnumerable<LinkAuthorizedPersonChildDto>> GetAuthorizedPeopleByChildIdAsync(int childId);
        Task<IEnumerable<LinkAuthorizedPersonChildDto>> GetChildrenByAuthorizedPersonIdAsync(int authorizedPersonId);
        Task<bool> ExistsLinkAuthorizedPersonChildAsync(int authorizedPersonId, int childId);
        Task<LinkAuthorizedPersonChildDto> CreateLinkAuthorizedPersonChildAsync(CreateLinkAuthorizedPersonChildDto authorizedPersonChildDto);
        Task UpdateLinkAuthorizedPersonChildAsync(UpdateLinkAuthorizedPersonChildDto authorizedPersonChildDto);
        Task RemoveLinkAuthorizedPersonChildAsync(int authorizedPersonId, int childId);
    }
}