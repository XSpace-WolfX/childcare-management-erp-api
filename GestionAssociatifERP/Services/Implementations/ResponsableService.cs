using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Helpers;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class ResponsableService : IResponsableService
    {
        private readonly IResponsableRepository _responsableRepository;
        private readonly IMapper _mapper;

        public ResponsableService(IResponsableRepository responsableRepository, IMapper mapper)
        {
            _responsableRepository = responsableRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResponsableDto>> GetAllResponsablesAsync()
        {
            var responsables = await _responsableRepository.GetAllAsync();
            var responsablesDto = _mapper.Map<IEnumerable<ResponsableDto>>(responsables);

            if (responsablesDto == null)
                responsablesDto = new List<ResponsableDto>();

            return responsablesDto;
        }

        public async Task<ResponsableDto> GetResponsableAsync(int id)
        {
            var responsable = await _responsableRepository.GetByIdAsync(id);
            if (responsable == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            return _mapper.Map<ResponsableDto>(responsable);
        }

        public async Task<ResponsableWithInformationFinanciereDto> GetResponsableWithInformationFinanciereAsync(int id)
        {
            var responsable = await _responsableRepository.GetWithInformationFinanciereAsync(id);
            if (responsable == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            return _mapper.Map<ResponsableWithInformationFinanciereDto>(responsable);
        }

        public async Task<ResponsableWithSituationPersonnelleDto> GetResponsableWithSituationPersonnelleAsync(int id)
        {
            var responsable = await _responsableRepository.GetWithSituationPersonnelleAsync(id);
            if (responsable == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            return _mapper.Map<ResponsableWithSituationPersonnelleDto>(responsable);
        }

        public async Task<ResponsableWithEnfantsDto> GetResponsableWithEnfantsAsync(int id)
        {
            var responsable = await _responsableRepository.GetWithEnfantsAsync(id);
            if (responsable == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            return _mapper.Map<ResponsableWithEnfantsDto>(responsable);
        }

        public async Task<ResponsableDto> CreateResponsableAsync(CreateResponsableDto responsableDto)
        {
            var responsable = _mapper.Map<Responsable>(responsableDto);
            if (responsable == null)
                throw new Exception("Erreur lors de la création du responsable : Le Mapping a échoué.");

            await _responsableRepository.AddAsync(responsable);

            var responsableCreated = await _responsableRepository.GetByIdAsync(responsable.Id);
            if (responsableCreated == null)
                throw new Exception("Échec de la création du responsable.");

            return _mapper.Map<ResponsableDto>(responsable);
        }

        public async Task UpdateResponsableAsync(int id, UpdateResponsableDto responsableDto)
        {
            if (id != responsableDto.Id)
                throw new BadRequestException("L'identifiant du responsable ne correspond pas à celui de l'objet envoyé.");

            var responsable = await _responsableRepository.GetByIdAsync(id);
            if (responsable == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            _mapper.Map(responsableDto, responsable);

            await _responsableRepository.UpdateAsync(responsable);
        }

        public async Task DeleteResponsableAsync(int id)
        {
            var responsable = await _responsableRepository.GetByIdAsync(id);
            if (responsable == null)
                throw new NotFoundException("Aucun responsable correspondant n'a été trouvé.");

            await _responsableRepository.DeleteAsync(id);
        }
    }
}