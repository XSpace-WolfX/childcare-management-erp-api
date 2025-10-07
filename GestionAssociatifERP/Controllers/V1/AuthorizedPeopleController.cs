using Asp.Versioning;
using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionAssociatifERP.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthorizedPeopleController : ControllerBase
    {
        private readonly IAuthorizedPersonService _authorizedPersonService;

        public AuthorizedPeopleController(IAuthorizedPersonService authorizedPersonService, IMapper mapper)
        {
            _authorizedPersonService = authorizedPersonService;
        }

        // GET: api/v1/authorizedpeople
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _authorizedPersonService.GetAllAuthorizedPeopleAsync();

            return Ok(result);
        }

        // GET: api/v1/authorizedpeople/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _authorizedPersonService.GetAuthorizedPersonAsync(id);

            return Ok(result);
        }

        // GET: api/v1/authorizedpeople/{id}/with-children
        [HttpGet("{id}/with-children")]
        public async Task<IActionResult> GetWithChildren(int id)
        {
            var result = await _authorizedPersonService.GetAuthorizedPersonWithChildrenAsync(id);

            return Ok(result);
        }

        // POST: api/v1/authorizedpeople
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuthorizedPersonDto authorizedPersonDto)
        {
            if (authorizedPersonDto == null)
                return BadRequest(new { Message = "Le corps de la requête ne peut pas être vide." });

            var result = await _authorizedPersonService.CreateAuthorizedPersonAsync(authorizedPersonDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/v1/authorizedpeople/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorizedPersonDto authorizedPersonDto)
        {
            await _authorizedPersonService.UpdateAuthorizedPersonAsync(id, authorizedPersonDto);

            return NoContent();
        }

        // DELETE: api/v1/authorizedpeople/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _authorizedPersonService.DeleteAuthorizedPersonAsync(id);

            return NoContent();
        }
    }
}