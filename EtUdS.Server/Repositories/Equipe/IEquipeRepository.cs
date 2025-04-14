namespace EtUdS.Server.Repositories.Equipe;

public interface IEquipeRepository : IBaseRepository
{
    Task<etuds.Server.Entities.Equipe> GetByMembreId(int membreId);
}