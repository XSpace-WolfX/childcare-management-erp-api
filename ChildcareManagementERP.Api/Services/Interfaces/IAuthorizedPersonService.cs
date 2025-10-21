using ChildcareManagementERP.Api.Dtos.V1;

namespace ChildcareManagementERP.Api.Services
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