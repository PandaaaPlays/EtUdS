using System.Security.Claims;
using EtUdS.Server.Dtos;
using etuds.Server.Entities;
using etuds.Server.Services.Conversation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/Conversations")]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _service;

    public ConversationController(IConversationService service)
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


    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Conversation>>> GetAll()
    {
        var entities = await _service.GetAll(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        return Ok(entities);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Ajouter([FromBody] Conversation conversation)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var addedConversation = await _service.Add(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), conversation);
        return CreatedAtAction(nameof(Ajouter), new { id = addedConversation.Id }, addedConversation);
    }
    
    [HttpGet("{id}/participants")]
    public async Task<IEnumerable<ParticipantEtudiantDTO>> GetParticipants(int id)
    {
        var participants = await _service.GetParticipantsEtudiants(id);
        return participants;
    }
    
    [Authorize]
    [HttpPost("{id}/participants")]
    public async Task<IActionResult> AjouterParticipants(int id, List<int> etudiantIds)
    {
        await _service.AjouterParticipants(
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), 
            id, 
            etudiantIds);
        return NoContent();
    }
    
    [Authorize]
    [HttpPut("{id}/participants")]
    public async Task<IActionResult> ModifierParticipants(int id, List<int> etudiantIds)
    {
        await _service.ModifierParticipants(
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), 
            id, 
            etudiantIds);
        return NoContent();
    }
    
    [Authorize]
    [HttpPut("{id}/quitter")]
    public async Task<IActionResult> QuitterConversation(int id)
    {
        await _service.Quitter(
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), 
            id);
        return NoContent();
    }
    
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] Conversation updatedConversation)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != updatedConversation.Id)
            return BadRequest("L'id fournis dans la requête n'est pas le même que celui de la conversation à modifier.");

        var conversation = await _service.GetById(id);
        if (conversation == null)
            return NotFound();

        await _service.Update(conversation, updatedConversation);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var conversation = await _service.GetById(id);
        if (conversation == null)
            return NotFound();

        await _service.Delete(conversation);
        return NoContent();
    }
}