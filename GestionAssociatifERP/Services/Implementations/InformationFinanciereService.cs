using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class InformationFinanciereService : IInformationFinanciereService
    {
        private readonly IInformationFinanciereRepository _informationFinanciereRepository;
        private readonly IMapper _mapper;
        public InformationFinanciereService(IInformationFinanciereRepository informationFinanciereRepository, IMapper mapper)
        {
            _informationFinanciereRepository = informationFinanciereRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InformationFinanciereDto>> GetAllInformationsFinancieresAsync()
        {
            var informationsFinancieres = await _informationFinanciereRepository.GetAllAsync();
            var informationsFinancieresDto = _mapper.Map<IEnumerable<InformationFinanciereDto>>(informationsFinancieres);

            if (informationsFinancieresDto == null)
                informationsFinancieresDto = new List<InformationFinanciereDto>();

            return informationsFinancieresDto;
        }

        public async Task<InformationFinanciereDto> GetInformationFinanciereAsync(int id)
        {
            var informationFinanciere = await _informationFinanciereRepository.GetByIdAsync(id);
            if (informationFinanciere == null)
                throw new NotFoundException("Aucune information financière correspondante n'a été trouvée.");

            return _mapper.Map<InformationFinanciereDto>(informationFinanciere);
        }

        public async Task<InformationFinanciereDto> CreateInformationFinanciereAsync(CreateInformationFinanciereDto informationFinanciereDto)
        {
            var informationFinanciere = _mapper.Map<InformationFinanciere>(informationFinanciereDto);
            if (informationFinanciere == null)
                throw new Exception("Erreur lors de la création de l'information financière : Le Mapping a échoué.");

            await _informationFinanciereRepository.AddAsync(informationFinanciere);

            var createdInformationFinanciere = await _informationFinanciereRepository.GetByIdAsync(informationFinanciere.Id);
            if (createdInformationFinanciere == null)
                throw new Exception("Échec de la création de l'information financière.");

            return _mapper.Map<InformationFinanciereDto>(createdInformationFinanciere);
        }

        public async Task UpdateInformationFinanciereAsync(int id, UpdateInformationFinanciereDto informationFinanciereDto)
        {
            if (id != informationFinanciereDto.Id)
                throw new BadRequestException("L'identifiant de l'information financière ne correspond pas à celui de l'objet envoyé.");

            var informationFinanciere = await _informationFinanciereRepository.GetByIdAsync(id);
            if (informationFinanciere == null)
                throw new NotFoundException("Aucune information financière correspondante n'a été trouvée.");

            _mapper.Map(informationFinanciereDto, informationFinanciere);

            await _informationFinanciereRepository.UpdateAsync(informationFinanciere);
        }

        public async Task DeleteInformationFinanciereAsync(int id)
        {
            var informationFinanciere = await _informationFinanciereRepository.GetByIdAsync(id);
            if (informationFinanciere == null)
                throw new NotFoundException("Aucune information financière correspondante n'a été trouvée.");

            await _informationFinanciereRepository.DeleteAsync(id);
        }
    }
}