using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Infrastructure.Persistence.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class LinkGuardianChildService : ILinkGuardianChildService
    {
        private readonly IGuardianChildRepository _guardianChildRepository;
        private readonly IChildRepository _childRepository;
        private readonly IGuardianRepository _guardianRepository;
        private readonly IMapper _mapper;

        public LinkGuardianChildService(IGuardianChildRepository guardianChildRepository, IChildRepository childRepository, IGuardianRepository guardianRepository, IMapper mapper)
        {
            _guardianChildRepository = guardianChildRepository;
            _childRepository = childRepository;
            _guardianRepository = guardianRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LinkGuardianChildDto>> GetGuardiansByChildIdAsync(int childId)
        {
            var exists = await _childRepository.ExistsAsync(c => c.Id == childId);
            if (!exists)
                throw new Exception("L'enfant spécifié n'existe pas.");

            var linkGuardianChild = await _guardianChildRepository.GetGuardiansByChildIdAsync(childId);

            return _mapper.Map<IEnumerable<LinkGuardianChildDto>>(linkGuardianChild);
        }

        public async Task<IEnumerable<LinkGuardianChildDto>> GetChildrenByGuardianIdAsync(int guardianId)
        {
            var exists = await _guardianRepository.ExistsAsync(g => g.Id == guardianId);
            if (!exists)
                throw new Exception("Le responsable spécifié n'existe pas.");

            var linkGuardianChild = await _guardianChildRepository.GetChildrenByGuardianIdAsync(guardianId);

            return _mapper.Map<IEnumerable<LinkGuardianChildDto>>(linkGuardianChild);
        }

        public async Task<bool> ExistsLinkGuardianChildAsync(int childId, int guardianId)
        {
            return await _guardianChildRepository.LinkExistsAsync(guardianId, childId);
        }

        public async Task<LinkGuardianChildDto> CreateLinkGuardianChildAsync(CreateLinkGuardianChildDto guardianChildDto)
        {
            if (!await _childRepository.ExistsAsync(c => c.Id == guardianChildDto.ChildId))
                throw new Exception("L'enfant spécifié n'existe pas.");
            else if (!await _guardianRepository.ExistsAsync(g => g.Id == guardianChildDto.GuardianId))
                throw new Exception("Le responsable spécifié n'existe pas.");
            else if (await _guardianChildRepository.LinkExistsAsync(guardianChildDto.GuardianId, guardianChildDto.ChildId))
                throw new Exception("Ce lien existe déjà entre ce responsable et cet enfant.");

            var guardianChild = _mapper.Map<GuardianChild>(guardianChildDto);
            if (guardianChild == null)
                throw new Exception("Erreur lors de la création du lien Responsable / Enfant : Le Mapping a échoué.");

            await _guardianChildRepository.AddAsync(guardianChild);

            var createdGuardianChild = await _guardianChildRepository.GetLinkAsync(guardianChildDto.GuardianId, guardianChildDto.ChildId);
            if (createdGuardianChild == null)
                throw new Exception("Échec de la création du lien Responsable / Enfant.");

            return _mapper.Map<LinkGuardianChildDto>(createdGuardianChild);
        }

        public async Task UpdateLinkGuardianChildAsync(UpdateLinkGuardianChildDto guardianChildDto)
        {
            var guardianChild = await _guardianChildRepository.GetLinkAsync(guardianChildDto.GuardianId, guardianChildDto.ChildId);
            if (guardianChild == null)
                throw new Exception("Aucun lien Responsable / Enfant trouvé à mettre à jour.");

            _mapper.Map(guardianChildDto, guardianChild);

            await _guardianChildRepository.UpdateAsync(guardianChild);
        }

        public async Task RemoveLinkGuardianChildAsync(int childId, int guardianId)
        {
            if (!await _guardianChildRepository.LinkExistsAsync(guardianId, childId))
                throw new Exception("Aucun lien Responsable / Enfant trouvé à supprimer.");

            await _guardianChildRepository.RemoveLinkAsync(guardianId, childId);
        }
    }   
}