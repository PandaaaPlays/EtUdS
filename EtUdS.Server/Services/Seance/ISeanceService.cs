using EtUdS.Server.Dtos;

namespace etuds.Server.Services.Seance;

public interface ISeanceService
{
    Task<Entities.Seance> GetById(int id);
    Task<IEnumerable<Entities.Seance>> GetAll();
    public Task<IEnumerable<SeanceDetailsCoursDTO>> GetSeanceAvecDetailsCours(int idEtudiant, DateTime? startDate, DateTime? endDate);
    Task<Entities.Seance> Add(Entities.Seance seance);
    Task<IEnumerable<Entities.Seance>> AddSerie(Entities.Seance seance, DateTime debutDate, DateTime endDate);
    Task Update(Entities.Seance seance, Entities.Seance updatedSeance);
    Task Delete(Entities.Seance seance);
}