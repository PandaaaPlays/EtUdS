using System.Security.Claims;
using etuds.Server.Entities;
using etuds.Server.Services.Membre;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/Membres")]
public class MembreController : ControllerBase
{
    private readonly IMembreService _service;

    public MembreController(IMembreService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Membre>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Membre>>> GetAll()
    {
        var entities = await _service.GetAll();
        return Ok(entities);
    }
    
    [HttpGet("equipe/{idEquipe}")]
    public async Task<ActionResult<IEnumerable<Membre>>> GetAllMembresByEquipeId(int idEquipe)
    {
        var entities = await _service.GetAllMembresByEquipeId(idEquipe);
        return Ok(entities);
    }
    
    [HttpGet("non-disponible-devoir/{idDevoir}")]
    public async Task<ActionResult<IEnumerable<int>>> GetIdEtudiantsNonDisponiblesPourDevoir(int idDevoir)
    {
        var entities = await _service.GetIdEtudiantsNonDisponiblesPourDevoir(idDevoir);
        return Ok(entities);
    }

    [Authorize]
    [HttpPost("invitation/{idEquipe}/accepter")]
    public async Task<ActionResult> AccepterInvitation(int idEquipe)
    {
        await _service.AccepterInvitation(idEquipe, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("invitation/{idEquipe}/refuser")]
    public async Task<ActionResult> RefuserInvitation(int idEquipe)
    {
        await _service.RefuserInvitation(idEquipe, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        return NoContent();
    }
    
    [HttpPost]
    public async Task<IActionResult> Ajouter([FromBody] Membre membre)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var addedMembre = await _service.Add(membre);
        return CreatedAtAction(nameof(Ajouter), new { id = addedMembre.Id }, addedMembre);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] Membre updatedMembre)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != updatedMembre.Id)
            return BadRequest("L'id fournis dans la requête n'est pas le même que celui de la membre à modifier.");

        var membre = await _service.GetById(id);
        if (membre == null)
            return NotFound();

        await _service.Update(membre, updatedMembre);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var membre = await _service.GetById(id);
        if (membre == null)
            return NotFound();

        await _service.Delete(membre);
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("{devoirId}/utilisateur-courant")]
    public async Task<ActionResult<Etudiant>> SupprimerCourant(int devoirId)
    {
        await _service.Delete(devoirId, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        return NoContent();
    }
}