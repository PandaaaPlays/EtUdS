using etuds.Server.Entities;
using etuds.Server.Services.Equipe;
using etuds.Server.Services.Equipe;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/Equipes")]
public class EquipeController : ControllerBase
{
    private readonly IEquipeService _service;

    public EquipeController(IEquipeService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Equipe>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }
    
    [HttpGet("{id}/details-membres")]
    public async Task<ActionResult<Equipe>> GetDetailsMembres(int id)
    {
        var entity = await _service.GetDetailsMembres(id);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Equipe>>> GetAll()
    {
        var entities = await _service.GetAll();
        return Ok(entities);
    }
    
    [HttpPost]
    
    [HttpPost]
    public async Task<IActionResult> Ajouter([FromBody] Equipe equipe)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var addedEquipe = await _service.Add(equipe);
        return CreatedAtAction(nameof(Ajouter), new { id = addedEquipe.Id }, addedEquipe);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] Equipe updatedEquipe)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != updatedEquipe.Id)
            return BadRequest("L'id fournis dans la requête n'est pas le même que celui de la equipe à modifier.");

        var equipe = await _service.GetById(id);
        if (equipe == null)
            return NotFound();

        await _service.Update(equipe, updatedEquipe);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var equipe = await _service.GetById(id);
        if (equipe == null)
            return NotFound();

        await _service.Delete(equipe);
        return NoContent();
    }
}