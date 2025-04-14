using System.Security.Claims;
using etuds.Server.Entities;
using etuds.Server.Services.Etudiant;
using EtUdS.Server.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;


[ApiController]
[Route("api/Connexion")]

public class ConnexionController : ControllerBase
{
    private readonly IEtudiantService _etudiantService;

    public ConnexionController(IEtudiantService etudiantService)
    {
        _etudiantService = etudiantService;
    }

    [HttpPost]
    public async Task<ActionResult<Etudiant>> AuthoriserConnexion([FromBody] requeteConnexion requete)
    {
        var utilisateur = await _etudiantService.GetByCourriel(requete.courriel);
        if (utilisateur == null || !PasswordHasher.VerifyPassword(requete.motDePasse, utilisateur.MotDePasse))
        {
            
            return Unauthorized();
        }
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, utilisateur.Id.ToString()),
            new Claim(ClaimTypes.Name, utilisateur.Prenom + " " + utilisateur.Nom)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = true, 
            ExpiresUtc = DateTime.UtcNow.AddHours(1) 
        });
        
        return Ok(utilisateur);
    }
    
    [HttpPost("logout")]
    public async Task<ActionResult<Etudiant>> LogoutConnexion()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            await HttpContext.SignOutAsync();
        }

        return Ok();
    }

    [HttpGet("Utilisateur")]
    public IActionResult GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new { id = userId, name = userName });
        }

        return Unauthorized();
    }

}

public class requeteConnexion
{
    public string courriel { get; set; }
    public string motDePasse { get; set; }
}