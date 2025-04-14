using EtUdS.Server.Dtos;

namespace etuds.Server.Services.Conversation;

public interface IConversationService
{
    Task<Entities.Conversation> GetById(int id);
    Task<IEnumerable<ConversationDetailsDTO>> GetAll(int idEtudiant);
    Task<IEnumerable<Entities.ConversationParticipant>> GetParticipants(int id);
    Task<IEnumerable<ParticipantEtudiantDTO>> GetParticipantsEtudiants(int id);
    Task<Entities.Conversation> Add(int idEtudiant, Entities.Conversation conversation);
    Task AjouterParticipants(int etudiantId, int conversationId, List<int> etudiantIds);
    Task ModifierParticipants(int etudiantId, int conversationId, List<int> etudiantIds);
    Task Quitter(int etudiantId, int conversationId);
    Task Update(Entities.Conversation conversation, Entities.Conversation updatedConversation);
    Task Delete(Entities.Conversation conversation);
}