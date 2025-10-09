using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Infrastructure.Persistence.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class LinkAuthorizedPersonChildService : ILinkAuthorizedPersonChildService
    {
        private readonly IAuthorizedPersonChildRepository _authorizedPersonChildRepository;
        private readonly IChildRepository _childRepository;
        private readonly IAuthorizedPersonRepository _authorizedPersonRepository;
        private readonly IMapper _mapper;

        public LinkAuthorizedPersonChildService(IAuthorizedPersonChildRepository authorizedPersonChildRepository, IChildRepository childRepository, 
            IAuthorizedPersonRepository authorizedPersonRepository, IMapper mapper)
        {
            _authorizedPersonChildRepository = authorizedPersonChildRepository;
            _childRepository = childRepository;
            _authorizedPersonRepository = authorizedPersonRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LinkAuthorizedPersonChildDto>> GetAuthorizedPeopleByChildIdAsync(int childId)
        {
            var exists = await _childRepository.ExistsAsync(c => c.Id == childId);
            if (!exists)
                throw new NotFoundException("L'enfant spécifié n'existe pas.");

            var linkAuthorizedPersonChild = await _authorizedPersonChildRepository.GetAuthorizedPeopleByChildIdAsync(childId);

            return _mapper.Map<IEnumerable<LinkAuthorizedPersonChildDto>>(linkAuthorizedPersonChild);
        }

        public async Task<IEnumerable<LinkAuthorizedPersonChildDto>> GetChildrenByAuthorizedPersonIdAsync(int authorizedPersonId)
        {
            var exists = await _authorizedPersonRepository.ExistsAsync(ap => ap.Id == authorizedPersonId);
            if (!exists)
                throw new NotFoundException("La personne autorisée spécifiée n'existe pas.");

            var linkAuthorizedPersonChild = await _authorizedPersonChildRepository.GetChildrenByAuthorizedPersonIdAsync(authorizedPersonId);

            return _mapper.Map<IEnumerable<LinkAuthorizedPersonChildDto>>(linkAuthorizedPersonChild);
        }

        public async Task<bool> ExistsLinkAuthorizedPersonChildAsync(int authorizedPersonId, int childId)
        {
            return await _authorizedPersonChildRepository.LinkExistsAsync(authorizedPersonId, childId);
        }

        public async Task<LinkAuthorizedPersonChildDto> CreateLinkAuthorizedPersonChildAsync(CreateLinkAuthorizedPersonChildDto authorizedPersonChildDto)
        {
            if (!await _authorizedPersonRepository.ExistsAsync(ap => ap.Id == authorizedPersonChildDto.AuthorizedPersonId))
                throw new NotFoundException("La personne autorisée spécifiée n'existe pas.");
            else if (!await _childRepository.ExistsAsync(c => c.Id == authorizedPersonChildDto.ChildId))
                throw new NotFoundException("L'enfant spécifié n'existe pas.");
            else if (await _authorizedPersonChildRepository.LinkExistsAsync(authorizedPersonChildDto.AuthorizedPersonId, authorizedPersonChildDto.ChildId))
                throw new ConflictException("Ce lien existe déjà entre cette personne autorisée et cet enfant.");

            var authorizedPersonChild = _mapper.Map<AuthorizedPersonChild>(authorizedPersonChildDto);
            if (authorizedPersonChild == null)
                throw new Exception("Erreur lors de la création du lien Personne Autorisée / Enfant : Le Mapping a échoué.");

            await _authorizedPersonChildRepository.AddAsync(authorizedPersonChild);

            var createdAuthorizedPersonChild = await _authorizedPersonChildRepository.GetLinkAsync(authorizedPersonChild.AuthorizedPersonId, authorizedPersonChild.ChildId);
            if (createdAuthorizedPersonChild == null)
                throw new Exception("Échec de la création du lien Personne Autorisée / Enfant.");

            return _mapper.Map<LinkAuthorizedPersonChildDto>(createdAuthorizedPersonChild);
        }

        public async Task UpdateLinkAuthorizedPersonChildAsync(UpdateLinkAuthorizedPersonChildDto authorizedPersonChildDto)
        {
            var authorizedPersonChild = await _authorizedPersonChildRepository.GetLinkAsync(authorizedPersonChildDto.AuthorizedPersonId, authorizedPersonChildDto.ChildId);
            if (authorizedPersonChild == null)
                throw new NotFoundException("Le lien Personne Autorisée / Enfant n'existe pas.");

            _mapper.Map(authorizedPersonChildDto, authorizedPersonChild);

            await _authorizedPersonChildRepository.UpdateAsync(authorizedPersonChild);
        }

        public async Task RemoveLinkAuthorizedPersonChildAsync(int authorizedPersonId, int childId)
        {
            if (!await _authorizedPersonChildRepository.LinkExistsAsync(authorizedPersonId, childId))
                throw new NotFoundException("Le lien Personne Autorisée / Enfant n'existe pas.");

            await _authorizedPersonChildRepository.RemoveLinkAsync(authorizedPersonId, childId);
        }
    }
}