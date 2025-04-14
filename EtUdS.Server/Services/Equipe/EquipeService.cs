using EtUdS.Server.Dtos;
using EtUdS.Server.Repositories;
using EtUdS.Server.Repositories.Equipe;
using etuds.Server.Repositories.Etudiant;
using EtUdS.Server.Repositories.Membre;

namespace etuds.Server.Services.Equipe;

public class EquipeService : IEquipeService
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IMembreRepository _membreRepository;
    private readonly IEtudiantRepository _etudiantRepository;

    public EquipeService(IEquipeRepository equipeRepository, IMembreRepository membreRepository, IEtudiantRepository etudiantRepository)
    {
        _equipeRepository = equipeRepository;
        _membreRepository = membreRepository;
        _etudiantRepository = etudiantRepository;
    }

    public async Task<Entities.Equipe> GetById(int id)
    {
        return await _equipeRepository.GetById<Entities.Equipe>(id);
    }
    
    public async Task<EquipeDetailsDTO> GetDetailsMembres(int id)
    {
        var equipe = await GetById(id);
        if (equipe == null)
            return null;
        
        var membres = await _membreRepository.GetAllMembresByEquipeId(equipe.Id);
        
        var etudiants = new List<MembreDetailsDTO>();
        foreach (var membre in membres)
        {
            var etudiant = await _etudiantRepository.GetById<Entities.Etudiant>(membre.IdEtudiant);
            if (etudiant != null)
            {
                etudiants.Add(new MembreDetailsDTO()
                {
                    MembreId = membre.Id, 
                    Accepte = membre.Accepte,
                    EtudiantId = etudiant.Id, 
                    Prenom = etudiant.Prenom, 
                    Nom = etudiant.Nom, 
                    Courriel = etudiant.Courriel
                });
            }
        }

        return new EquipeDetailsDTO
        {
            Equipe = equipe,
            Etudiants = etudiants
        };
    }

    public async Task<IEnumerable<Entities.Equipe>> GetAll()
    {
        return await _equipeRepository.GetAll<Entities.Equipe>();
    }
    
    public async Task<Entities.Equipe> Add(Entities.Equipe equipe)
    {
        var addedEquipe = await _equipeRepository.Add(equipe);
        await _equipeRepository.Save();
        return addedEquipe;
    }
    
    public async Task Update(Entities.Equipe equipe, Entities.Equipe updatedEquipe)
    {
        equipe.IdDevoir = updatedEquipe.IdDevoir;
        equipe.Note = updatedEquipe.Note;
        equipe.EtatSoumission = updatedEquipe.EtatSoumission;
        
        _equipeRepository.Update(equipe);
        await _equipeRepository.Save();
    }

    public async Task Delete(Entities.Equipe equipe)
    {
        _equipeRepository.Delete(equipe);
        await _equipeRepository.Save();
    }
}