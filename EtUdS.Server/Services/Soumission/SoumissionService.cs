using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using etuds.Server.Services.Devoir;
using etuds.Server.Services.Equipe;
using etuds.Server.Services.Membre;
using EtUdS.Server.Utils;

namespace etuds.Server.Services.Soumission;

public class SoumissionService : ISoumissionService
{
    private readonly IBaseRepository _repository;
    private readonly IDevoirService _devoirService;
    private readonly IEquipeService _equipeService;
    private readonly IMembreService _membreService;

    public SoumissionService(IBaseRepository repository, IDevoirService devoirService, IEquipeService equipeService, IMembreService membreService)
    {
        _repository = repository;
        _devoirService = devoirService;
        _equipeService = equipeService;
        _membreService = membreService;
    }

    public async Task<Entities.Soumission> GetById(int id)
    {
        return await _repository.GetById<Entities.Soumission>(id);
    }

    public async Task<IEnumerable<Entities.Soumission>> GetAll()
    {
        return await _repository.GetAll<Entities.Soumission>();
    }
    
    public async Task<IEnumerable<Entities.Soumission>> GetByEquipeDevoir(int idDevoir, int idEtudiant)
    {
        var equipe = await _devoirService.GetEquipeOfCurrentEtudiantPourDevoir(idDevoir, idEtudiant);
        var soumissions = await GetAll();
        return soumissions.Where(x => x.IdEquipe == equipe.Id);
    }
    
    public async Task<(byte[], string, string)?> DownloadById(int id, int idEtudiant, bool correction)
    {
        var soumission = await GetById(id);
        var equipe = await _equipeService.GetById(soumission.IdEquipe);
        var membres = await _membreService.GetAllMembresByEquipeId(equipe.Id);
        if (membres.All(m => m.IdEtudiant != idEtudiant))
            return null;
        
        string path = Directory.GetCurrentDirectory() + "/soumissions" + "/devoir-" + equipe.IdDevoir + "/equipe-" + equipe.Id + "/";
        var filePath = Path.Combine(path, correction ? soumission.NomFichierCorrection : soumission.NomFichier);
        
        if (!File.Exists(filePath))
        {
            if(!correction)
                await Delete(soumission.Id, idEtudiant);
            return null;
        }

        var fileBytes = File.ReadAllBytes(filePath);
        var contentType = "application/octet-stream"; 

        return (fileBytes, contentType, correction ? soumission.NomFichierCorrection : soumission.NomFichier);
    }
    
    public async Task UploadFichiers(int idDevoir, int idEtudiant, List<IFormFile> fichiers)
    {
        var equipe = await _devoirService.GetEquipeOfCurrentEtudiantPourDevoir(idDevoir, idEtudiant);
        
        string path = Directory.GetCurrentDirectory() + "/soumissions" + "/devoir-" + idDevoir + "/equipe-" + equipe.Id + "/";
        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        foreach (var fichier in fichiers)
        {
            if (fichier.Length > 0)
            {
                var filePath = Path.Combine(path, fichier.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fichier.CopyToAsync(stream);
                }

                await Add(new Entities.Soumission()
                {
                    IdEquipe = equipe.Id,
                    NomFichier = fichier.FileName,
                    TempsSoumission = Dates.GetTimeNowEst().ToUniversalTime(),
                    Taille = fichier.Length / 1024
                });
            }
        }
        
        if (equipe.EtatSoumission != EtatSoumission.Evalue)
            equipe.EtatSoumission = EtatSoumission.Soumis;
        
        await _repository.Save();
    }
    
    public async Task<Entities.Soumission> Add(Entities.Soumission soumission)
    {
        var addedSoumission = await _repository.Add(soumission);
        await _repository.Save();
        return addedSoumission;
    }
    
    public async Task Update(Entities.Soumission soumission, Entities.Soumission updatedSoumission)
    {
        soumission.NomFichier = updatedSoumission.NomFichier;
        soumission.TempsSoumission = updatedSoumission.TempsSoumission;
        soumission.NomFichierCorrection = updatedSoumission.NomFichierCorrection;
        soumission.Taille = updatedSoumission.Taille;
        
        _repository.Update(soumission);
        await _repository.Save();
    }

    public async Task Delete(int idSoumission, int idEtudiant)
    {
        var soumission = await GetById(idSoumission);
        var equipe = await _equipeService.GetById(soumission.IdEquipe);
        var membres = await _membreService.GetAllMembresByEquipeId(equipe.Id);
        if (membres.All(m => m.IdEtudiant != idEtudiant))
            return;

        string path = Directory.GetCurrentDirectory() + "/soumissions" + "/devoir-" + equipe.IdDevoir + "/equipe-" + equipe.Id + "/";
        var filePath = Path.Combine(path, soumission.NomFichier);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        _repository.Delete(soumission);
        await _repository.Save();

        var soumissions = await _repository.GetAll<Entities.Soumission>();
        soumissions = soumissions.Where(s => s.IdEquipe == equipe.Id);
        if(!soumissions.Any() && equipe.EtatSoumission != EtatSoumission.Evalue)
            equipe.EtatSoumission = EtatSoumission.AFaire;
        
        await _repository.Save();
    }
}