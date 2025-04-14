using etuds.Server.Entities;

namespace EtUdS.Server.Dtos;

public class MembreDetailsDTO
{
    public int MembreId { get; set; }
    public bool Accepte { get; set; }
    public int EtudiantId { get; set; }
    public string Prenom { get; set; }
    public string Nom { get; set; }
    public string Courriel { get; set; }
}