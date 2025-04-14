using System.Security.Claims;
using etuds.Server.Services.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/Documents")]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _service;

    public DocumentController(IDocumentService service)
    {
        _service = service;
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadFichier(int id)
    {
        var result = await _service.DownloadById(id, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));

        if (result == null)
        {
            return NotFound(new { message = "Fichier non trouvé." });
        }
        
        Response.Headers.Append("X-File-Name", result.Value.Item3);
        return File(result.Value.Item1, result.Value.Item2, result.Value.Item3);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> SupprimerFichier(int id)
    {
        await _service.Delete(id, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("{idDevoir}/equipe")]
    public async Task<IActionResult> GetAllOfEquipe(int idDevoir)
    {
        var results = await _service.GetByEquipeDevoir(idDevoir, int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        return Ok(results);
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