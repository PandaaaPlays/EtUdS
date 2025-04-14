using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class Seance
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("id_cours")]
    public int IdCours { get; set; }

    [Column("debut")]
    public DateTime DebutSeance { get; set; }
    
    [Column("fin")]
    public DateTime FinSeance { get; set; }
    
    [Column("local")]
    public string Local { get; set; }

    [Column("laboratoire")]
    public bool Laboratoire { get; set; }
}