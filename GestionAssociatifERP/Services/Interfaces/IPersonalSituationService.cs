using GestionAssociatifERP.Dtos.V1;

namespace GestionAssociatifERP.Services
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