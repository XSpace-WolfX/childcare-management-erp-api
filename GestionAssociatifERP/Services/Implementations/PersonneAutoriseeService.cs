using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class PersonneAutoriseeService : IPersonneAutoriseeService
    {
        private readonly IPersonneAutoriseeRepository _personneAutoriseeRepository;
        private readonly IMapper _mapper;

        public PersonneAutoriseeService(IPersonneAutoriseeRepository personneAutoriseeRepository, IMapper mapper)
        {
            _personneAutoriseeRepository = personneAutoriseeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PersonneAutoriseeDto>> GetAllPersonnesAutoriseesAsync()
        {
            var personnesAutorisees = await _personneAutoriseeRepository.GetAllAsync();
            var personnesAutoriseesDto = _mapper.Map<IEnumerable<PersonneAutoriseeDto>>(personnesAutorisees);

            if (personnesAutoriseesDto == null)
                personnesAutoriseesDto = new List<PersonneAutoriseeDto>();

            return personnesAutoriseesDto;
        }

        public async Task<PersonneAutoriseeDto> GetPersonneAutoriseeAsync(int id)
        {
            var personneAutorisee = await _personneAutoriseeRepository.GetByIdAsync(id);
            if (personneAutorisee == null)
                throw new NotFoundException("Aucune personne autorisée correspondante n'a été trouvée.");

            return _mapper.Map<PersonneAutoriseeDto>(personneAutorisee);
        }

        public async Task<PersonneAutoriseeWithEnfantsDto> GetPersonneAutoriseeWithEnfantsAsync(int id)
        {
            var personneAutorisee = await _personneAutoriseeRepository.GetWithEnfantsAsync(id);
            if (personneAutorisee == null)
                throw new NotFoundException("Aucune personne autorisée correspondante n'a été trouvée.");

            return _mapper.Map<PersonneAutoriseeWithEnfantsDto>(personneAutorisee);
        }

        public async Task<PersonneAutoriseeDto> CreatePersonneAutoriseeAsync(CreatePersonneAutoriseeDto personneAutoriseeDto)
        {
            var personneAutorisee = _mapper.Map<PersonneAutorisee>(personneAutoriseeDto);
            if (personneAutorisee == null)
                throw new Exception("Erreur lors de la création de la personne autorisée : Le Mapping a échoué.");

            await _personneAutoriseeRepository.AddAsync(personneAutorisee);

            var createdPersonneAutorisee = await _personneAutoriseeRepository.GetByIdAsync(personneAutorisee.Id);
            if (createdPersonneAutorisee == null)
                throw new Exception("Échec de la création de la personne autorisée.");

            return _mapper.Map<PersonneAutoriseeDto>(personneAutorisee);
        }

        public async Task UpdatePersonneAutoriseeAsync(int id, UpdatePersonneAutoriseeDto personneAutoriseeDto)
        {
            if (id != personneAutoriseeDto.Id)
                throw new BadRequestException("L'identifiant de la personne autorisée ne correspond pas à celui de l'objet envoyé.");

            var personneAutorisee = await _personneAutoriseeRepository.GetByIdAsync(id);
            if (personneAutorisee is null)
                throw new NotFoundException("Aucune personne autorisée correspondante n'a été trouvée.");

            _mapper.Map(personneAutoriseeDto, personneAutorisee);

            await _personneAutoriseeRepository.UpdateAsync(personneAutorisee);
        }

        public async Task DeletePersonneAutoriseeAsync(int id)
        {
            var personneAutorisee = await _personneAutoriseeRepository.GetByIdAsync(id);
            if (personneAutorisee is null)
                throw new NotFoundException("Aucune personne autorisée correspondante n'a été trouvée.");

            await _personneAutoriseeRepository.DeleteAsync(id);
        }
    }
}