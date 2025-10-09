using GestionAssociatifERP.Dtos.V1;

namespace GestionAssociatifERP.Services
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