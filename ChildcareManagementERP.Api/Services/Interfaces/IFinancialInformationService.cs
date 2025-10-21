using ChildcareManagementERP.Api.Dtos.V1;

namespace ChildcareManagementERP.Api.Services
{
    public interface IFinancialInformationService
    {
        Task<IEnumerable<FinancialInformationDto>> GetAllFinancialInformationsAsync();
        Task<FinancialInformationDto> GetFinancialInformationAsync(int id);
        Task<FinancialInformationDto> CreateFinancialInformationAsync(CreateFinancialInformationDto financialInformationDto);
        Task UpdateFinancialInformationAsync(int id, UpdateFinancialInformationDto financialInformationDto);
        Task DeleteFinancialInformationAsync(int id);
    }
}