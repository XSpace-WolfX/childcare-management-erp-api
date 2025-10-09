using GestionAssociatifERP.Dtos.V1;

namespace GestionAssociatifERP.Services
{
    public interface IAuthorizedPersonService
    {
        Task<IEnumerable<AuthorizedPersonDto>> GetAllAuthorizedPeopleAsync();
        Task<AuthorizedPersonDto> GetAuthorizedPersonAsync(int id);
        Task<AuthorizedPersonWithChildrenDto> GetAuthorizedPersonWithChildrenAsync(int id);
        Task<AuthorizedPersonDto> CreateAuthorizedPersonAsync(CreateAuthorizedPersonDto authorizedPersonDto);
        Task UpdateAuthorizedPersonAsync(int id, UpdateAuthorizedPersonDto authorizedPersonDto);
        Task DeleteAuthorizedPersonAsync(int id);
    }
}