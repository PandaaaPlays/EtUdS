using EtUdS.Server.Dtos;
using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using EtUdS.Server.Repositories.Equipe;
using etuds.Server.Repositories.Etudiant;
using EtUdS.Server.Repositories.Membre;
using EtUdS.Server.Utils;

namespace etuds.Server.Services.Devoir;

public class DevoirService : IDevoirService
{
    private readonly IBaseRepository _repository;
    private readonly IEtudiantRepository _etudiantRepository;
    private readonly IMembreRepository _membreRepository;
    private readonly IEquipeRepository _equipeRepository;
    
    public DevoirService(
        IBaseRepository repository, 
        IEtudiantRepository etudiantRepository, 
        IMembreRepository membreRepository,
        IEquipeRepository equipeRepository
        )
    {
        _repository = repository;
        _etudiantRepository = etudiantRepository;
        _membreRepository = membreRepository;
        _equipeRepository = equipeRepository;
    }

    public async Task<Entities.Devoir> GetById(int id)
    {
        var devoir = await _repository.GetById<Entities.Devoir>(id);
        if (devoir != null)
        {
            await UpdateEtat(devoir);
        }
        return devoir;
    }
    
    public async Task<DevoirDetailsDTO> GetByIdAvecDetails(int id, int idEtudiant)
    {
        var devoir = await _repository.GetById<Entities.Devoir>(id);
        if (devoir != null)
        {
            await UpdateEtat(devoir);
        }
        
        var equipe = await GetEquipeOfCurrentEtudiantPourDevoir(devoir.Id, idEtudiant);
        var cours = await _repository.GetById<Entities.Cours>(devoir.IdCours);
        var note = equipe != null ? equipe.Note : null;
        EtatSoumission? etatSoumission = equipe != null ? equipe.EtatSoumission : null;
            
        return new DevoirDetailsDTO
        {
            Id = devoir.Id,
            Sigle = cours.Sigle,
            Nom = devoir.Nom,
            Etat = etatSoumission,
            DateLimiteSoumission = devoir.DateLimiteSoumission,
            Note = note,
            TailleMaxEquipes = devoir.TailleMaxEquipes
        };
    }

    public async Task<IEnumerable<Entities.Devoir>> GetAll()
    {
        var devoirs = await _repository.GetAll<Entities.Devoir>();
        foreach (var devoir in devoirs)
        {
            await UpdateEtat(devoir);
        }
        return devoirs;
    }

    public async Task<IEnumerable<EtudiantDTO>> GetEtudiantsDisponiblePourDevoir(int idDevoir)
    {
        var devoir = await GetById(idDevoir);
        var inscriptions = await _repository.GetAll<Inscription>();
        var idEtudiantsDansCours = inscriptions
            .Where(i => i.IdCours == devoir.IdCours)
            .Select(i => i.IdEtudiant);
        var etudiants = idEtudiantsDansCours
            .Select(async id => await _etudiantRepository.GetById<Entities.Etudiant>(id));
        
        var idEtudiantNonDisponibles = await _membreRepository.GetIdEtudiantsNonDisponiblesPourDevoir(idDevoir);
        
        var etudiantsDisponibles = etudiants
            .Where(e => !idEtudiantNonDisponibles.Contains(e.Result.Id))
            .Select(e => new EtudiantDTO()
            {
                Id = e.Result.Id,
                Prenom = e.Result.Prenom,
                Nom = e.Result.Nom
            })
            .ToList();

        return etudiantsDisponibles;
    }

    public async Task<Entities.Equipe?> GetEquipeOfCurrentEtudiantPourDevoir(int devoirId, int etudiantId)
    {
        var equipes = await _equipeRepository.GetAll<Entities.Equipe>();
        var devoir = await _repository.GetById<Entities.Devoir>(devoirId);

        var equipesDevoir = equipes
            .Where(e => e.IdDevoir == devoirId)
            .ToList();

        foreach (var equipe in equipesDevoir)
        {
            var membres = await _membreRepository.GetAllMembresByEquipeId(equipe.Id);
            
            if (membres.Any(m => m.IdEtudiant == etudiantId && m.Accepte))
            {
                if (membres.Count() > 1)
                {
                    var membresEtudiant = await GetMembresIdEtudiant(etudiantId, devoirId);
                    foreach (var membreEtudiant in membresEtudiant)
                    {
                        if(membreEtudiant.Accepte == false)
                            _membreRepository.Delete(membreEtudiant);
                    }
    
                    await _membreRepository.Save();
                }
                UpdateEtat(equipe, devoir.DateLimiteSoumission);
                await _repository.Save();
                return equipe;
            }
        }

        var nouvelleEquipe = new Entities.Equipe
        {
            IdDevoir = devoirId,
            EtatSoumission = EtatSoumission.AFaire,
            Note = null
        };

        var addedEquipe = await _equipeRepository.Add(nouvelleEquipe);
        await _repository.Save();
        
        var membre = new Entities.Membre
        {
            IdEquipe = addedEquipe.Id,
            IdEtudiant = etudiantId,
            Accepte = true
        };

        await _membreRepository.Add(membre);
        await _repository.Save();

        UpdateEtat(addedEquipe, devoir.DateLimiteSoumission);
        await _repository.Save();

        return addedEquipe;
    }
    
    public async Task<IEnumerable<InvitationDTO>> GetInvitationsOfCurrentEtudiantPourDevoir(int devoirId, int etudiantId)
    {
        var equipes = await _equipeRepository.GetAll<Entities.Equipe>();
        equipes = equipes.Where(e => e.IdDevoir == devoirId).ToList();

        List<InvitationDTO> equipeInvite = new List<InvitationDTO>();
        foreach (var equipe in equipes)
        {
            var membresAvecEtudiant = new List<MembreEtudiantDTO>();
            var membres = await _membreRepository.GetAllMembresByEquipeId(equipe.Id);
            
            foreach (var membre in membres)
            {
                var etudiant = await _etudiantRepository.GetById<Entities.Etudiant>(membre.IdEtudiant);
            
                membresAvecEtudiant.Add(new MembreEtudiantDTO
                {
                    MembreId = membre.Id,
                    IdEtudiant = membre.IdEtudiant,
                    Prenom = etudiant.Prenom,
                    Nom = etudiant.Nom, 
                    Accepte = membre.Accepte
                });
            }

            if (membres.Any(m => m.IdEtudiant == etudiantId && m.Accepte != true))
            {
                equipeInvite.Add(new InvitationDTO
                {
                    EquipeId = equipe.Id,
                    Membres = membresAvecEtudiant
                });
            }
        }

        return equipeInvite;
    }
    
    public async Task<IEnumerable<DevoirCardDTO>> GetDevoirAvecDetailsCours(
        int idEtudiant,
        string? cours = null, 
        string? etat = null,
        DateTime? startDate = null,
        DateTime? endDate = null
    )
    {
        // Inscriptions (cours) de l'étudiant
        var inscriptions = await _repository.GetAll<Inscription>();
        var coursIds = inscriptions
            .Where(e => e.IdEtudiant == idEtudiant)
            .Select(e => e.IdCours)
            .ToList();
        
        var coursEtudiant = await _repository.GetAll<Entities.Cours>();
        var coursDict = coursEtudiant
            .Where(c => coursIds.Contains(c.Id))
            .ToDictionary(c => c.Id, c => c.Sigle);
        
        // Devoirs des cours de l'étudiant
        var devoirs = await _repository.GetAll<Entities.Devoir>();
        var devoirsEtudiant = devoirs
            .Where(d => coursIds.Contains(d.IdCours))
            .ToList();

        var filteredDevoirs = devoirsEtudiant.AsQueryable();

        // Filtre par cours (sigle)
        if (!string.IsNullOrEmpty(cours))
        {
            filteredDevoirs = filteredDevoirs.Where(d =>
                coursDict.ContainsKey(d.IdCours) && coursDict[d.IdCours] == cours);
        }
        
        // Filtre par état
        if (!string.IsNullOrEmpty(etat) && Enum.TryParse<EtatSoumission>(etat, true, out var etatEnum))
        {
            var devoirsAvecEtat = new List<Entities.Devoir>();
            foreach (var filteredDevoir in filteredDevoirs)
            {
                var equipe = await GetEquipeOfCurrentEtudiantPourDevoir(filteredDevoir.Id, idEtudiant);
                if (equipe == null || equipe.EtatSoumission != etatEnum)
                    continue;
                devoirsAvecEtat.Add(filteredDevoir);
            }

            filteredDevoirs = devoirsAvecEtat.AsQueryable();
        }

        // Filtre par date (entre date de début et date de fin) 
        if (startDate.HasValue && endDate.HasValue)
        {
            // Récuperer la fin du jour plutot que le début... 
            endDate = endDate.Value.Date.AddDays(1).AddMilliseconds(-1);
            filteredDevoirs = filteredDevoirs.Where(d => 
                d.DateLimiteSoumission >= startDate.Value && d.DateLimiteSoumission <= endDate.Value
            );
        }

        var devoirsList = filteredDevoirs.ToList();

        var result = new List<DevoirCardDTO>();

        foreach (var devoir in devoirsList)
        {
            var equipe = await GetEquipeOfCurrentEtudiantPourDevoir(devoir.Id, idEtudiant);
            var note = equipe != null ? equipe.Note : null;
            EtatSoumission? etatSoumission = equipe != null ? equipe.EtatSoumission : null;
            
            result.Add(new DevoirCardDTO
            {
                Id = devoir.Id,
                Sigle = coursDict.ContainsKey(devoir.IdCours) ? coursDict[devoir.IdCours] : "N/A",
                Nom = devoir.Nom,
                Etat = etatSoumission,
                DateLimiteSoumission = devoir.DateLimiteSoumission,
                Note = note,
                TailleMaxEquipes = devoir.TailleMaxEquipes
            });
        }

        return result;
    }

    public async Task<IEnumerable<Entities.Membre>> GetMembresIdEtudiant(int idEtudiant, int idDevoir)
    {
        var equipes = await _equipeRepository.GetAll<Entities.Equipe>();
        var equipesDevoir = equipes.Where(e => e.IdDevoir == idDevoir);

        var membresEtudiantDevoir = new List<Entities.Membre>();
        
        foreach (var equipe in equipesDevoir)
        {
            var membres = await _membreRepository.GetAllMembresByEquipeId(equipe.Id);

            foreach (var membre in membres)
            {
                if (membre.IdEtudiant == idEtudiant)
                    membresEtudiantDevoir.Add(membre);
            }
        }

        return membresEtudiantDevoir;
    }
    
    public async Task<Entities.Devoir> Add(Entities.Devoir devoir)
    {
        var addedDevoir = await _repository.Add(devoir);
        await _repository.Save();
        return addedDevoir;
    }
    
    public async Task Update(Entities.Devoir devoir, Entities.Devoir updatedDevoir)
    {
        devoir.IdCours = updatedDevoir.IdCours;
        devoir.Nom = updatedDevoir.Nom;
        devoir.DateLimiteSoumission = updatedDevoir.DateLimiteSoumission;
        devoir.TailleMaxEquipes = updatedDevoir.TailleMaxEquipes;
        
        _repository.Update(devoir);
        await _repository.Save();
    }

    public async Task Delete(Entities.Devoir devoir)
    {
        _repository.Delete(devoir);
        await _repository.Save();
    }

    private async Task UpdateEtat(Entities.Devoir devoir)
    {
        var equipes = await _equipeRepository.GetAll<Entities.Equipe>();
        equipes = equipes.Where(e => e.IdDevoir == devoir.Id);

        foreach (var equipe in equipes)
        {
            UpdateEtat(equipe, devoir.DateLimiteSoumission); 
        }
        await _repository.Save();
    }
    
    public void UpdateEtat(Entities.Equipe equipe, DateTime dateLimite)
    {
        if (equipe.EtatSoumission == EtatSoumission.Evalue)
            return;
    
        if (equipe.Note != null)
            equipe.EtatSoumission = EtatSoumission.Evalue;
        else if (equipe.EtatSoumission == EtatSoumission.AFaire && dateLimite < Dates.GetTimeNowEst())
            equipe.EtatSoumission = EtatSoumission.EnRetard;
    }
}