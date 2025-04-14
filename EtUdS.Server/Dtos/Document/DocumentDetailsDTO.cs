using etuds.Server.Entities;

namespace EtUdS.Server.Dtos;

public class DocumentDetailsDTO
{
    public int Id { get; set; }
    public string NomFichier { get; set; }
    public int IdEtudiant { get; set; }
    public string Prenom { get; set; }
    public string Nom { get; set; }
    public long Taille { get; set; }
}