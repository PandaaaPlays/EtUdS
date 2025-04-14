using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class Membre
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("id_etudiant")]
    public int IdEtudiant { get; set; }

    [Column("id_equipe")]
    public int IdEquipe { get; set; }
    
    [Column("accepte")]
    public bool Accepte { get; set; }
}