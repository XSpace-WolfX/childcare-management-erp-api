using Asp.Versioning;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionAssociatifERP.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ChildrenController : ControllerBase
    {
        private readonly IChildService _childService;

        public ChildrenController(IChildService childService)
        {
            _childService = childService;
        }

        // GET: api/v1/children
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _childService.GetAllChildrenAsync();

            return Ok(result);
        }

        // GET: api/v1/children/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _childService.GetChildAsync(id);

            return Ok(result);
        }

        // GET: api/v1/children/{id}/with-guardians
        [HttpGet("{id}/with-guardians")]
        public async Task<IActionResult> GetWithGuardians(int id)
        {
            var result = await _childService.GetChildWithGuardiansAsync(id);

            return Ok(result);
        }

        // GET: api/v1/children/{id}/with-authorized-people
        [HttpGet("{id}/with-authorized-people")]
        public async Task<IActionResult> GetWithAuthorizedPeople(int id)
        {
            var result = await _childService.GetChildWithAuthorizedPeopleAsync(id);

            return Ok(result);
        }

        // GET: api/v1/children/{id}/with-additional-datas
        [HttpGet("{id}/with-additional-datas")]
        public async Task<IActionResult> GetWithAdditionalDatas(int id)
        {
            var result = await _childService.GetChildWithAdditionalDatasAsync(id);

            return Ok(result);
        }

        // POST: api/v1/children
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChildDto childDto)
        {
            if (childDto == null)
                return BadRequest(new { Message = "Le corps de la requête ne peut pas être vide." });

            var result = await _childService.CreateChildAsync(childDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/v1/children/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChildDto childDto)
        {
            await _childService.UpdateChildAsync(id, childDto);

            return NoContent();
        }

        // DELETE: api/v1/children/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _childService.DeleteChildAsync(id);

            return NoContent();
        }
    }
}