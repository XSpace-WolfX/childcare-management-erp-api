using AutoMapper;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Helpers;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using ChildcareManagementERP.Api.Repositories;

namespace ChildcareManagementERP.Api.Services
{
    public class AuthorizedPersonService : IAuthorizedPersonService
    {
        private readonly IAuthorizedPersonRepository _authorizedPersonRepository;
        private readonly IMapper _mapper;

        public AuthorizedPersonService(IAuthorizedPersonRepository authorizedPersonRepository, IMapper mapper)
        {
            _authorizedPersonRepository = authorizedPersonRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorizedPersonDto>> GetAllAuthorizedPeopleAsync()
        {
            var authorizedPeople = await _authorizedPersonRepository.GetAllAsync();
            var authorizedPeopleDto = _mapper.Map<IEnumerable<AuthorizedPersonDto>>(authorizedPeople);

            if (authorizedPeopleDto == null)
                authorizedPeopleDto = new List<AuthorizedPersonDto>();

            return authorizedPeopleDto;
        }

        public async Task<AuthorizedPersonDto> GetAuthorizedPersonAsync(int id)
        {
            var personneAutorisee = await _authorizedPersonRepository.GetByIdAsync(id);
            if (personneAutorisee == null)
                throw new NotFoundException("Aucune personne autorisée correspondante n'a été trouvée.");

            return _mapper.Map<AuthorizedPersonDto>(personneAutorisee);
        }

        public async Task<AuthorizedPersonWithChildrenDto> GetAuthorizedPersonWithChildrenAsync(int id)
        {
            var authorizedPerson = await _authorizedPersonRepository.GetWithChildrenAsync(id);
            if (authorizedPerson == null)
                throw new NotFoundException("Aucune personne autorisée correspondante n'a été trouvée.");

            return _mapper.Map<AuthorizedPersonWithChildrenDto>(authorizedPerson);
        }

        public async Task<AuthorizedPersonDto> CreateAuthorizedPersonAsync(CreateAuthorizedPersonDto authorizedPersonDto)
        {
            var authorizedPerson = _mapper.Map<AuthorizedPerson>(authorizedPersonDto);
            if (authorizedPerson == null)
                throw new Exception("Erreur lors de la création de la personne autorisée : Le Mapping a échoué.");

            await _authorizedPersonRepository.AddAsync(authorizedPerson);

            var createdAuthorizedPerson = await _authorizedPersonRepository.GetByIdAsync(authorizedPerson.Id);
            if (createdAuthorizedPerson == null)
                throw new Exception("Échec de la création de la personne autorisée.");

            return _mapper.Map<AuthorizedPersonDto>(authorizedPerson);
        }

        public async Task UpdateAuthorizedPersonAsync(int id, UpdateAuthorizedPersonDto authorizedPersonDto)
        {
            if (id != authorizedPersonDto.Id)
                throw new BadRequestException("L'identifiant de la personne autorisée ne correspond pas à celui de l'objet envoyé.");

            var authorizedPerson = await _authorizedPersonRepository.GetByIdAsync(id);
            if (authorizedPerson is null)
                throw new NotFoundException("Aucune personne autorisée correspondante n'a été trouvée.");

            _mapper.Map(authorizedPersonDto, authorizedPerson);

            await _authorizedPersonRepository.UpdateAsync(authorizedPerson);
        }

        public async Task DeleteAuthorizedPersonAsync(int id)
        {
            var authorizedPerson = await _authorizedPersonRepository.GetByIdAsync(id);
            if (authorizedPerson is null)
                throw new NotFoundException("Aucune personne autorisée correspondante n'a été trouvée.");

            await _authorizedPersonRepository.DeleteAsync(id);
        }
    }
}