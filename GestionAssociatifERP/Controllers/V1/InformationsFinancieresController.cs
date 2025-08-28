using Asp.Versioning;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionAssociatifERP.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class InformationsFinancieresController : ControllerBase
    {
        public readonly IInformationFinanciereService _informationFinanciereService;

        public InformationsFinancieresController(IInformationFinanciereService informationFinanciereService)
        {
            _informationFinanciereService = informationFinanciereService;
        }

        // GET: api/v1/informationsfinancieres
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _informationFinanciereService.GetAllInformationsFinancieresAsync();

            return Ok(result);
        }

        // GET: api/v1/informationsfinancieres/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _informationFinanciereService.GetInformationFinanciereAsync(id);

            return Ok(result);
        }

        // POST: api/v1/informationsfinancieres
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInformationFinanciereDto informationFinanciereDto)
        {
            var result = await _informationFinanciereService.CreateInformationFinanciereAsync(informationFinanciereDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/v1/informationsfinancieres/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInformationFinanciereDto informationFinanciereDto)
        {
            await _informationFinanciereService.UpdateInformationFinanciereAsync(id, informationFinanciereDto);

            return NoContent();
        }

        // DELETE: api/v1/informationsfinancieres/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _informationFinanciereService.DeleteInformationFinanciereAsync(id);

            return NoContent();
        }
    }
}