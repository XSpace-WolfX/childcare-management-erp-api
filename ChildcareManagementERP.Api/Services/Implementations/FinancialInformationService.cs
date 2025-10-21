using AutoMapper;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Helpers;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using ChildcareManagementERP.Api.Repositories;

namespace ChildcareManagementERP.Api.Services
{
    public class FinancialInformationService : IFinancialInformationService
    {
        private readonly IFinancialInformationRepository _financialInformationRepository;
        private readonly IMapper _mapper;
        public FinancialInformationService(IFinancialInformationRepository financialInformationRepository, IMapper mapper)
        {
            _financialInformationRepository = financialInformationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FinancialInformationDto>> GetAllFinancialInformationsAsync()
        {
            var financialInformations = await _financialInformationRepository.GetAllAsync();
            var financialInformationsDto = _mapper.Map<IEnumerable<FinancialInformationDto>>(financialInformations);

            if (financialInformationsDto == null)
                financialInformationsDto = new List<FinancialInformationDto>();

            return financialInformationsDto;
        }

        public async Task<FinancialInformationDto> GetFinancialInformationAsync(int id)
        {
            var financialInformation = await _financialInformationRepository.GetByIdAsync(id);
            if (financialInformation == null)
                throw new NotFoundException("Aucune information financière correspondante n'a été trouvée.");

            return _mapper.Map<FinancialInformationDto>(financialInformation);
        }

        public async Task<FinancialInformationDto> CreateFinancialInformationAsync(CreateFinancialInformationDto financialInformationDto)
        {
            var financialInformation = _mapper.Map<FinancialInformation>(financialInformationDto);
            if (financialInformation == null)
                throw new Exception("Erreur lors de la création de l'information financière : Le Mapping a échoué.");

            await _financialInformationRepository.AddAsync(financialInformation);

            var createdFinancialInformation = await _financialInformationRepository.GetByIdAsync(financialInformation.Id);
            if (createdFinancialInformation == null)
                throw new Exception("Échec de la création de l'information financière.");

            return _mapper.Map<FinancialInformationDto>(createdFinancialInformation);
        }

        public async Task UpdateFinancialInformationAsync(int id, UpdateFinancialInformationDto financialInformationDto)
        {
            if (id != financialInformationDto.Id)
                throw new BadRequestException("L'identifiant de l'information financière ne correspond pas à celui de l'objet envoyé.");

            var financialInformation = await _financialInformationRepository.GetByIdAsync(id);
            if (financialInformation == null)
                throw new NotFoundException("Aucune information financière correspondante n'a été trouvée.");

            _mapper.Map(financialInformationDto, financialInformation);

            await _financialInformationRepository.UpdateAsync(financialInformation);
        }

        public async Task DeleteFinancialInformationAsync(int id)
        {
            var financialInformation = await _financialInformationRepository.GetByIdAsync(id);
            if (financialInformation == null)
                throw new NotFoundException("Aucune information financière correspondante n'a été trouvée.");

            await _financialInformationRepository.DeleteAsync(id);
        }
    }
}