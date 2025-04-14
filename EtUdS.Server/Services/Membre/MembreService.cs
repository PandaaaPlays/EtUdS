using EtUdS.Server.Repositories;
using EtUdS.Server.Repositories.Equipe;
using etuds.Server.Repositories.Etudiant;
using EtUdS.Server.Repositories.Membre;
using etuds.Server.Services.Devoir;
using etuds.Server.Services.Document;
using etuds.Server.Services.Equipe;

namespace etuds.Server.Services.Membre;

public class MembreService : IMembreService
{
    private readonly IMembreRepository _repository;
    private readonly IBaseRepository _documentRepository;
    private readonly IEquipeService _equipeService;
    private readonly IDevoirService _devoirService;

    public MembreService(IMembreRepository repository, IEquipeService equipeService, IDevoirService devoirService, IBaseRepository documentRepository)
    {
        _repository = repository;
        _equipeService = equipeService;
        _devoirService = devoirService;
        _documentRepository = documentRepository;
    }

    public async Task<Entities.Membre> GetById(int id)
    {
        return await _repository.GetById<Entities.Membre>(id);
    }

    public async Task<IEnumerable<Entities.Membre>> GetAll()
    {
        return await _repository.GetAll<Entities.Membre>();
    }

    public async Task<IEnumerable<Entities.Membre>> GetAllMembresByEquipeId(int equipeId)
    {
        return await _repository.GetAllMembresByEquipeId(equipeId);
    }
    
    public async Task<IEnumerable<int>> GetIdEtudiantsNonDisponiblesPourDevoir(int devoirId)
    {
        return await _repository.GetIdEtudiantsNonDisponiblesPourDevoir(devoirId);
    }
    
    public async Task AccepterInvitation(int equipeId, int etudiantId)
    {
        var equipeInvite = await _equipeService.GetById(equipeId);
        var membres = await GetAll();
        foreach (var membre in membres)
        {
            if (membre.IdEtudiant == etudiantId)
            {
                var equipe = await _equipeService.GetById(membre.IdEquipe);
                if(equipe.IdDevoir == equipeInvite.IdDevoir && membre.Accepte)
                    await Delete(membre);
            }
        }
        
        var membresEquipe = await GetAllMembresByEquipeId(equipeId);
        foreach (var membre in membresEquipe)
        {
            if (membre.IdEtudiant == etudiantId)
            {
                var newMembre = membre;
                newMembre.Accepte = true;
                await Update(membre, newMembre);
            }
        }
    }
    
    public async Task RefuserInvitation(int equipeId, int etudiantId)
    {
        var membresEquipe = await GetAllMembresByEquipeId(equipeId);
        
        foreach (var membre in membresEquipe)
        {
            if (membre.IdEtudiant == etudiantId)
            {
                await Delete(membre);
            }
        }
    }
    
    public async Task<Entities.Membre> Add(Entities.Membre membre)
    {
        var addedMembre = await _repository.Add(membre);
        await _repository.Save();
        return addedMembre;
    }
    
    public async Task Update(Entities.Membre membre, Entities.Membre updatedMembre)
    {
        membre.IdEquipe = updatedMembre.IdEquipe;
        membre.IdEtudiant = updatedMembre.IdEtudiant;
        membre.Accepte = updatedMembre.Accepte;
        
        _repository.Update(membre);
        await _repository.Save();
    }

    public async Task Delete(Entities.Membre membre)
    {
        await SupprimerDocuments(membre);
        
        _repository.Delete(membre);
        await _repository.Save();
        
        var remainingMembres = await _repository.GetAllMembresByEquipeId(membre.IdEquipe);
        bool hasAcceptedMembers = remainingMembres.Any(m => m.Accepte);

        if (!hasAcceptedMembers)
        {
            var equipe = await _equipeService.GetById(membre.IdEquipe);
            foreach (var remainingMembre in remainingMembres)
            {
                await SupprimerDocuments(remainingMembre);
                _repository.Delete(remainingMembre);
            }
            await _repository.Save();
            await _equipeService.Delete(equipe);
        }
        else if (remainingMembres.Count() == 1)
        {
            await SupprimerDocuments(remainingMembres.FirstOrDefault());
            _repository.Delete(remainingMembres.FirstOrDefault());
            await _repository.Save();
            var equipe = await _equipeService.GetById(membre.IdEquipe);
            await _equipeService.Delete(equipe);
        }
    }
    
    public async Task Delete(int idDevoir, int idEtudiant)
    {
        var equipe = await _devoirService.GetEquipeOfCurrentEtudiantPourDevoir(idDevoir, idEtudiant);
        var membres = await GetAllMembresByEquipeId(equipe.Id);
        var membre = membres.Where(m => m.IdEtudiant == idEtudiant).First();
        await Delete(membre);
    }

    private async Task SupprimerDocuments(Entities.Membre membre)
    {
        var documents = await _documentRepository.GetAll<Entities.Document>();
        documents = documents.Where(d => d.IdMembre == membre.Id);
        foreach (var document in documents)
        {
            _documentRepository.Delete(document);
        }
        await _documentRepository.Save();
    }

}