using EtUdS.Server.Dtos;

namespace etuds.Server.Services.Message;

public interface IMessageService
{
    Task<Entities.Message> GetById(int id);
    Task<IEnumerable<Entities.Message>> GetAll();
    Task<Entities.Message> Add(int idEtudiant, Entities.Message message);
    Task Update(Entities.Message conversation, Entities.Message updatedConversation);
    Task Delete(Entities.Message conversation);
    Task<IEnumerable<MessagesVuesDTO>> GetMessagesOfConversation(int idEtudiant, int idConversation);
}