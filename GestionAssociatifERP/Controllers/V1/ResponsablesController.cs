using Asp.Versioning;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionAssociatifERP.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ResponsablesController : ControllerBase
    {
        private readonly IResponsableService _responsableService;

        public ResponsablesController(IResponsableService responsableService)
        {
            _responsableService = responsableService;
        }

        // GET: api/v1/responsables
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _responsableService.GetAllResponsablesAsync();

            return Ok(result);
        }

        // GET: api/v1/responsables/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _responsableService.GetResponsableAsync(id);

            return Ok(result);
        }

        // GET: api/v1/responsables/{id}/with-enfants
        [HttpGet("{id}/with-enfants")]
        public async Task<IActionResult> GetWithEnfants(int id)
        {
            var result = await _responsableService.GetResponsableWithEnfantsAsync(id);

            return Ok(result);
        }

        // GET: api/v1/responsables/{id}/with-information-financiere
        [HttpGet("{id}/with-information-financiere")]
        public async Task<IActionResult> GetWithInformationFinanciere(int id)
        {
            var result = await _responsableService.GetResponsableWithInformationFinanciereAsync(id);

            return Ok(result);
        }

        // GET: api/v1/responsables/{id}/with-situation-personnelle
        [HttpGet("{id}/with-situation-personnelle")]
        public async Task<IActionResult> GetWithSituationPersonnelle(int id)
        {
            var result = await _responsableService.GetResponsableWithSituationPersonnelleAsync(id);

            return Ok(result);
        }

        // POST: api/v1/responsables
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateResponsableDto responsableDto)
        {
            var result = await _responsableService.CreateResponsableAsync(responsableDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/v1/responsables/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateResponsableDto responsableDto)
        {
            await _responsableService.UpdateResponsableAsync(id, responsableDto);

            return NoContent();
        }

        // DELETE: api/v1/responsables/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _responsableService.DeleteResponsableAsync(id);

            return NoContent();
        }
    }
}