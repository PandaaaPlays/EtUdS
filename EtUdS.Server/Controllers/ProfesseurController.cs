using etuds.Server.Entities;
using etuds.Server.Services.Professeur;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/Professeur")]
public class ProfesseurController: ControllerBase
{
    private readonly IProfesseurService _service;

    public ProfesseurController(IProfesseurService service)
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Professeur>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }
}