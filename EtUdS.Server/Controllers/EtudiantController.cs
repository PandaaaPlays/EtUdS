using System.Security.Claims;
using etuds.Server.Entities;
using etuds.Server.Services.Etudiant;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/Etudiants")]
public class EtudiantController : ControllerBase
{
    private readonly IEtudiantService _service;

    public EtudiantController(IEtudiantService service)
    {
        _service = service;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Etudiant>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }
    
    [HttpGet("utilisateur-courant")]
    public async Task<ActionResult<Etudiant>> GetEtudiantConnecte()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized();
        }
        
        var entities = await _service.GetById(
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
        );
        return Ok(entities);
    }
    
    [HttpGet("authorized/{id}")]
    public async Task<ActionResult<Etudiant>> GetByIdWithPassword(int id)
    {
        var entity = await _service.GetByIdWithPassword(id);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Etudiant>>> GetAll()
    {
        var entities = await _service.GetAll();
        return Ok(entities);
    }
    
    [HttpPost]
    public async Task<IActionResult> Ajouter([FromBody] Etudiant etudiant)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var addedEtudiant = await _service.Add(etudiant);
        return CreatedAtAction(nameof(Ajouter), new { id = addedEtudiant.Id }, addedEtudiant);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] Etudiant updatedEtudiant)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != updatedEtudiant.Id)
            return BadRequest("L'id fournis dans la requête n'est pas le même que celui de la etudiant à modifier.");

        var etudiant = await _service.GetByIdWithPassword(id);
        if (etudiant == null)
            return NotFound();

        await _service.Update(etudiant, updatedEtudiant);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var etudiant = await _service.GetByIdWithPassword(id);
        if (etudiant == null)
            return NotFound();

        await _service.Delete(etudiant);
        return NoContent();
    }
}