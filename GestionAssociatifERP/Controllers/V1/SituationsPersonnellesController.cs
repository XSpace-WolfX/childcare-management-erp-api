using Asp.Versioning;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionAssociatifERP.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SituationsPersonnellesController : ControllerBase
    {
        private readonly ISituationPersonnelleService _situationPersonnelleService;

        public SituationsPersonnellesController(ISituationPersonnelleService situationPersonnelleService)
        {
            _situationPersonnelleService = situationPersonnelleService;
        }

        // GET: api/situationspersonnelles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _situationPersonnelleService.GetAllSituationsPersonnellesAsync();

            return Ok(result);
        }

        // GET: api/situationspersonnelles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _situationPersonnelleService.GetSituationPersonnelleAsync(id);

            return Ok(result);
        }

        // POST: api/situationspersonnelles
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSituationPersonnelleDto situationPersonnelleDto)
        {
            var result = await _situationPersonnelleService.CreateSituationPersonnelleAsync(situationPersonnelleDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/situationspersonnelles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSituationPersonnelleDto situationPersonnelleDto)
        {
            await _situationPersonnelleService.UpdateSituationPersonnelleAsync(id, situationPersonnelleDto);

            return NoContent();
        }

        // DELETE: api/situationspersonnelles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _situationPersonnelleService.DeleteSituationPersonnelleAsync(id);

            return NoContent();
        }
    }
}