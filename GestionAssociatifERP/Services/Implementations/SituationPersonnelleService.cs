using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class SituationPersonnelleService : ISituationPersonnelleService
    {
        private readonly ISituationPersonnelleRepository _situationPersonnelleRepository;
        private readonly IMapper _mapper;

        public SituationPersonnelleService(ISituationPersonnelleRepository situationPersonnelleRepository, IMapper mapper)
        {
            _situationPersonnelleRepository = situationPersonnelleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SituationPersonnelleDto>> GetAllSituationsPersonnellesAsync()
        {
            var situationsPersonnelles = await _situationPersonnelleRepository.GetAllAsync();
            var situationsPersonnellesDto = _mapper.Map<IEnumerable<SituationPersonnelleDto>>(situationsPersonnelles);

            if (situationsPersonnellesDto == null)
                situationsPersonnellesDto = new List<SituationPersonnelleDto>();

            return situationsPersonnellesDto;
        }

        public async Task<SituationPersonnelleDto> GetSituationPersonnelleAsync(int id)
        {
            var situationPersonnelle = await _situationPersonnelleRepository.GetByIdAsync(id);
            if (situationPersonnelle == null)
                throw new NotFoundException("Aucune situation personnelle correspondante n'a été trouvée.");

            return _mapper.Map<SituationPersonnelleDto>(situationPersonnelle);
        }

        public async Task<SituationPersonnelleDto> CreateSituationPersonnelleAsync(CreateSituationPersonnelleDto situationPersonnelleDto)
        {
            var situationPersonnelle = _mapper.Map<SituationPersonnelle>(situationPersonnelleDto);
            if (situationPersonnelle == null)
                throw new Exception("Erreur lors de la création de la situation personnelle : Le Mapping a échoué.");

            await _situationPersonnelleRepository.AddAsync(situationPersonnelle);

            var situationPersonnelleCreated = await _situationPersonnelleRepository.GetByIdAsync(situationPersonnelle.Id);
            if (situationPersonnelleCreated == null)
                throw new Exception("Échec de la création de la situation personnelle.");

            return _mapper.Map<SituationPersonnelleDto>(situationPersonnelle);
        }

        public async Task UpdateSituationPersonnelleAsync(int id, UpdateSituationPersonnelleDto situationPersonnelleDto)
        {
            if (id != situationPersonnelleDto.Id)
                throw new BadRequestException("L'identifiant de la situation personnelle ne correspond pas à celui de l'objet envoyé.");

            var situationPersonnelle = await _situationPersonnelleRepository.GetByIdAsync(id);
            if (situationPersonnelle == null)
                throw new NotFoundException("Aucune situation personnelle correspondante n'a été trouvée.");

            _mapper.Map(situationPersonnelleDto, situationPersonnelle);

            await _situationPersonnelleRepository.UpdateAsync(situationPersonnelle);
        }

        public async Task DeleteSituationPersonnelleAsync(int id)
        {
            var situationPersonnelle = await _situationPersonnelleRepository.GetByIdAsync(id);
            if (situationPersonnelle == null)
                throw new NotFoundException("Aucune situation personnelle correspondante n'a été trouvée.");

            await _situationPersonnelleRepository.DeleteAsync(id);
        }
    }
}