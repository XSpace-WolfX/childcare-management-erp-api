using GestionAssociatifERP.Dtos.V1;

namespace GestionAssociatifERP.Services
{
    public interface ILinkGuardianChildService
    {
        Task<IEnumerable<LinkGuardianChildDto>> GetGuardiansByChildIdAsync(int childId);
        Task<IEnumerable<LinkGuardianChildDto>> GetChildrenByGuardianIdAsync(int guardianId);
        Task<bool> ExistsLinkGuardianChildAsync(int childId, int guardianId);
        Task<LinkGuardianChildDto> CreateLinkGuardianChildAsync(CreateLinkGuardianChildDto guardianChildDto);
        Task UpdateLinkGuardianChildAsync(UpdateLinkGuardianChildDto guardianChildDto);
        Task RemoveLinkGuardianChildAsync(int childId, int guardianId);
    }
}