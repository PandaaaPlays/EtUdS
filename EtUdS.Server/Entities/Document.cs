using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class Document
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("id_equipe")]
    public int IdEquipe { get; set; }

    [Column("id_membre")]
    public int IdMembre { get; set; }
    
    [Column("taille")]
    public long Taille { get; set; }
    
    [Column("nom_fichier")]
    public string NomFichier { get; set; }
}