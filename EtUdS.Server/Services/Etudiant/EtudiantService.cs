using EtUdS.Server.Dtos;
using etuds.Server.Repositories.Etudiant;

namespace etuds.Server.Services.Etudiant;

public class EtudiantService: IEtudiantService
{
    private readonly IEtudiantRepository _repository;

    public EtudiantService(IEtudiantRepository repository)
    {
        _repository = repository;
    }

    public async Task<EtudiantDTO> GetById(int id)
    {
        var etudiant = await _repository.GetById<Entities.Etudiant>(id);
        return etudiant != null ? new EtudiantDTO
            { 
                Id = etudiant.Id, 
                Nom = etudiant.Nom, 
                Prenom = etudiant.Prenom, 
                Courriel = etudiant.Courriel 
            } : null;
    }
    
    public async Task<Entities.Etudiant> GetByIdWithPassword(int id)
    {
        return await _repository.GetById<Entities.Etudiant>(id);
    }
    
    public async Task<Entities.Etudiant> GetByCourriel(string courriel)
    {
        return await _repository.GetByCourriel(courriel);
    }
    

    public async Task<IEnumerable<EtudiantDTO>> GetAll()
    {
        var etudiants = await _repository.GetAll<Entities.Etudiant>();
        return etudiants.Select(e => new EtudiantDTO
        {
            Id = e.Id, 
            Nom = e.Nom, 
            Prenom = e.Prenom, 
            Courriel = e.Courriel 
        });
    }

    
    public async Task<Entities.Etudiant> Add(Entities.Etudiant etudiant)
    {
        var addedEtudiant = await _repository.Add(etudiant);
        await _repository.Save();
        return addedEtudiant;
    }
    
    public async Task Update(Entities.Etudiant etudiant, Entities.Etudiant updatedEtudiant)
    {
        etudiant.Nom = updatedEtudiant.Nom;
        etudiant.Prenom = updatedEtudiant.Prenom;
        etudiant.Courriel = updatedEtudiant.Courriel;
        etudiant.MotDePasse = updatedEtudiant.MotDePasse;
        
        _repository.Update(etudiant);
        await _repository.Save();
    }

    public async Task Delete(Entities.Etudiant etudiant)
    {
        _repository.Delete(etudiant);
        await _repository.Save();
    }
}