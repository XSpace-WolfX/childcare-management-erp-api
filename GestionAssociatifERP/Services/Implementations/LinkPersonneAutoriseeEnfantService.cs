using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class LinkPersonneAutoriseeEnfantService : ILinkPersonneAutoriseeEnfantService
    {
        private readonly IPersonneAutoriseeEnfantRepository _personneAutoriseeEnfantRepository;
        private readonly IEnfantRepository _enfantRepository;
        private readonly IPersonneAutoriseeRepository _personneAutoriseeRepository;
        private readonly IMapper _mapper;

        public LinkPersonneAutoriseeEnfantService(IPersonneAutoriseeEnfantRepository personneAutoriseeEnfantRepository, IEnfantRepository enfantRepository, IPersonneAutoriseeRepository personneAutoriseeRepository, IMapper mapper)
        {
            _personneAutoriseeEnfantRepository = personneAutoriseeEnfantRepository;
            _enfantRepository = enfantRepository;
            _personneAutoriseeRepository = personneAutoriseeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LinkPersonneAutoriseeEnfantDto>> GetPersonnesAutoriseesByEnfantIdAsync(int enfantId)
        {
            var exists = await _enfantRepository.ExistsAsync(e => e.Id == enfantId);
            if (!exists)
                throw new NotFoundException("L'enfant spécifié n'existe pas.");

            var linkPersonneAutoriseeEnfant = await _personneAutoriseeEnfantRepository.GetPersonnesAutoriseesByEnfantIdAsync(enfantId);

            return _mapper.Map<IEnumerable<LinkPersonneAutoriseeEnfantDto>>(linkPersonneAutoriseeEnfant);
        }

        public async Task<IEnumerable<LinkPersonneAutoriseeEnfantDto>> GetEnfantsByPersonneAutoriseeIdAsync(int personneAutoriseeId)
        {
            var exists = await _personneAutoriseeRepository.ExistsAsync(pa => pa.Id == personneAutoriseeId);
            if (!exists)
                throw new NotFoundException("La personne autorisée spécifiée n'existe pas.");

            var linkPersonneAutoriseeEnfant = await _personneAutoriseeEnfantRepository.GetEnfantsByPersonneAutoriseeIdAsync(personneAutoriseeId);

            return _mapper.Map<IEnumerable<LinkPersonneAutoriseeEnfantDto>>(linkPersonneAutoriseeEnfant);
        }

        public async Task<bool> ExistsLinkPersonneAutoriseeEnfantAsync(int enfantId, int personneAutoriseeId)
        {
            return await _personneAutoriseeEnfantRepository.LinkExistsAsync(personneAutoriseeId, enfantId);
        }

        public async Task<LinkPersonneAutoriseeEnfantDto> CreateLinkPersonneAutoriseeEnfantAsync(CreateLinkPersonneAutoriseeEnfantDto personneAutoriseeEnfantDto)
        {
            if (!await _personneAutoriseeRepository.ExistsAsync(pa => pa.Id == personneAutoriseeEnfantDto.PersonneAutoriseeId))
                throw new NotFoundException("La personne autorisée spécifiée n'existe pas.");
            else if (!await _enfantRepository.ExistsAsync(e => e.Id == personneAutoriseeEnfantDto.EnfantId))
                throw new NotFoundException("L'enfant spécifié n'existe pas.");
            else if (await _personneAutoriseeEnfantRepository.LinkExistsAsync(personneAutoriseeEnfantDto.PersonneAutoriseeId, personneAutoriseeEnfantDto.EnfantId))
                throw new ConflictException("Ce lien existe déjà entre cette personne autorisée et cet enfant.");

            var personneAutoriseeEnfant = _mapper.Map<PersonneAutoriseeEnfant>(personneAutoriseeEnfantDto);
            if (personneAutoriseeEnfant == null)
                throw new Exception("Erreur lors de la création du lien Personne Autorisée / Enfant : Le Mapping a échoué.");

            await _personneAutoriseeEnfantRepository.AddAsync(personneAutoriseeEnfant);

            var createdPersonneAutoriseeEnfant = await _personneAutoriseeEnfantRepository.GetLinkAsync(personneAutoriseeEnfant.PersonneAutoriseeId, personneAutoriseeEnfant.EnfantId);
            if (createdPersonneAutoriseeEnfant == null)
                throw new Exception("Échec de la création du lien Personne Autorisée / Enfant.");

            return _mapper.Map<LinkPersonneAutoriseeEnfantDto>(createdPersonneAutoriseeEnfant);
        }

        public async Task UpdateLinkPersonneAutoriseeEnfantAsync(UpdateLinkPersonneAutoriseeEnfantDto personneAutoriseeEnfantDto)
        {
            var personneAutoriseeEnfant = await _personneAutoriseeEnfantRepository.GetLinkAsync(personneAutoriseeEnfantDto.PersonneAutoriseeId, personneAutoriseeEnfantDto.EnfantId);
            if (personneAutoriseeEnfant == null)
                throw new NotFoundException("Le lien Personne Autorisée / Enfant n'existe pas.");

            _mapper.Map(personneAutoriseeEnfantDto, personneAutoriseeEnfant);

            await _personneAutoriseeEnfantRepository.UpdateAsync(personneAutoriseeEnfant);
        }

        public async Task RemoveLinkPersonneAutoriseeEnfantAsync(int enfantId, int personneAutoriseeId)
        {
            if (!await _personneAutoriseeEnfantRepository.LinkExistsAsync(personneAutoriseeId, enfantId))
                throw new NotFoundException("Le lien Personne Autorisée / Enfant n'existe pas.");

            await _personneAutoriseeEnfantRepository.RemoveLinkAsync(personneAutoriseeId, enfantId);
        }
    }
}