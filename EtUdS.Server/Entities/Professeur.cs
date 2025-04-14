using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class Professeur
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("nom")]
    public string Nom { get; set; }

    [Column("prenom")]
    public string Prenom { get; set; }
    
    [Column("courriel")]
    public string Courriel { get; set; }
    
}