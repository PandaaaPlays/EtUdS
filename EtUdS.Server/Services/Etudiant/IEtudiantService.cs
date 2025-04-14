using EtUdS.Server.Dtos;

namespace etuds.Server.Services.Etudiant;

public interface IEtudiantService
{
    Task<EtudiantDTO> GetById(int id);
    Task<Entities.Etudiant> GetByIdWithPassword(int id);
    Task<Entities.Etudiant> GetByCourriel(string courriel);
    Task<IEnumerable<EtudiantDTO>> GetAll();
    Task<Entities.Etudiant> Add(Entities.Etudiant etudiant);
    Task Update(Entities.Etudiant etudiant, Entities.Etudiant updatedEtudiant);
    Task Delete(Entities.Etudiant etudiant);
}