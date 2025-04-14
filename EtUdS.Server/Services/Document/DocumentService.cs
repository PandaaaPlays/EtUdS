using EtUdS.Server.Dtos;
using EtUdS.Server.Repositories;
using etuds.Server.Services.Devoir;
using etuds.Server.Services.Equipe;
using etuds.Server.Services.Membre;

namespace etuds.Server.Services.Document;

public class DocumentService: IDocumentService
{
    private readonly IBaseRepository _repository;
    private readonly IDevoirService _devoirService;
    private readonly IMembreService _membreService;
    private readonly IEquipeService _equipeService;

    public DocumentService(IBaseRepository repository, IDevoirService devoirService, IMembreService membreService, IEquipeService equipeService)
    {
        _repository = repository;
        _devoirService = devoirService;
        _membreService = membreService;
        _equipeService = equipeService;
    }

    public async Task UploadFichiers(int idDevoir, int idEtudiant, List<IFormFile> fichiers)
    {
        var equipe = await _devoirService.GetEquipeOfCurrentEtudiantPourDevoir(idDevoir, idEtudiant);
        var membres = await _membreService.GetAllMembresByEquipeId(equipe.Id);
        
        var membreEtudiant = membres.First(m => m.IdEtudiant == idEtudiant);

        string path = Directory.GetCurrentDirectory() + "/devoirs" + "/devoir-" + idDevoir + "/equipe-" + equipe.Id + "/";
        
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

                await Add(new Entities.Document()
                {
                    IdEquipe = equipe.Id, 
                    IdMembre = membreEtudiant.Id, 
                    NomFichier = fichier.FileName,
                    Taille = fichier.Length / 1024
                });
            }
        }
    }

    public async Task<(byte[], string, string)?> DownloadById(int id, int idEtudiant)
    {
        var document = await GetById(id);
        var equipe = await _equipeService.GetById(document.IdEquipe);
        var membres = await _membreService.GetAllMembresByEquipeId(equipe.Id);
        if (membres.All(m => m.IdEtudiant != idEtudiant))
            return null;
        
        string path = Directory.GetCurrentDirectory() + "/devoirs" + "/devoir-" + equipe.IdDevoir + "/equipe-" + equipe.Id + "/";
        var filePath = Path.Combine(path, document.NomFichier);
        
        if (!File.Exists(filePath))
        {
            await Delete(document.Id, idEtudiant);
            return null;
        }

        var fileBytes = File.ReadAllBytes(filePath);
        var contentType = "application/octet-stream"; 

        return (fileBytes, contentType, document.NomFichier);
    }
    
    public async Task<Entities.Document> GetById(int id)
    {
        var document = await _repository.GetById<Entities.Document>(id);
        return document;
    }
    
    public async Task<IEnumerable<DocumentDetailsDTO>> GetByEquipeDevoir(int idDevoir, int idEtudiant)
    {
        var equipe = await _devoirService.GetEquipeOfCurrentEtudiantPourDevoir(idDevoir, idEtudiant);
        var documents = await GetAll();
        documents = documents.Where(x => x.IdEquipe == equipe.Id);

        var documentsDetails = new List<DocumentDetailsDTO>();
        foreach (var document in documents)
        {
            var membre = await _membreService.GetById(document.IdMembre);
            var etudiant = await _repository.GetById<Entities.Etudiant>(membre.IdEtudiant);
            documentsDetails.Add(new DocumentDetailsDTO
            {
                Id = document.Id,
                NomFichier = document.NomFichier,
                IdEtudiant = etudiant.Id,
                Prenom = etudiant.Prenom,
                Nom = etudiant.Nom,
                Taille = document.Taille
            });
        }
        
        return documentsDetails;
    }
    
    public async Task<IEnumerable<Entities.Document>> GetAll()
    {
        var documents = await _repository.GetAll<Entities.Document>();
        return documents;
    }

    
    public async Task<Entities.Document> Add(Entities.Document document)
    {
        var addedDocument = await _repository.Add(document);
        await _repository.Save();
        return addedDocument;
    }
    
    public async Task Update(Entities.Document document, Entities.Document updatedDocument)
    {
        document.IdEquipe = updatedDocument.IdEquipe;
        document.IdMembre = updatedDocument.IdMembre;
        document.NomFichier = updatedDocument.NomFichier;
        document.Taille = updatedDocument.Taille;
        
        _repository.Update(document);
        await _repository.Save();
    }

    public async Task Delete(int idDocument, int idEtudiant)
    {
        var document = await GetById(idDocument);
        var membre = await _membreService.GetById(document.IdMembre);
        if (membre.IdEtudiant != idEtudiant)
            return;
        
        var equipe = await _equipeService.GetById(document.IdEquipe);
        string path = Directory.GetCurrentDirectory() + "/devoirs" + "/devoir-" + equipe.IdDevoir + "/equipe-" + equipe.Id + "/";
        var filePath = Path.Combine(path, document.NomFichier);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        _repository.Delete(document);
        await _repository.Save();
    }
}