using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Infrastructure.Persistence.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class PersonalSituationService : IPersonalSituationService
    {
        private readonly IPersonalSituationRepository _personalSituationRepository;
        private readonly IMapper _mapper;

        public PersonalSituationService(IPersonalSituationRepository personalSituationRepository, IMapper mapper)
        {
            _personalSituationRepository = personalSituationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PersonalSituationDto>> GetAllPersonalSituationsAsync()
        {
            var personalSituations = await _personalSituationRepository.GetAllAsync();
            var personalSituationsDto = _mapper.Map<IEnumerable<PersonalSituationDto>>(personalSituations);

            if (personalSituationsDto == null)
                personalSituationsDto = new List<PersonalSituationDto>();

            return personalSituationsDto;
        }

        public async Task<PersonalSituationDto> GetPersonalSituationAsync(int id)
        {
            var personalSituation = await _personalSituationRepository.GetByIdAsync(id);
            if (personalSituation == null)
                throw new Exception("Aucune situation personnelle correspondante n'a été trouvée.");

            return _mapper.Map<PersonalSituationDto>(personalSituation);
        }

        public async Task<PersonalSituationDto> CreatePersonalSituationAsync(CreatePersonalSituationDto personalSituationDto)
        {
            var personalSituation = _mapper.Map<PersonalSituation>(personalSituationDto);
            if (personalSituation == null)
                throw new Exception("Erreur lors de la création de la situation personnelle : Le Mapping a échoué.");

            await _personalSituationRepository.AddAsync(personalSituation);

            var createdPersonalSituation = await _personalSituationRepository.GetByIdAsync(personalSituation.Id);
            if (createdPersonalSituation == null)
                throw new Exception("Échec de la création de la situation personnelle.");

            return _mapper.Map<PersonalSituationDto>(personalSituation);
        }

        public async Task UpdatePersonalSituationAsync(int id, UpdatePersonalSituationDto personalSituationDto)
        {
            if (id != personalSituationDto.Id)
                throw new Exception("L'identifiant de la situation personnelle ne correspond pas à celui de l'objet envoyé.");

            var personalSituation = await _personalSituationRepository.GetByIdAsync(id);
            if (personalSituation == null)
                throw new Exception("Aucune situation personnelle correspondante n'a été trouvée.");

            _mapper.Map(personalSituationDto, personalSituation);

            await _personalSituationRepository.UpdateAsync(personalSituation);
        }

        public async Task DeletePersonalSituationAsync(int id)
        {
            var personalSituation = await _personalSituationRepository.GetByIdAsync(id);
            if (personalSituation == null)
                throw new Exception("Aucune situation personnelle correspondante n'a été trouvée.");

            await _personalSituationRepository.DeleteAsync(id);
        }
    }
}