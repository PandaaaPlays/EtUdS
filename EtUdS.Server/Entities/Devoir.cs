using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class Devoir
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("id_cours")]
    public int IdCours { get; set; }

    [Column("date_limite_soumission")]
    public DateTime DateLimiteSoumission { get; set; }
    
    [Column("nom")]
    public string Nom { get; set; }
    
    [Column("taille_max_equipes")]
    public int TailleMaxEquipes { get; set; }
}