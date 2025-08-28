using Asp.Versioning;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionAssociatifERP.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EnfantsController : ControllerBase
    {
        private readonly IEnfantService _enfantService;

        public EnfantsController(IEnfantService enfantService)
        {
            _enfantService = enfantService;
        }

        // GET: api/v1/enfants
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _enfantService.GetAllEnfantsAsync();

            return Ok(result);
        }

        // GET: api/v1/enfants/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _enfantService.GetEnfantAsync(id);

            return Ok(result);
        }

        // GET: api/v1/enfants/{id}/with-responsables
        [HttpGet("{id}/with-responsables")]
        public async Task<IActionResult> GetWithResponsables(int id)
        {
            var result = await _enfantService.GetEnfantWithResponsablesAsync(id);

            return Ok(result);
        }

        // GET: api/v1/enfants/{id}/with-personnes-autorisees
        [HttpGet("{id}/with-personnes-autorisees")]
        public async Task<IActionResult> GetWithPersonnesAutorisees(int id)
        {
            var result = await _enfantService.GetEnfantWithPersonnesAutoriseesAsync(id);

            return Ok(result);
        }

        // GET: api/v1/enfants/{id}/with-donnees-supplementaires
        [HttpGet("{id}/with-donnees-supplementaires")]
        public async Task<IActionResult> GetWithDonneesSupplementaires(int id)
        {
            var result = await _enfantService.GetEnfantWithDonneesSupplementairesAsync(id);

            return Ok(result);
        }

        // POST: api/v1/enfants
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEnfantDto enfantDto)
        {
            var result = await _enfantService.CreateEnfantAsync(enfantDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/v1/enfants/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEnfantDto enfantDto)
        {
            await _enfantService.UpdateEnfantAsync(id, enfantDto);

            return NoContent();
        }

        // DELETE: api/v1/enfants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _enfantService.DeleteEnfantAsync(id);

            return NoContent();
        }
    }
}