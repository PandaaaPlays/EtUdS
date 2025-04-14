using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class MessageNonLu
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("id_message")] 
    public int IdMessage { get; set; }
    
    [Column("id_etudiant")] 
    public int IdEtudiant { get; set; }
        
    [Column("rappel_envoye")] 
    public bool RappelEnvoye { get; set; }
}