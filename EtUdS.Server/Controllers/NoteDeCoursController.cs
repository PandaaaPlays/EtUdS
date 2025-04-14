using etuds.Server.Services.NoteDeCours;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace etuds.Server.Controllers;

[ApiController]
[Route("api/NoteDeCours")]

public class NoteDeCoursController: ControllerBase
{
    private readonly INoteDeCoursService _service;

    public NoteDeCoursController(INoteDeCoursService service)
    {
        _service = service;
    }
    
    [HttpGet("{idCours}")]
    public async Task<IActionResult> GetAllOfCours(int idCours)
    {
        var results = await _service.GetByCours(idCours);
        return Ok(results);
    }
    
    [Authorize]
    [HttpDelete("{idCours}/{idNotes}")]
    public async Task<IActionResult> Supprimer(int idCours, int idNotes)
    {
        var noteDeCours = await _service.GetById(idNotes);
        if (noteDeCours == null)
            return NotFound();

        await _service.Delete(idCours, idNotes);
        return NoContent();
    }
    
    [HttpGet("{idCours}/{idNotes}")]
    public async Task<IActionResult> DownloadFichier(int idCours, int idNotes)
    {
        var result = await _service.DownloadById(idCours, idNotes);

        if (result == null)
        {
            return NotFound(new { message = "Fichier non trouvé." });
        }
        
        Response.Headers.Append("X-File-Name", result.Value.Item3);
        return File(result.Value.Item1, result.Value.Item2, result.Value.Item3);
    }
    
    [HttpPost("{idCours}")]
    public async Task<IActionResult> UploadFichier(int idCours, [FromForm] List<IFormFile> fichiers)
    {
        if (fichiers == null || fichiers.Count == 0)
        {
            return BadRequest("Aucun fichier reçu.");
        }

        await _service.UploadFichiers(idCours, fichiers);
        return NoContent();
    }
}