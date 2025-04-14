namespace EtUdS.Server.Dtos;

public class ConversationDetailsDTO
{
    public int Id { get; set; }
    public string? Prenom { get; set; }
    public string Nom { get; set; }
    public string? Message { get; set; }
    public DateTime? Date { get; set; }
    public bool NonLus { get; set; }
}