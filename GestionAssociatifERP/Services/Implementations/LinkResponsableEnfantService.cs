using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class LinkResponsableEnfantService : ILinkResponsableEnfantService
    {
        private readonly IResponsableEnfantRepository _responsableEnfantRepository;
        private readonly IEnfantRepository _enfantRepository;
        private readonly IResponsableRepository _responsableRepository;
        private readonly IMapper _mapper;

        public LinkResponsableEnfantService(IResponsableEnfantRepository responsableEnfantRepository, IEnfantRepository enfantRepository, IResponsableRepository responsableRepository, IMapper mapper)
        {
            _responsableEnfantRepository = responsableEnfantRepository;
            _enfantRepository = enfantRepository;
            _responsableRepository = responsableRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LinkResponsableEnfantDto>> GetResponsablesByEnfantIdAsync(int enfantId)
        {
            var exists = await _enfantRepository.ExistsAsync(e => e.Id == enfantId);
            if (!exists)
                throw new NotFoundException("L'enfant spécifié n'existe pas.");

            var linkResponsableEnfant = await _responsableEnfantRepository.GetResponsablesByEnfantIdAsync(enfantId);

            return _mapper.Map<IEnumerable<LinkResponsableEnfantDto>>(linkResponsableEnfant);
        }

        public async Task<IEnumerable<LinkResponsableEnfantDto>> GetEnfantsByResponsableIdAsync(int responsableId)
        {
            var exists = await _responsableRepository.ExistsAsync(r => r.Id == responsableId);
            if (!exists)
                throw new NotFoundException("Le responsable spécifié n'existe pas.");

            var linkResponsableEnfant = await _responsableEnfantRepository.GetEnfantsByResponsableIdAsync(responsableId);

            return _mapper.Map<IEnumerable<LinkResponsableEnfantDto>>(linkResponsableEnfant);
        }

        public async Task<bool> ExistsLinkResponsableEnfantAsync(int enfantId, int responsableId)
        {
            return await _responsableEnfantRepository.LinkExistsAsync(responsableId, enfantId);
        }

        public async Task<LinkResponsableEnfantDto> CreateLinkResponsableEnfantAsync(CreateLinkResponsableEnfantDto responsableEnfantDto)
        {
            if (!await _enfantRepository.ExistsAsync(e => e.Id == responsableEnfantDto.EnfantId))
                throw new NotFoundException("L'enfant spécifié n'existe pas.");
            else if (!await _responsableRepository.ExistsAsync(r => r.Id == responsableEnfantDto.ResponsableId))
                throw new NotFoundException("Le responsable spécifié n'existe pas.");
            else if (await _responsableEnfantRepository.LinkExistsAsync(responsableEnfantDto.ResponsableId, responsableEnfantDto.EnfantId))
                throw new ConflictException("Ce lien existe déjà entre ce responsable et cet enfant.");

            var responsableEnfant = _mapper.Map<ResponsableEnfant>(responsableEnfantDto);
            if (responsableEnfant == null)
                throw new Exception("Erreur lors de la création du lien Responsable / Enfant : Le Mapping a échoué.");

            await _responsableEnfantRepository.AddAsync(responsableEnfant);

            var createdresponsableEnfant = await _responsableEnfantRepository.GetLinkAsync(responsableEnfantDto.ResponsableId, responsableEnfantDto.EnfantId);
            if (createdresponsableEnfant == null)
                throw new Exception("Échec de la création du lien Responsable / Enfant.");

            return _mapper.Map<LinkResponsableEnfantDto>(createdresponsableEnfant);
        }

        public async Task UpdateLinkResponsableEnfantAsync(UpdateLinkResponsableEnfantDto responsableEnfantDto)
        {
            var responsableEnfant = await _responsableEnfantRepository.GetLinkAsync(responsableEnfantDto.ResponsableId, responsableEnfantDto.EnfantId);
            if (responsableEnfant == null)
                throw new NotFoundException("Aucun lien Responsable / Enfant trouvé à mettre à jour.");

            _mapper.Map(responsableEnfantDto, responsableEnfant);

            await _responsableEnfantRepository.UpdateAsync(responsableEnfant);
        }

        public async Task RemoveLinkResponsableEnfantAsync(int enfantId, int responsableId)
        {
            if (!await _responsableEnfantRepository.LinkExistsAsync(responsableId, enfantId))
                throw new NotFoundException("Aucun lien Responsable / Enfant trouvé à supprimer.");

            await _responsableEnfantRepository.RemoveLinkAsync(responsableId, enfantId);
        }
    }   
}