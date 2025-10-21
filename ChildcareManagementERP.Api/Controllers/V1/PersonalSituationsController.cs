using Asp.Versioning;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChildcareManagementERP.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PersonalSituationsController : ControllerBase
    {
        private readonly IPersonalSituationService _personalSituationService;

        public PersonalSituationsController(IPersonalSituationService personalSituationService)
        {
            _personalSituationService = personalSituationService;
        }

        // GET: api/personalsituations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _personalSituationService.GetAllPersonalSituationsAsync();

            return Ok(result);
        }

        // GET: api/personalsituations/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _personalSituationService.GetPersonalSituationAsync(id);

            return Ok(result);
        }

        // POST: api/personalsituations
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePersonalSituationDto personalSituationDto)
        {
            var result = await _personalSituationService.CreatePersonalSituationAsync(personalSituationDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/personalsituations/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonalSituationDto personalSituationDto)
        {
            await _personalSituationService.UpdatePersonalSituationAsync(id, personalSituationDto);

            return NoContent();
        }

        // DELETE: api/personalsituations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _personalSituationService.DeletePersonalSituationAsync(id);

            return NoContent();
        }
    }
}