using EtUdS.Server.Repositories;
using etuds.Server.Services.Cours;
using EtUdS.Server.Utils;

namespace etuds.Server.Services.NoteDeCours;

public class NoteDeCoursService: INoteDeCoursService
{
    private readonly IBaseRepository _repository;
    private readonly ICoursService _coursService;
    
    public NoteDeCoursService(IBaseRepository repository, ICoursService coursService)
    {
        _repository = repository;
        _coursService = coursService;
    }
    
    public async Task UploadFichiers(int idCours, List<IFormFile> fichiers)
    {

        string path = Directory.GetCurrentDirectory() + "/cours" + "/cours-" + idCours + "/";
        
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

                await Add(new Entities.NoteDeCours()
                {
                    IdCours = idCours,
                    NomFichier = fichier.FileName, 
                    Date = Dates.GetTimeNowEst().ToUniversalTime(),
                    Taille = fichier.Length / 1024
                });
            }
        }
    }
    
    public async Task<Entities.NoteDeCours> Add(Entities.NoteDeCours noteDeCours)
    {
        var addedDocument = await _repository.Add(noteDeCours);
        await _repository.Save();
        return addedDocument;
    }
    
    public async Task<IEnumerable<Entities.NoteDeCours>> GetAll()
    {
        var notedeCours = await _repository.GetAll<Entities.NoteDeCours>();
        return notedeCours;
    }
    
    public async Task<IEnumerable<Entities.NoteDeCours>> GetByCours(int idCours)
    {
        var noteDeCours = await GetAll();
        noteDeCours = noteDeCours.Where(x => x.IdCours == idCours);
        
        return noteDeCours;
    }
    
    public async Task<Entities.NoteDeCours> GetById(int id)
    {
        var noteDeCours = await _repository.GetById<Entities.NoteDeCours>(id);
        return noteDeCours;
    }

    public async Task<(byte[], string, string)?> DownloadById(int idCours, int idNoteDeCours)
    {
        var noteDeCours = await GetById(idNoteDeCours);
        if (noteDeCours == null)
            return null;

        var cours = await _coursService.GetById(noteDeCours.IdCours);
        if (cours == null || cours.Id != idCours)
            return null;
        
        string path = Directory.GetCurrentDirectory() + "/cours" + "/cours-" + idCours  + "/";
        var filePath = Path.Combine(path, noteDeCours.NomFichier);
        
        if (!File.Exists(filePath))
        {
            await Delete(noteDeCours.Id, idCours);
            return null;
        }

        var fileBytes = File.ReadAllBytes(filePath);
        var contentType = "application/octet-stream"; 

        return (fileBytes, contentType, noteDeCours.NomFichier);
    }
    
    public async Task Delete(int idCours, int idNoteDeCours)
    {
        var noteDeCours = await GetById(idNoteDeCours);
        var cours = await _coursService.GetById(noteDeCours.IdCours);
        if (cours.Id != idCours)
            return;
        
        string path = Directory.GetCurrentDirectory() + "/cours" + "/cours-" + idCours + "/";
        var filePath = Path.Combine(path, noteDeCours.NomFichier);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        
        _repository.Delete(noteDeCours);
        await _repository.Save();
    }
}