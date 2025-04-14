namespace etuds.Server.Services.Cours;

public interface ICoursService
{
    Task<Entities.Cours> GetById(int id);
    Task<IEnumerable<Entities.Cours>> GetAll();
    Task<IEnumerable<Entities.Cours>> GetAllCoursEtudiant(int idEtudiant);
    Task<Entities.Cours> Add(Entities.Cours cours);
    Task Update(Entities.Cours cours, Entities.Cours updatedCours);
    Task Delete(Entities.Cours cours);
}