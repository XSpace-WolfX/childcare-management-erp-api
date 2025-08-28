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
    public class PersonnesAutoriseesController : ControllerBase
    {
        private readonly IPersonneAutoriseeService _personneAutoriseeService;

        public PersonnesAutoriseesController(IPersonneAutoriseeService personneAutoriseeService, IMapper mapper)
        {
            _personneAutoriseeService = personneAutoriseeService;
        }

        // GET: api/v1/personnesautorisees
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _personneAutoriseeService.GetAllPersonnesAutoriseesAsync();

            return Ok(result);
        }

        // GET: api/v1/personnesautorisees/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _personneAutoriseeService.GetPersonneAutoriseeAsync(id);

            return Ok(result);
        }

        // GET: api/v1/personnesautorisees/{id}/with-enfants
        [HttpGet("{id}/with-enfants")]
        public async Task<IActionResult> GetWithEnfants(int id)
        {
            var result = await _personneAutoriseeService.GetPersonneAutoriseeWithEnfantsAsync(id);

            return Ok(result);
        }

        // POST: api/v1/personnesautorisees
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePersonneAutoriseeDto personneAutoriseeDto)
        {
            var result = await _personneAutoriseeService.CreatePersonneAutoriseeAsync(personneAutoriseeDto);

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        // PUT: api/v1/personnesautorisees/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonneAutoriseeDto personneAutoriseeDto)
        {
            await _personneAutoriseeService.UpdatePersonneAutoriseeAsync(id, personneAutoriseeDto);

            return NoContent();
        }

        // DELETE: api/v1/personnesautorisees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _personneAutoriseeService.DeletePersonneAutoriseeAsync(id);

            return NoContent();
        }
    }
}