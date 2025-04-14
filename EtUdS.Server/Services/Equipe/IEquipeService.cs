using EtUdS.Server.Dtos;

namespace etuds.Server.Services.Equipe;

public interface IEquipeService
{
    Task<Entities.Equipe> GetById(int id);
    Task<EquipeDetailsDTO> GetDetailsMembres(int id);
    Task<IEnumerable<Entities.Equipe>> GetAll();
    Task<Entities.Equipe> Add(Entities.Equipe equipe);
    Task Update(Entities.Equipe equipe, Entities.Equipe updatedEquipe);
    Task Delete(Entities.Equipe equipe);
}