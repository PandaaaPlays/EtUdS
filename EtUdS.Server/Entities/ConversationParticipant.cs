using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace etuds.Server.Entities;

public class ConversationParticipant
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("id_conversation")] 
    public int IdConversation { get; set; }
    
    [Column("id_etudiant")] 
    public int IdEtudiant { get; set; }
}