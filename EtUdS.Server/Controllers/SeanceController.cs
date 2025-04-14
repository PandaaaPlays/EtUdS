using System.Security.Claims;
using etuds.Server.Entities;
using etuds.Server.Services.Seance;
using etuds.Server.Services.Seance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/Seances")]
public class SeanceController : ControllerBase
{
    private readonly ISeanceService _service;

    public SeanceController(ISeanceService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Seance>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Seance>>> GetAll()
    {
        var entities = await _service.GetAll();
        return Ok(entities);
    }
    
    [HttpGet("utilisateur-courant")]
    public async Task<ActionResult<IEnumerable<Seance>>> GetAllCourant()
    {
        var entities = await _service.GetAll();
        return Ok(entities);
    }
    
    [Authorize]
    [HttpGet("cours-details")]
    public async Task<IActionResult> GetSeancesAvecDetailsSeance(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null
    )
    {
        var result = await _service
            .GetSeanceAvecDetailsCours(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), startDate, endDate);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Ajouter([FromBody] Seance seance)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var addedSeance = await _service.Add(seance);
        return CreatedAtAction(nameof(Ajouter), new { id = addedSeance.Id }, addedSeance);
    }
    
    [HttpPost("serie")]
    public async Task<IActionResult> AjouterSerie(
        [FromBody] Seance seance,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null
        )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (startDate == null || endDate == null)
            return BadRequest("Vous devez spécifier les dates de la série.");

        var addedSeance = await _service.AddSerie(seance, startDate.Value, endDate.Value);
        return Created("serie", addedSeance);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] Seance updatedSeance)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != updatedSeance.Id)
            return BadRequest("L'id fournis dans la requête n'est pas le même que celui de la seance à modifier.");

        var seance = await _service.GetById(id);
        if (seance == null)
            return NotFound();

        await _service.Update(seance, updatedSeance);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var seance = await _service.GetById(id);
        if (seance == null)
            return NotFound();

        await _service.Delete(seance);
        return NoContent();
    }
}