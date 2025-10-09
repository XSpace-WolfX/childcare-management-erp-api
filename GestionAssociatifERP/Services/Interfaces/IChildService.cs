using GestionAssociatifERP.Dtos.V1;

namespace GestionAssociatifERP.Services
{
    public interface IChildService
    {
        Task<IEnumerable<ChildDto>> GetAllChildrenAsync();
        Task<ChildDto> GetChildAsync(int id);
        Task<ChildWithGuardiansDto> GetChildWithGuardiansAsync(int id);
        Task<ChildWithAuthorizedPeopleDto> GetChildWithAuthorizedPeopleAsync(int id);
        Task<ChildWithAdditionalDatasDto> GetChildWithAdditionalDatasAsync(int id);
        Task<ChildDto> CreateChildAsync(CreateChildDto childDto);
        Task UpdateChildAsync(int id, UpdateChildDto childDto);
        Task DeleteChildAsync(int id);
    }
}