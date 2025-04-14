using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class Soumission
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("id_equipe")]
    public int IdEquipe { get; set; }

    [Column("temps_soumission")]
    public DateTime TempsSoumission { get; set; }
    
    [Column("taille")]
    public long Taille { get; set; }
    
    [Column("nom_fichier")]
    public string NomFichier { get; set; }
    
    [Column("nom_fichier_correction")]
    public string? NomFichierCorrection { get; set; }
}