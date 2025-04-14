namespace etuds.Server.Services.Professeur;

public interface IProfesseurService
{
    Task<Entities.Professeur> GetById(int id);
}