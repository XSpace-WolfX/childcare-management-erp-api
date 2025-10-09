using GestionAssociatifERP.Dtos.V1;

namespace GestionAssociatifERP.Services
{
    public interface ILinkGuardianChildService
    {
        Task<IEnumerable<LinkGuardianChildDto>> GetGuardiansByChildIdAsync(int childId);
        Task<IEnumerable<LinkGuardianChildDto>> GetChildrenByGuardianIdAsync(int guardianId);
        Task<bool> ExistsLinkGuardianChildAsync(int guardianId, int childId);
        Task<LinkGuardianChildDto> CreateLinkGuardianChildAsync(CreateLinkGuardianChildDto guardianChildDto);
        Task UpdateLinkGuardianChildAsync(UpdateLinkGuardianChildDto guardianChildDto);
        Task RemoveLinkGuardianChildAsync(int guardianId, int childId);
    }
}