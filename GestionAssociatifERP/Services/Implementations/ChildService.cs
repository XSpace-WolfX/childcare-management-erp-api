using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Infrastructure.Persistence.Models;
using GestionAssociatifERP.Repositories;

namespace GestionAssociatifERP.Services
{
    public class ChildService : IChildService
    {
        private readonly IChildRepository _childRepository;
        private readonly IMapper _mapper;

        public ChildService(IChildRepository childRepository, IMapper mapper)
        {
            _childRepository = childRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChildDto>> GetAllChildrenAsync()
        {
            var children = await _childRepository.GetAllAsync();
            var childrenDto = _mapper.Map<IEnumerable<ChildDto>>(children);

            if (childrenDto == null)
                childrenDto = new List<ChildDto>();

            return childrenDto;
        }

        public async Task<ChildDto> GetChildAsync(int id)
        {
            var child = await _childRepository.GetByIdAsync(id);
            if (child == null)
                throw new Exception("Aucun enfant correspondant n'a été trouvé.");

            return _mapper.Map<ChildDto>(child);
        }

        public async Task<ChildWithGuardiansDto> GetChildWithGuardiansAsync(int id)
        {
            var child = await _childRepository.GetWithGuardiansAsync(id);
            if (child == null)
                throw new Exception("Aucun enfant correspondant n'a été trouvé.");

            return _mapper.Map<ChildWithGuardiansDto>(child);
        }

        public async Task<ChildWithAuthorizedPeopleDto> GetChildWithAuthorizedPeopleAsync(int id)
        {
            var child = await _childRepository.GetWithAuthorizedPeopleAsync(id);
            if (child == null)
                throw new Exception("Aucun enfant correspondant n'a été trouvé.");

            return _mapper.Map<ChildWithAuthorizedPeopleDto>(child);
        }

        public async Task<ChildWithAdditionalDatasDto> GetChildWithAdditionalDatasAsync(int id)
        {
            var child = await _childRepository.GetWithAdditionalDatasAsync(id);
            if (child == null)
                throw new Exception("Aucun enfant correspondant n'a été trouvé.");

            return _mapper.Map<ChildWithAdditionalDatasDto>(child);
        }

        public async Task<ChildDto> CreateChildAsync(CreateChildDto childDto)
        {
            var child = _mapper.Map<Child>(childDto);
            if (child == null)
                throw new Exception("Erreur lors de la création de l'enfant : Le Mapping a échoué.");

            await _childRepository.AddAsync(child);

            var createdChild = await _childRepository.GetByIdAsync(child.Id);
            if (createdChild == null)
                throw new Exception("Échec de la création de l'enfant.");

            return _mapper.Map<ChildDto>(createdChild);
        }

        public async Task UpdateChildAsync(int id, UpdateChildDto childDto)
        {
            if (id != childDto.Id)
                throw new Exception("L'identifiant de l'enfant ne correspond pas à celui de l'objet envoyé.");

            var child = await _childRepository.GetByIdAsync(id);
            if (child == null)
                throw new Exception("Aucun enfant correspondant n'a été trouvé.");

            _mapper.Map(childDto, child);

            await _childRepository.UpdateAsync(child);
        }

        public async Task DeleteChildAsync(int id)
        {
            var child = await _childRepository.GetByIdAsync(id);
            if (child == null)
                throw new Exception("Aucun enfant correspondant n'a été trouvé.");

            await _childRepository.DeleteAsync(id);
        }
    }
}