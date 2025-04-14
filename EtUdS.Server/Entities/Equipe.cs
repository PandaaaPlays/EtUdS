using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public enum EtatSoumission
{
    AFaire = 1,
    EnRetard = 2,
    Soumis = 3,
    Evalue = 4
}

public class Equipe
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("id_devoir")]
    public int IdDevoir { get; set; }
    
    [Column("etat_soumission")]
    public EtatSoumission EtatSoumission { get; set; }
    
    [Column("note")]
    public decimal? Note { get; set; }
}