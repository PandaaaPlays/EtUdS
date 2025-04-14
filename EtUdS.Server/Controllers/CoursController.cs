using System.Security.Claims;
using etuds.Server.Entities;
using etuds.Server.Services.Cours;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursController : ControllerBase
{
    private readonly ICoursService _service;

    public CoursController(ICoursService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cours>> GetById(int id)
    {
        var entity = await _service.GetById(id);
        if (entity == null)
            return NotFound();

        return Ok(entity);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cours>>> GetAll()
    {
        var entities = await _service.GetAll();
        return Ok(entities);
    }
    
    [Authorize]
    [HttpGet("utilisateur-courant")]
    public async Task<ActionResult<IEnumerable<Cours>>> GetAllCourant()
    {
        var entities = await _service
            .GetAllCoursEtudiant(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        return Ok(entities);
    }
    
    [HttpPost]
    public async Task<IActionResult> Ajouter([FromBody] Cours cours)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var addedCours = await _service.Add(cours);
        return CreatedAtAction(nameof(Ajouter), new { id = addedCours.Id }, addedCours);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] Cours updatedCours)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != updatedCours.Id)
            return BadRequest("L'id fournis dans la requête n'est pas le même que celui du cours à modifier.");

        var cours = await _service.GetById(id);
        if (cours == null)
            return NotFound();

        await _service.Update(cours, updatedCours);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var cours = await _service.GetById(id);
        if (cours == null)
            return NotFound();

        await _service.Delete(cours);
        return NoContent();
    }
}