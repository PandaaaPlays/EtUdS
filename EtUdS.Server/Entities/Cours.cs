using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class Cours
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("id_professeur")]
    public int IdProfesseur { get; set; }

    [Column("sigle")]
    public string Sigle { get; set; }

    [Column("titre")]
    public string Titre { get; set; }
}