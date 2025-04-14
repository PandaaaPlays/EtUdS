namespace etuds.Server.Services.Soumission;

public interface ISoumissionService
{
    Task<Entities.Soumission> GetById(int id);
    Task<IEnumerable<Entities.Soumission>> GetAll();
    Task<IEnumerable<Entities.Soumission>> GetByEquipeDevoir(int idDevoir, int idEtudiant);
    Task<(byte[], string, string)?> DownloadById(int id, int idEtudiant, bool correction);
    Task UploadFichiers(int idDevoir, int idEtudiant, List<IFormFile> fichiers);
    Task<Entities.Soumission> Add(Entities.Soumission soumission);
    Task Update(Entities.Soumission soumission, Entities.Soumission updatedSoumission);
    Task Delete(int idSoumission, int idEtudiant);
}