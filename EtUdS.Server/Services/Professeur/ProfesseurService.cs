using EtUdS.Server.Repositories;

namespace etuds.Server.Services.Professeur;

public class ProfesseurService: IProfesseurService
{
    
    private readonly IBaseRepository _repository;

    public ProfesseurService(IBaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Entities.Professeur> GetById(int id)
    {
        return await _repository.GetById<Entities.Professeur>(id);
    }
    
}