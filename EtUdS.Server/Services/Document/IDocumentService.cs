using EtUdS.Server.Dtos;

namespace etuds.Server.Services.Document;

public interface IDocumentService
{
    Task<Entities.Document> GetById(int id);
    Task<(byte[], string, string)?> DownloadById(int id, int idEtudiant);
    Task UploadFichiers(int idDevoir, int idEtudiant, List<IFormFile> fichiers);
    Task<IEnumerable<DocumentDetailsDTO>> GetByEquipeDevoir(int idDevoir, int idEtudiant);
    Task<IEnumerable<Entities.Document>> GetAll();
    Task<Entities.Document> Add(Entities.Document document);
    Task Update(Entities.Document document, Entities.Document updatedDocument);
    Task Delete(int idDocument, int idEtudiant);
}