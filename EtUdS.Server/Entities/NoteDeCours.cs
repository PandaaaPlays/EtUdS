using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class NoteDeCours
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("id_cours")]
    public int IdCours { get; set; }
    
    [Column("date")]
    public DateTime Date { get; set; }
    [Column("taille")]
    public long Taille { get; set; }
    
    [Column("nom_fichier")]
    public string NomFichier { get; set; }
}