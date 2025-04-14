using System.Security.Claims;
using EtUdS.Server.Dtos;
using etuds.Server.Entities;
using etuds.Server.Services.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;

[ApiController]
[Route("api/Messages")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _service;

    public MessageController(IMessageService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetAll()
    {
        var entities = await _service.GetAll();
        return Ok(entities);
    }

    [Authorize]
    [HttpGet("{idConversation}")]
    public async Task<ActionResult<IEnumerable<MessagesVuesDTO>>> GetAllOfConversation(int idConversation)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var messages = await _service.GetMessagesOfConversation(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), idConversation);
        return Ok(messages);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Ajouter([FromBody] Message message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var addedMessage = await _service.Add(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value), message);
        return CreatedAtAction(nameof(Ajouter), new { id = addedMessage.Id }, addedMessage);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Modifier(int id, [FromBody] Message updatedMessage)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != updatedMessage.Id)
            return BadRequest("L'id fournis dans la requête n'est pas le même que celui de la message à modifier.");

        var message = await _service.GetById(id);
        if (message == null)
            return NotFound();

        await _service.Update(message, updatedMessage);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var message = await _service.GetById(id);
        if (message == null)
            return NotFound();

        await _service.Delete(message);
        return NoContent();
    }
}