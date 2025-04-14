using etuds.Server.Entities;

namespace EtUdS.Server.Dtos;

public class SeanceDetailsCoursDTO
{
    public int IdCours { get; set; }
    public string Sigle { get; set; }
    public string Titre { get; set; }
    public DateTime DebutSeance { get; set; }
    public DateTime FinSeance { get; set; }
    public string Local { get; set; }
}