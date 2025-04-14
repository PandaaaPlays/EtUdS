using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class Inscription
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("id_cours")]
    public int IdCours { get; set; }

    [Column("id_etudiant")]
    public int IdEtudiant { get; set; }
}