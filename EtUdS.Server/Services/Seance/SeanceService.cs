using EtUdS.Server.Dtos;
using etuds.Server.Entities;
using EtUdS.Server.Repositories;

namespace etuds.Server.Services.Seance;

public class SeanceService : ISeanceService
{
    private readonly IBaseRepository _repository;

    public SeanceService(IBaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Entities.Seance> GetById(int id)
    {
        return await _repository.GetById<Entities.Seance>(id);
    }

    public async Task<IEnumerable<Entities.Seance>> GetAll()
    {
        return await _repository.GetAll<Entities.Seance>();
    }

    public async Task<IEnumerable<SeanceDetailsCoursDTO>> GetSeanceAvecDetailsCours(
        int idEtudiant,
        DateTime? startDate = null,
        DateTime? endDate = null
    )
    {
        var inscriptions = await _repository.GetAll<Inscription>();
        var coursIds = inscriptions
            .Where(e => e.IdEtudiant == idEtudiant)
            .Select(e => e.IdCours)
            .ToList();

        var cours = await _repository.GetAll<Entities.Cours>();
        
        var seances = await GetAll();
        seances = seances.Where(s => coursIds.Contains(s.IdCours));

        var filteredSeances = seances.AsQueryable();
        
        // Filtre par date (entre date de début et date de fin) 
        if (startDate.HasValue && endDate.HasValue)
        {
            // Récuperer la fin du jour plutot que le début... 
            endDate = endDate.Value.Date.AddDays(1).AddMilliseconds(-1);
            filteredSeances = filteredSeances.Where(s => 
                s.DebutSeance >= startDate.Value && s.FinSeance <= endDate.Value
            );
        }

        var seanceList = filteredSeances.ToList();

        return seanceList.Select(s => 
        {
            var coursSeance = cours.FirstOrDefault(c => c.Id == s.IdCours);
            return new SeanceDetailsCoursDTO
            {
                IdCours = coursSeance != null ? coursSeance.Id : 0,
                Sigle = coursSeance != null ? coursSeance.Sigle : "N/A",
                Titre = coursSeance != null ? coursSeance.Titre : "N/A",
                DebutSeance = s.DebutSeance,
                FinSeance = s.FinSeance,
                Local = s.Local
            };
        }).ToList();
    }

    public async Task<Entities.Seance> Add(Entities.Seance seance)
    {
        var addedSeance = await _repository.Add(seance);
        await _repository.Save();
        return addedSeance;
    }
    
    public async Task<IEnumerable<Entities.Seance>> AddSerie(Entities.Seance seance, DateTime startDate, DateTime endDate)
    {
        var addedSeances = new List<Entities.Seance>();
        var referenceDayOfWeek = seance.DebutSeance.DayOfWeek;
        var referenceStartTime = seance.DebutSeance.TimeOfDay;
        var referenceEndTime = seance.FinSeance.TimeOfDay;
        
        DateTime currentStartDate = startDate.Date
            .AddDays((7 + referenceDayOfWeek - startDate.DayOfWeek) % 7)
            .Add(referenceStartTime)
            .ToUniversalTime();
    
        DateTime currentEndDate = currentStartDate.Date
            .Add(referenceEndTime)
            .ToUniversalTime();

        while (currentEndDate <= endDate)
        {
            var newSeance = new Entities.Seance
            {
                DebutSeance = currentStartDate,
                FinSeance = currentEndDate,
                IdCours = seance.IdCours,
                Laboratoire = seance.Laboratoire,
                Local = seance.Local,
            };

            addedSeances.Add(await _repository.Add(newSeance));
            currentStartDate = currentStartDate.AddDays(7).ToUniversalTime();
            currentEndDate = currentEndDate.AddDays(7).ToUniversalTime(); // Passer à la semaine suivante
        } 
        
        await _repository.Save();
        return addedSeances;
    }
    
    public async Task Update(Entities.Seance seance, Entities.Seance updatedSeance)
    {
        seance.IdCours = updatedSeance.IdCours;
        seance.DebutSeance = updatedSeance.DebutSeance;
        seance.FinSeance = updatedSeance.FinSeance;
        seance.Local = updatedSeance.Local;
        seance.Laboratoire = updatedSeance.Laboratoire;
        
        _repository.Update(seance);
        await _repository.Save();
    }

    public async Task Delete(Entities.Seance seance)
    {
        _repository.Delete(seance);
        await _repository.Save();
    }
}