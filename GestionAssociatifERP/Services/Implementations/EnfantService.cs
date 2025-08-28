using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class EnfantService : IEnfantService
    {
        private readonly IEnfantRepository _enfantRepository;
        private readonly IMapper _mapper;

        public EnfantService(IEnfantRepository enfantRepository, IMapper mapper)
        {
            _enfantRepository = enfantRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EnfantDto>> GetAllEnfantsAsync()
        {
            var enfants = await _enfantRepository.GetAllAsync();
            var enfantsDto = _mapper.Map<IEnumerable<EnfantDto>>(enfants);

            if (enfantsDto == null)
                enfantsDto = new List<EnfantDto>();

            return enfantsDto;
        }

        public async Task<EnfantDto> GetEnfantAsync(int id)
        {
            var enfant = await _enfantRepository.GetByIdAsync(id);
            if (enfant == null)
                throw new NotFoundException("Aucun enfant correspondant n'a été trouvé.");

            return _mapper.Map<EnfantDto>(enfant);
        }

        public async Task<EnfantWithResponsablesDto> GetEnfantWithResponsablesAsync(int id)
        {
            var enfant = await _enfantRepository.GetWithResponsablesAsync(id);
            if (enfant == null)
                throw new NotFoundException("Aucun enfant correspondant n'a été trouvé.");

            return _mapper.Map<EnfantWithResponsablesDto>(enfant);
        }

        public async Task<EnfantWithPersonnesAutoriseesDto> GetEnfantWithPersonnesAutoriseesAsync(int id)
        {
            var enfant = await _enfantRepository.GetWithPersonnesAutoriseesAsync(id);
            if (enfant == null)
                throw new NotFoundException("Aucun enfant correspondant n'a été trouvé.");

            return _mapper.Map<EnfantWithPersonnesAutoriseesDto>(enfant);
        }

        public async Task<EnfantWithDonneesSupplementairesDto> GetEnfantWithDonneesSupplementairesAsync(int id)
        {
            var enfant = await _enfantRepository.GetWithDonneesSupplementairesAsync(id);
            if (enfant == null)
                throw new NotFoundException("Aucun enfant correspondant n'a été trouvé.");

            return _mapper.Map<EnfantWithDonneesSupplementairesDto>(enfant);
        }

        public async Task<EnfantDto> CreateEnfantAsync(CreateEnfantDto enfantDto)
        {
            var enfant = _mapper.Map<Enfant>(enfantDto);
            if (enfant == null)
                throw new Exception("Erreur lors de la création de l'enfant : Le Mapping a échoué.");

            await _enfantRepository.AddAsync(enfant);

            var createdEnfant = await _enfantRepository.GetByIdAsync(enfant.Id);
            if (createdEnfant == null)
                throw new Exception("Échec de la création de l'enfant.");

            return _mapper.Map<EnfantDto>(createdEnfant);
        }

        public async Task UpdateEnfantAsync(int id, UpdateEnfantDto enfantDto)
        {
            if (id != enfantDto.Id)
                throw new BadRequestException("L'identifiant de l'enfant ne correspond pas à celui de l'objet envoyé.");

            var enfant = await _enfantRepository.GetByIdAsync(id);
            if (enfant == null)
                throw new NotFoundException("Aucun enfant correspondant n'a été trouvé.");

            _mapper.Map(enfantDto, enfant);

            await _enfantRepository.UpdateAsync(enfant);
        }

        public async Task DeleteEnfantAsync(int id)
        {
            var enfant = await _enfantRepository.GetByIdAsync(id);
            if (enfant == null)
                throw new NotFoundException("Aucun enfant correspondant n'a été trouvé.");

            await _enfantRepository.DeleteAsync(id);
        }
    }
}