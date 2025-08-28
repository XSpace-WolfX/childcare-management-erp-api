using Asp.Versioning;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionAssociatifERP.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DonneesSupplementairesController : ControllerBase
    {
        private readonly IDonneeSupplementaireService _donneeSupplementaireService;

        public DonneesSupplementairesController(IDonneeSupplementaireService donneeSupplementaireService)
        {
            _donneeSupplementaireService = donneeSupplementaireService;
        }

        // GET: api/v1/donneessupplementaires
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _donneeSupplementaireService.GetAllDonneesSupplementairesAsync();

            return Ok(result);
        }

        // GET: api/v1/donneessupplementaires/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _donneeSupplementaireService.GetDonneeSupplementaireAsync(id);

            return Ok(result);
        }

        // POST: api/v1/donneessupplementaires
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDonneeSupplementaireDto donneeSupplementaireDto)
        {
            var result = await _donneeSupplementaireService.CreateDonneeSupplementaireAsync(donneeSupplementaireDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/v1/donneessupplementaires/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDonneeSupplementaireDto donneeSupplementaireDto)
        {
            await _donneeSupplementaireService.UpdateDonneeSupplementaireAsync(id, donneeSupplementaireDto);

            return NoContent();
        }

        // DELETE: api/v1/donneessupplementaires/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _donneeSupplementaireService.DeleteDonneeSupplementaireAsync(id);

            return NoContent();
        }
    }
}