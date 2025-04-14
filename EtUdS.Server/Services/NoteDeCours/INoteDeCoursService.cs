namespace etuds.Server.Services.NoteDeCours;

public interface INoteDeCoursService
{
    Task UploadFichiers(int idCours, List<IFormFile> fichiers);
    Task<IEnumerable<Entities.NoteDeCours>> GetByCours(int idCours);
    Task<Entities.NoteDeCours> Add(Entities.NoteDeCours noteDeCours);
    Task<IEnumerable<Entities.NoteDeCours>> GetAll();
    Task<Entities.NoteDeCours> GetById(int id);
    Task<(byte[], string, string)?> DownloadById(int idCours, int idNoteDeCours);
    Task Delete(int idCours, int idNoteDeCours);
}