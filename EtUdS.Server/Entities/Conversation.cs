using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class Conversation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("titre")] 
    public string Titre { get; set; }
}