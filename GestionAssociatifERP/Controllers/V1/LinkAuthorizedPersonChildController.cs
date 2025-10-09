using Asp.Versioning;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionAssociatifERP.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LinkAuthorizedPersonChildController : ControllerBase
    {
        private readonly ILinkAuthorizedPersonChildService _linkAuthorizedPersonChildService;

        public LinkAuthorizedPersonChildController(ILinkAuthorizedPersonChildService linkAuthorizedPersonChildService)
        {
            _linkAuthorizedPersonChildService = linkAuthorizedPersonChildService;
        }

        // GET: api/v1/linkauthorizedpersonchild/authorized-person/{authorizedPersonId}
        [HttpGet("authorized-person/{authorizedPersonId}")]
        public async Task<IActionResult> GetChildrenByAuthorizedPerson(int authorizedPersonId)
        {
            var result = await _linkAuthorizedPersonChildService.GetChildrenByAuthorizedPersonIdAsync(authorizedPersonId);

            return Ok(result);
        }

        // GET: api/v1/linkauthorizedpersonchild/child/{childId}
        [HttpGet("child/{childId}")]
        public async Task<IActionResult> GetAuthorizedPeopleByChild(int childId)
        {
            var result = await _linkAuthorizedPersonChildService.GetAuthorizedPeopleByChildIdAsync(childId);

            return Ok(result);
        }

        // GET: api/v1/linkauthorizedpersonchild/authorized-person/{authorizedPersonId}/child/{childId}
        [HttpGet("authorized-person/{authorizedPersonId}/child/{childId}")]
        public async Task<IActionResult> Exists(int authorizedPersonId, int childId)
        {
            var result = await _linkAuthorizedPersonChildService.ExistsLinkAuthorizedPersonChildAsync(authorizedPersonId, childId);

            return Ok(result);
        }

        // POST: api/v1/linkauthorizedpersonchild
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLinkAuthorizedPersonChildDto linkAuthorizedPersonChildDto)
        {
            var result = await _linkAuthorizedPersonChildService.CreateLinkAuthorizedPersonChildAsync(linkAuthorizedPersonChildDto);

            return Ok(result);
        }

        // PUT: api/v1/linkauthorizedpersonchild
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateLinkAuthorizedPersonChildDto linkAuthorizedPersonChildDto)
        {
            if (linkAuthorizedPersonChildDto == null)
                return BadRequest(new { Message = "Le corps de la requête ne peut pas être vide." });

            await _linkAuthorizedPersonChildService.UpdateLinkAuthorizedPersonChildAsync(linkAuthorizedPersonChildDto);

            return NoContent();
        }

        // DELETE: api/v1/linkauthorizedpersonchild/authorized-person/{authorizedPersonId}/child/{childId}
        [HttpDelete("authorized-person/{authorizedPersonId}/child/{childId}")]
        public async Task<IActionResult> Delete(int authorizedPersonId, int childId)
        {
            await _linkAuthorizedPersonChildService.RemoveLinkAuthorizedPersonChildAsync(authorizedPersonId, childId);

            return NoContent();
        }
    }
}