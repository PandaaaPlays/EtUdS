namespace EtUdS.Server.Dtos;

public class MessagesVuesDTO
{
    public int? IdSender { get; set; }
    public string NomSender { get; set; }
    public DateTime Date { get; set; }
    public string Message { get; set; }
    public List<VuesDTO>? Vues { get; set; }
}