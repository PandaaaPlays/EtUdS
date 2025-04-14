namespace etuds.Server.Services.Membre;

public interface IMembreService
{
    Task<Entities.Membre> GetById(int id);
    Task<IEnumerable<Entities.Membre>> GetAll();
    Task<IEnumerable<Entities.Membre>> GetAllMembresByEquipeId(int equipeId);
    Task<IEnumerable<int>> GetIdEtudiantsNonDisponiblesPourDevoir(int devoirId);
    Task AccepterInvitation(int equipeId, int etudiantId);
    Task RefuserInvitation(int equipeId, int etudiantId);
    Task<Entities.Membre> Add(Entities.Membre membre);
    Task Update(Entities.Membre membre, Entities.Membre updatedMembre);
    Task Delete(Entities.Membre membre);
    Task Delete(int idDevoir, int idEtudiant);
}