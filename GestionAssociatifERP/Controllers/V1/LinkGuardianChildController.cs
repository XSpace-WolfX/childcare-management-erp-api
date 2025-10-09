using Asp.Versioning;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionAssociatifERP.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LinkGuardianChildController : ControllerBase
    {
        private readonly ILinkGuardianChildService _linkGuardianChildService;

        public LinkGuardianChildController(ILinkGuardianChildService linkGuardianChildService)
        {
            _linkGuardianChildService = linkGuardianChildService;
        }

        // GET: api/v1/linkguardianchild/guardian/{guardianId}
        [HttpGet("guardian/{guardianId}")]
        public async Task<IActionResult> GetChildrenByGuardian(int guardianId)
        {
            var result = await _linkGuardianChildService.GetChildrenByGuardianIdAsync(guardianId);

            return Ok(result);
        }

        // GET: api/v1/linkguardianchild/child/{childId}
        [HttpGet("child/{childId}")]
        public async Task<IActionResult> GetGuardiansByChild(int childId)
        {
            var result = await _linkGuardianChildService.GetGuardiansByChildIdAsync(childId);

            return Ok(result);
        }

        // GET: api/v1/linkguardianchild/guardian/{guardianId}/child/{childId}
        [HttpGet("guardian/{guardianId}/child/{childId}")]
        public async Task<IActionResult> Exists(int guardianId, int childId)
        {
            var result = await _linkGuardianChildService.ExistsLinkGuardianChildAsync(guardianId, childId);

            return Ok(result);
        }

        // POST: api/v1/linkguardianchild
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLinkGuardianChildDto linkGuardianChildDto)
        {
            var result = await _linkGuardianChildService.CreateLinkGuardianChildAsync(linkGuardianChildDto);

            return Ok(result);
        }

        // PUT: api/v1/linkguardianchild
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateLinkGuardianChildDto linkGuardianChildDto)
        {
            if (linkGuardianChildDto == null)
                return BadRequest(new { Message = "Le corps de la requête ne peut pas être vide." });

            await _linkGuardianChildService.UpdateLinkGuardianChildAsync(linkGuardianChildDto);

            return NoContent();
        }

        // DELETE: api/v1/linkguardianchild/guardian/{guardianId}/child/{childId}
        [HttpDelete("guardian/{guardianId}/child/{childId}")]
        public async Task<IActionResult> Delete(int guardianId, int childId)
        {
            await _linkGuardianChildService.RemoveLinkGuardianChildAsync(guardianId, childId);

            return NoContent();
        }
    }
}