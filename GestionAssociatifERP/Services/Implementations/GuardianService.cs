using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Infrastructure.Persistence.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class GuardianService : IGuardianService
    {
        private readonly IGuardianRepository _guardianRepository;
        private readonly IMapper _mapper;

        public GuardianService(IGuardianRepository guardianRepository, IMapper mapper)
        {
            _guardianRepository = guardianRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GuardianDto>> GetAllGuardiansAsync()
        {
            var guardians = await _guardianRepository.GetAllAsync();
            var guardiansDto = _mapper.Map<IEnumerable<GuardianDto>>(guardians);

            if (guardiansDto == null)
                guardiansDto = new List<GuardianDto>();

            return guardiansDto;
        }

        public async Task<GuardianDto> GetGuardianAsync(int id)
        {
            var guardian = await _guardianRepository.GetByIdAsync(id);
            if (guardian == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            return _mapper.Map<GuardianDto>(guardian);
        }

        public async Task<GuardianWithFinancialInformationDto> GetGuardianWithFinancialInformationAsync(int id)
        {
            var guardian = await _guardianRepository.GetWithFinancialInformationAsync(id);
            if (guardian == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            return _mapper.Map<GuardianWithFinancialInformationDto>(guardian);
        }

        public async Task<GuardianWithPersonalSituationDto> GetGuardianWithPersonalSituationAsync(int id)
        {
            var guardian = await _guardianRepository.GetWithPersonalSituationAsync(id);
            if (guardian == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            return _mapper.Map<GuardianWithPersonalSituationDto>(guardian);
        }

        public async Task<GuardianWithChildrenDto> GetGuardianWithChildrenAsync(int id)
        {
            var guardian = await _guardianRepository.GetWithChildrenAsync(id);
            if (guardian == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            return _mapper.Map<GuardianWithChildrenDto>(guardian);
        }

        public async Task<GuardianDto> CreateGuardianAsync(CreateGuardianDto guardianDto)
        {
            var guardian = _mapper.Map<Guardian>(guardianDto);
            if (guardian == null)
                throw new Exception("Erreur lors de la création du responsable : Le Mapping a échoué.");

            await _guardianRepository.AddAsync(guardian);

            var createdGuardian = await _guardianRepository.GetByIdAsync(guardian.Id);
            if (createdGuardian == null)
                throw new Exception("Échec de la création du responsable.");

            return _mapper.Map<GuardianDto>(guardian);
        }

        public async Task UpdateGuardianAsync(int id, UpdateGuardianDto guardianDto)
        {
            if (id != guardianDto.Id)
                throw new BadRequestException("L'identifiant du responsable ne correspond pas à celui de l'objet envoyé.");

            var guardian = await _guardianRepository.GetByIdAsync(id);
            if (guardian == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            _mapper.Map(guardianDto, guardian);

            await _guardianRepository.UpdateAsync(guardian);
        }

        public async Task DeleteGuardianAsync(int id)
        {
            var guardian = await _guardianRepository.GetByIdAsync(id);
            if (guardian == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            await _guardianRepository.DeleteAsync(id);
        }
    }
}