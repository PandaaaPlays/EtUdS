using etuds.Server.Entities;
using etuds.Server.Services.Etudiant;
using EtUdS.Server.Utils;
using Microsoft.AspNetCore.Mvc;

namespace etuds.Server.Controllers;


[ApiController]
[Route("api/Inscription")]
public class InscriptionController: ControllerBase
{
    private readonly IEtudiantService _etudiantService;

    public InscriptionController(IEtudiantService etudiantService) 
    { 
        _etudiantService = etudiantService;
    }

    [HttpPost]
    public async Task<ActionResult<Etudiant>> InscrireUtilisateur([FromBody] requeteInscription requete)
    {
        var utilisateurExisteDejar = await _etudiantService.GetByCourriel(requete.courriel);
        if (utilisateurExisteDejar != null)
        {
            return Conflict("Un compte avec ce courriel existe déjà.");
        }
        
        var newEtudiant = new Etudiant
        {
            Courriel = requete.courriel,
            MotDePasse = PasswordHasher.HashPassword(requete.motDePasse),
            Nom = requete.nom, 
            Prenom = requete.prenom
        };
        
        var etudiant = await _etudiantService.Add(newEtudiant);
        
        return CreatedAtAction(nameof(InscrireUtilisateur), new {id = etudiant.Id}, etudiant);
        
    }

    public class requeteInscription
    {
        public string courriel { get; set; }
        public string motDePasse { get; set; }
        public string nom { get; set; }
        public string prenom { get; set; }
    }

}