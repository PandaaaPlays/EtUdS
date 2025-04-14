using EtUdS.Server.Repositories;

namespace etuds.Server.Services.Cours;

public class CoursService : ICoursService
{
    private readonly IBaseRepository _repository;

    public CoursService(IBaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Entities.Cours> GetById(int id)
    {
        return await _repository.GetById<Entities.Cours>(id);
    }

    public async Task<IEnumerable<Entities.Cours>> GetAll()
    {
        return await _repository.GetAll<Entities.Cours>();
    }
    
    public async Task<IEnumerable<Entities.Cours>> GetAllCoursEtudiant(int idEtudiant)
    {
        var inscriptions = await _repository.GetAll<Entities.Inscription>();
        var coursInscrit = new List<Entities.Cours>();
        
        foreach(var inscription in inscriptions)
        {
            if (inscription.IdEtudiant == idEtudiant)
            {
                var cours = await _repository.GetById<Entities.Cours>(inscription.IdCours);
                coursInscrit.Add(cours);
            }
        }

        return coursInscrit;
    }
    
    public async Task<Entities.Cours> Add(Entities.Cours cours)
    {
        var addedCours = await _repository.Add(cours);
        await _repository.Save();
        return addedCours;
    }
    
    public async Task Update(Entities.Cours cours, Entities.Cours updatedCours)
    {
        cours.IdProfesseur = updatedCours.IdProfesseur;
        cours.Sigle = updatedCours.Sigle;
        cours.Titre = updatedCours.Titre;
        
        _repository.Update(cours);
        await _repository.Save();
    }

    public async Task Delete(Entities.Cours cours)
    {
        _repository.Delete(cours);
        await _repository.Save();
    }
}