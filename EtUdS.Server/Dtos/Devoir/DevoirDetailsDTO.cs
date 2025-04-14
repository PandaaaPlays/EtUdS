using etuds.Server.Entities;

namespace EtUdS.Server.Dtos;

public class DevoirDetailsDTO
{
    public int Id { get; set; }
    public string Sigle { get; set; }
    public string Nom { get; set; }
    public EtatSoumission? Etat { get; set; }
    public DateTime DateLimiteSoumission { get; set; }
    public decimal? Note { get; set; }
    public int TailleMaxEquipes { get; set; }
}