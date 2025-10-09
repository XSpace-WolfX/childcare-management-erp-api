using GestionAssociatifERP.Dtos.V1;

namespace GestionAssociatifERP.Services
{
    public interface IGuardianService
    {
        Task<IEnumerable<GuardianDto>> GetAllGuardiansAsync();
        Task<GuardianDto> GetGuardianAsync(int id);
        Task<GuardianWithFinancialInformationDto> GetGuardianWithFinancialInformationAsync(int id);
        Task<GuardianWithPersonalSituationDto> GetGuardianWithPersonalSituationAsync(int id);
        Task<GuardianWithChildrenDto> GetGuardianWithChildrenAsync(int id);
        Task<GuardianDto> CreateGuardianAsync(CreateGuardianDto guardianDto);
        Task UpdateGuardianAsync(int id, UpdateGuardianDto guardianDto);
        Task DeleteGuardianAsync(int id);
    }
}