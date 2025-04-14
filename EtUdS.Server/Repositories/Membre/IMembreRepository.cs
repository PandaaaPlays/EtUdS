namespace EtUdS.Server.Repositories.Membre;

public interface IMembreRepository : IBaseRepository
{
    Task<IEnumerable<etuds.Server.Entities.Membre>> GetAllMembresByEquipeId(int equipeId);
    Task<IEnumerable<int>> GetIdEtudiantsNonDisponiblesPourDevoir(int devoirId);
}