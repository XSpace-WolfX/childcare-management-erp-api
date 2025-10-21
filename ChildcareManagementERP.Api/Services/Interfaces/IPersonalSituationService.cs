using ChildcareManagementERP.Api.Dtos.V1;

namespace ChildcareManagementERP.Api.Services
{
    public interface IPersonalSituationService
    {
        Task<IEnumerable<PersonalSituationDto>> GetAllPersonalSituationsAsync();
        Task<PersonalSituationDto> GetPersonalSituationAsync(int id);
        Task<PersonalSituationDto> CreatePersonalSituationAsync(CreatePersonalSituationDto personalSituationDto);
        Task UpdatePersonalSituationAsync(int id, UpdatePersonalSituationDto personalSituationDto);
        Task DeletePersonalSituationAsync(int id);
    }
}