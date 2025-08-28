using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class DonneeSupplementaireService : IDonneeSupplementaireService
    {
        private readonly IDonneeSupplementaireRepository _donneeSupplementaireRepository;
        private readonly IMapper _mapper;

        public DonneeSupplementaireService(IDonneeSupplementaireRepository donneeSupplementaireRepository, IMapper mapper)
        {
            _donneeSupplementaireRepository = donneeSupplementaireRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DonneeSupplementaireDto>> GetAllDonneesSupplementairesAsync()
        {
            var donneesSupplementaires = await _donneeSupplementaireRepository.GetAllAsync();
            var donneesSupplementairesDto = _mapper.Map<IEnumerable<DonneeSupplementaireDto>>(donneesSupplementaires);

            if (donneesSupplementairesDto == null)
                donneesSupplementairesDto = new List<DonneeSupplementaireDto>();

            return donneesSupplementairesDto;
        }

        public async Task<DonneeSupplementaireDto> GetDonneeSupplementaireAsync(int id)
        {
            var donneeSupplementaire = await _donneeSupplementaireRepository.GetByIdAsync(id);
            if (donneeSupplementaire == null)
                throw new NotFoundException("Aucune donnée supplémentaire correspondante n'a été trouvée.");

            return _mapper.Map<DonneeSupplementaireDto>(donneeSupplementaire);
        }

        public async Task<DonneeSupplementaireDto> CreateDonneeSupplementaireAsync(CreateDonneeSupplementaireDto donneeSupplementaireDto)
        {
            var donneeSupplementaire = _mapper.Map<DonneeSupplementaire>(donneeSupplementaireDto);
            if (donneeSupplementaire == null)
                throw new Exception("Erreur lors de la création de la donnée supplémentaire : Le Mapping a échoué.");

            await _donneeSupplementaireRepository.AddAsync(donneeSupplementaire);

            var createdDonneeSupplementaire = await _donneeSupplementaireRepository.GetByIdAsync(donneeSupplementaire.Id);
            if (createdDonneeSupplementaire == null)
                throw new Exception("Échec de la création de la donnée supplémentaire.");

            return _mapper.Map<DonneeSupplementaireDto>(createdDonneeSupplementaire);
        }

        public async Task UpdateDonneeSupplementaireAsync(int id, UpdateDonneeSupplementaireDto donneeSupplementaireDto)
        {
            if (id != donneeSupplementaireDto.Id)
                throw new BadRequestException("L'identifiant de la donnée supplémentaire ne correspond pas à celui de l'objet envoyé.");

            var donneeSupplementaire = await _donneeSupplementaireRepository.GetByIdAsync(id);
            if (donneeSupplementaire == null)
                throw new NotFoundException("Aucune donnée supplémentaire correspondante n'a été trouvée.");

            _mapper.Map(donneeSupplementaireDto, donneeSupplementaire);

            await _donneeSupplementaireRepository.UpdateAsync(donneeSupplementaire);
        }

        public async Task DeleteDonneeSupplementaireAsync(int id)
        {
            var donneeSupplementaire = await _donneeSupplementaireRepository.GetByIdAsync(id);
            if (donneeSupplementaire == null)
                throw new NotFoundException("Aucune donnée supplémentaire correspondante n'a été trouvée.");

            await _donneeSupplementaireRepository.DeleteAsync(id);
        }
    }
}