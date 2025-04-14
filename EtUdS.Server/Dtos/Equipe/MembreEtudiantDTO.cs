namespace EtUdS.Server.Dtos;

public class MembreEtudiantDTO
{
    public int MembreId { get; set; }
    public int IdEtudiant { get; set; }
    public string Prenom { get; set; }
    public string Nom { get; set; }
    public bool Accepte { get; set; }
}