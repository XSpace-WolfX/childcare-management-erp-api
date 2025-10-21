using ChildcareManagementERP.Api.Dtos.V1;

namespace ChildcareManagementERP.Api.Services
{
    public interface IAdditionalDataService
    {
        Task<IEnumerable<AdditionalDataDto>> GetAllAdditionalDatasAsync();
        Task<AdditionalDataDto> GetAdditionalDataAsync(int id);
        Task<AdditionalDataDto> CreateAdditionalDataAsync(CreateAdditionalDataDto additionalDataDto);
        Task UpdateAdditionalDataAsync(int id, UpdateAdditionalDataDto additionalDataDto);
        Task DeleteAdditionalDataAsync(int id);
    }
}