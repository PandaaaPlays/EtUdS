using EtUdS.Server.Dtos;

namespace etuds.Server.Services.Devoir;

public interface IDevoirService
{
    Task<Entities.Devoir> GetById(int id);
    Task<DevoirDetailsDTO> GetByIdAvecDetails(int id, int idEtudiant);
    Task<IEnumerable<Entities.Devoir>> GetAll();
    Task<IEnumerable<EtudiantDTO>> GetEtudiantsDisponiblePourDevoir(int id);
    Task<Entities.Equipe?> GetEquipeOfCurrentEtudiantPourDevoir(int devoirId, int etudiantId);
    Task<IEnumerable<InvitationDTO>> GetInvitationsOfCurrentEtudiantPourDevoir(int devoirId, int etudiantId);
    Task<IEnumerable<DevoirCardDTO>> GetDevoirAvecDetailsCours(int idEtudiant, string? cours, string? etat, DateTime? startDate, DateTime? endDate);
    Task<IEnumerable<Entities.Membre>> GetMembresIdEtudiant(int idEtudiant, int idDevoir);
    Task<Entities.Devoir> Add(Entities.Devoir devoir);
    Task Update(Entities.Devoir devoir, Entities.Devoir updatedDevoir);
    Task Delete(Entities.Devoir devoir);
}