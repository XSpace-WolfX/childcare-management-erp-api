using Asp.Versioning;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChildcareManagementERP.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AdditionalDatasController : ControllerBase
    {
        private readonly IAdditionalDataService _additionalDataService;

        public AdditionalDatasController(IAdditionalDataService additionalDataService)
        {
            _additionalDataService = additionalDataService;
        }

        // GET: api/v1/additionaldatas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _additionalDataService.GetAllAdditionalDatasAsync();

            return Ok(result);
        }

        // GET: api/v1/additionaldatas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _additionalDataService.GetAdditionalDataAsync(id);

            return Ok(result);
        }

        // POST: api/v1/additionaldatas
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdditionalDataDto additionalDataDto)
        {
            var result = await _additionalDataService.CreateAdditionalDataAsync(additionalDataDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/v1/additionaldatas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdditionalDataDto additionalDataDto)
        {
            await _additionalDataService.UpdateAdditionalDataAsync(id, additionalDataDto);

            return NoContent();
        }

        // DELETE: api/v1/additionaldatas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _additionalDataService.DeleteAdditionalDataAsync(id);

            return NoContent();
        }
    }
}