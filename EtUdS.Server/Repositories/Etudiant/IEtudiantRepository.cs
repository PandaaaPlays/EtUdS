using EtUdS.Server.Repositories;

namespace etuds.Server.Repositories.Etudiant;

public interface IEtudiantRepository : IBaseRepository
{
    Task<Entities.Etudiant> GetByCourriel(string courriel);
}