using Asp.Versioning;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChildcareManagementERP.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FinancialInformationsController : ControllerBase
    {
        public readonly IFinancialInformationService _financialInformationService;

        public FinancialInformationsController(IFinancialInformationService financialInformationService)
        {
            _financialInformationService = financialInformationService;
        }

        // GET: api/v1/financialinformations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _financialInformationService.GetAllFinancialInformationsAsync();

            return Ok(result);
        }

        // GET: api/v1/financialinformations/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _financialInformationService.GetFinancialInformationAsync(id);

            return Ok(result);
        }

        // POST: api/v1/financialinformations
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFinancialInformationDto financialInformationDto)
        {
            var result = await _financialInformationService.CreateFinancialInformationAsync(financialInformationDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/v1/financialinformations/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFinancialInformationDto financialInformationDto)
        {
            await _financialInformationService.UpdateFinancialInformationAsync(id, financialInformationDto);

            return NoContent();
        }

        // DELETE: api/v1/financialinformations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _financialInformationService.DeleteFinancialInformationAsync(id);

            return NoContent();
        }
    }
}