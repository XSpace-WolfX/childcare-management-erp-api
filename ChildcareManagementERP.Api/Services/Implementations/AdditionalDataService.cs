using AutoMapper;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Helpers;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using ChildcareManagementERP.Api.Repositories;

namespace ChildcareManagementERP.Api.Services
{
    public class AdditionalDataService : IAdditionalDataService
    {
        private readonly IAdditionalDataRepository _additionalDataRepository;
        private readonly IMapper _mapper;

        public AdditionalDataService(IAdditionalDataRepository additionalDataRepository, IMapper mapper)
        {
            _additionalDataRepository = additionalDataRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AdditionalDataDto>> GetAllAdditionalDatasAsync()
        {
            var additionalDatas = await _additionalDataRepository.GetAllAsync();
            var additionalDatasDto = _mapper.Map<IEnumerable<AdditionalDataDto>>(additionalDatas);

            if (additionalDatasDto == null)
                additionalDatasDto = new List<AdditionalDataDto>();

            return additionalDatasDto;
        }

        public async Task<AdditionalDataDto> GetAdditionalDataAsync(int id)
        {
            var additionalData = await _additionalDataRepository.GetByIdAsync(id);
            if (additionalData == null)
                throw new NotFoundException("Aucune donnée supplémentaire correspondante n'a été trouvée.");

            return _mapper.Map<AdditionalDataDto>(additionalData);
        }

        public async Task<AdditionalDataDto> CreateAdditionalDataAsync(CreateAdditionalDataDto additionalDataDto)
        {
            var additionalData = _mapper.Map<AdditionalDatum>(additionalDataDto);
            if (additionalData == null)
                throw new Exception("Erreur lors de la création de la donnée supplémentaire : Le Mapping a échoué.");

            await _additionalDataRepository.AddAsync(additionalData);

            var createdAdditionalData = await _additionalDataRepository.GetByIdAsync(additionalData.Id);
            if (createdAdditionalData == null)
                throw new Exception("Échec de la création de la donnée supplémentaire.");

            return _mapper.Map<AdditionalDataDto>(createdAdditionalData);
        }

        public async Task UpdateAdditionalDataAsync(int id, UpdateAdditionalDataDto additionalDataDto)
        {
            if (id != additionalDataDto.Id)
                throw new BadRequestException("L'identifiant de la donnée supplémentaire ne correspond pas à celui de l'objet envoyé.");

            var additionalData = await _additionalDataRepository.GetByIdAsync(id);
            if (additionalData == null)
                throw new NotFoundException("Aucune donnée supplémentaire correspondante n'a été trouvée.");

            _mapper.Map(additionalDataDto, additionalData);

            await _additionalDataRepository.UpdateAsync(additionalData);
        }

        public async Task DeleteAdditionalDataAsync(int id)
        {
            var additionalData = await _additionalDataRepository.GetByIdAsync(id);
            if (additionalData == null)
                throw new NotFoundException("Aucune donnée supplémentaire correspondante n'a été trouvée.");

            await _additionalDataRepository.DeleteAsync(id);
        }
    }
}