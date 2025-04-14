using System.Security.Claims;
using EtUdS.Server.Dtos;
using etuds.Server.Entities;
using etuds.Server.Services.Devoir;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/Devoirs")]
public class DevoirController : ControllerBase
{
    private readonly IDevoirService _devoirService;

    public DevoirController(IDevoirService devoirService)
    {
        _devoirService = devoirService;
    }
    
    [Authorize]
    [HttpGet("{idDevoir}/equipe")]
    public async Task<ActionResult<int?>> GetEquipeOfCurrentEtudiantPourDevoir(int idDevoir)
    {
        var equipe = await _devoirService.GetEquipeOfCurrentEtudiantPourDevoir(
            idDevoir, 
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
        );
        
        return Ok(equipe?.Id);
    }
    
    [Authorize]
    [HttpGet("{idDevoir}/invitations")]
    public async Task<ActionResult<InvitationDTO>> GetInvitationsOfCurrentEtudiantPourDevoir(int idDevoir)
    {
        var invitations = await _devoirService.GetInvitationsOfCurrentEtudiantPourDevoir(
            idDevoir, 
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
        );
        
        return Ok(invitations);
    }
    
    [HttpGet("{idDevoir}/etudiants-disponibles")]
    public async Task<ActionResult<IEnumerable<Membre>>> GetEtudiantsDisponiblePourDevoir(int idDevoir)
    {
        var entities = await _devoirService.GetEtudiantsDisponiblePourDevoir(idDevoir);
        return Ok(entities);
    }
    
    [Authorize]
    [HttpGet("cours-details")]
    public async Task<IActionResult> GetDevoirAvecDetailsCoursPourUtilisateurCourant(
        [FromQuery] string? cours = null, 
        [FromQuery] string? etat = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null
    )
    {
        var result = await _devoirService
            .GetDevoirAvecDetailsCours(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), cours, etat, startDate, endDate);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("{id}/details")]
    public async Task<ActionResult<DevoirDetailsDTO>> GetByIdAvecDetails(int id)
    {
        var entity = await _devoirService.GetByIdAvecDetails(id, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }
    
    // Accessibles seulement via le Swagger : 
    [HttpGet("{id}")]
    public async Task<ActionResult<Devoir>> GetById(int id)
    {
        var entity = await _devoirService.GetById(id);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Devoir>>> GetAll()
    {
        var entities = await _devoirService.GetAll();
        return Ok(entities);
    }
    
    [HttpPost]
    public async Task<IActionResult> Ajouter([FromBody] Devoir devoir)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var addedDevoir = await _devoirService.Add(devoir);
        return CreatedAtAction(nameof(Ajouter), new { id = addedDevoir.Id }, addedDevoir);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] Devoir updatedDevoir)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != updatedDevoir.Id)
            return BadRequest("L'id fournis dans la requête n'est pas le même que celui du devoir à modifier.");

        var devoir = await _devoirService.GetById(id);
        if (devoir == null)
            return NotFound();

        await _devoirService.Update(devoir, updatedDevoir);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var devoir = await _devoirService.GetById(id);
        if (devoir == null)
            return NotFound();

        await _devoirService.Delete(devoir);
        return NoContent();
    }
}
