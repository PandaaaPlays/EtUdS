using System.Security.Claims;
using etuds.Server.Entities;
using etuds.Server.Services.Soumission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/Soumissions")]
public class SoumissionController : ControllerBase
{
    private readonly ISoumissionService _service;

    public SoumissionController(ISoumissionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Soumission>>> GetAll()
    {
        var entities = await _service.GetAll();
        return Ok(entities);
    }
    
    [Authorize]
    [HttpGet("{idDevoir}/equipe")]
    public async Task<IActionResult> GetAllOfEquipe(int idDevoir)
    {
        var results = await _service.GetByEquipeDevoir(idDevoir, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        return Ok(results);
    }
    
    [HttpPost]
    public async Task<IActionResult> Ajouter([FromBody] Soumission soumission)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var addedSoumission = await _service.Add(soumission);
        return CreatedAtAction(nameof(Ajouter), new { id = addedSoumission.Id }, addedSoumission);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] Soumission updatedSoumission)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != updatedSoumission.Id)
            return BadRequest("L'id fournis dans la requête n'est pas le même que celui de la soumission à modifier.");

        var soumission = await _service.GetById(id);
        if (soumission == null)
            return NotFound();

        await _service.Update(soumission, updatedSoumission);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var soumission = await _service.GetById(id);
        if (soumission == null)
            return NotFound();

        await _service.Delete(id, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadFichier(int id)
    {
        var result = await _service.DownloadById(id, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), false);

        if (result == null)
        {
            return NotFound(new { message = "Fichier non trouvé." });
        }
        
        Response.Headers.Append("X-File-Name", result.Value.Item3);
        return File(result.Value.Item1, result.Value.Item2, result.Value.Item3);
    }
    
    [Authorize]
    [HttpGet("{id}/Correction")]
    public async Task<IActionResult> DownloadFichierCorrection(int id)
    {
        var result = await _service.DownloadById(id, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), true);

        if (result == null)
        {
            return NotFound(new { message = "Fichier non trouvé." });
        }
        
        Response.Headers.Append("X-File-Name", result.Value.Item3);
        return File(result.Value.Item1, result.Value.Item2, result.Value.Item3);
    }
    
    [Authorize]
    [HttpPost("{idDevoir}")]
    public async Task<IActionResult> UploadFichier(int idDevoir, [FromForm] List<IFormFile> fichiers)
    {
        if (fichiers == null || fichiers.Count == 0)
        {
            return BadRequest("Aucun fichier reçu.");
        }

        await _service.UploadFichiers(idDevoir, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), fichiers);
        return NoContent();
    }
}