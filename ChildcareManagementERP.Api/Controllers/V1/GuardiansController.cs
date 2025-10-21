using Asp.Versioning;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChildcareManagementERP.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class GuardiansController : ControllerBase
    {
        private readonly IGuardianService _guardianService;

        public GuardiansController(IGuardianService guardianService)
        {
            _guardianService = guardianService;
        }

        // GET: api/v1/guardians
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _guardianService.GetAllGuardiansAsync();

            return Ok(result);
        }

        // GET: api/v1/guardians/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _guardianService.GetGuardianAsync(id);

            return Ok(result);
        }

        // GET: api/v1/guardians/{id}/with-children
        [HttpGet("{id}/with-children")]
        public async Task<IActionResult> GetWithChildren(int id)
        {
            var result = await _guardianService.GetGuardianWithChildrenAsync(id);

            return Ok(result);
        }

        // GET: api/v1/guardians/{id}/with-financial-information
        [HttpGet("{id}/with-financial-information")]
        public async Task<IActionResult> GetWithFinancialInformation(int id)
        {
            var result = await _guardianService.GetGuardianWithFinancialInformationAsync(id);

            return Ok(result);
        }

        // GET: api/v1/guardians/{id}/with-personal-situation
        [HttpGet("{id}/with-personal-situation")]
        public async Task<IActionResult> GetWithPersonalSituation(int id)
        {
            var result = await _guardianService.GetGuardianWithPersonalSituationAsync(id);

            return Ok(result);
        }

        // POST: api/v1/guardians
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGuardianDto guardianDto)
        {
            var result = await _guardianService.CreateGuardianAsync(guardianDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/v1/guardians/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGuardianDto guardianDto)
        {
            await _guardianService.UpdateGuardianAsync(id, guardianDto);

            return NoContent();
        }

        // DELETE: api/v1/guardians/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _guardianService.DeleteGuardianAsync(id);

            return NoContent();
        }
    }
}