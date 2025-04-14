using EtUdS.Server.Dtos;
using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using etuds.Server.Services.Conversation;
using EtUdS.Server.Utils;

namespace etuds.Server.Services.Message;

public class MessageService: IMessageService
{
    private readonly IBaseRepository _repository;
    private readonly IConversationService _conversationService;
    
    public MessageService(IBaseRepository repository, IConversationService conversationService)
    {
        _repository = repository;
        _conversationService = conversationService;
    }

    public async Task<Entities.Message> GetById(int id)
    {
        var message = await _repository.GetById<Entities.Message>(id);
        return message;
    }
    
    public async Task<IEnumerable<Entities.Message>> GetAll()
    {
        return await _repository.GetAll<Entities.Message>();
    }
    
    public async Task<Entities.Message> Add(int idEtudiant, Entities.Message message)
    {
        var messageFull = message;
        messageFull.DateEnvoi = Dates.GetTimeNowEst().ToUniversalTime();
        messageFull.IdEtudiant = idEtudiant;
        
        var addedMessage = await _repository.Add(message);
        await _repository.Save();
        
        var participants = await _conversationService.GetParticipants(message.IdConversation);
        foreach (var participant in participants)
        {
            if (participant.IdEtudiant != idEtudiant)
            {
                await _repository.Add(new MessageNonLu { IdEtudiant = participant.IdEtudiant, IdMessage = addedMessage.Id, RappelEnvoye = false });
            }
        }
        await _repository.Save();
        
        return addedMessage;
    }
    
    public async Task Update(Entities.Message message, Entities.Message updatedMessage)
    {
        message.IdConversation = updatedMessage.IdConversation;
        message.IdEtudiant = updatedMessage.IdEtudiant;
        message.Contenu = updatedMessage.Contenu;
        message.DateEnvoi = updatedMessage.DateEnvoi;
        
        _repository.Update(message);
        await _repository.Save();
    }

    public async Task Delete(Entities.Message message)
    {
        _repository.Delete(message);
        await _repository.Save();
    }

    public async Task<IEnumerable<MessagesVuesDTO>> GetMessagesOfConversation(int idEtudiant, int idConversation)
    {
        var participants = await _conversationService.GetParticipants(idConversation);
        if (!participants.Any(p => p.IdEtudiant == idEtudiant))
            throw new Exception("L'étudiant actuel n'est pas dans la conversation.");
        
        var messages = await _repository.GetAll<Entities.Message>();
        messages = messages.Where(m => m.IdConversation == idConversation);
        
        var messageIds = messages.Select(m => m.Id).ToList();
        var messagesNonLus = await _repository.GetAll<MessageNonLu>();
        messagesNonLus = messagesNonLus.Where(m => messageIds.Contains(m.IdMessage));

        foreach (var messageNonLu in messagesNonLus)
        {
            if (messageNonLu.IdEtudiant == idEtudiant)
                _repository.Delete(messageNonLu);
        }

        await _repository.Save();
        
        var nonLusDict = messagesNonLus
            .GroupBy(m => m.IdMessage)
            .ToDictionary(g => g.Key, g => g.Select(m => m.IdEtudiant).ToList());
        var etudiants = await _repository.GetAll<Entities.Etudiant>();
        var etudiantDict = etudiants.ToDictionary(e => e.Id);
    
        List<MessagesVuesDTO> messagesFormatted = new List<MessagesVuesDTO>();
    
        foreach (var message in messages)
        {
            if (message.IdEtudiant != null)
            {
                etudiantDict.TryGetValue(message.IdEtudiant.Value, out var etudiant);

                var vuesMessage = participants
                    .Where(p => !nonLusDict.ContainsKey(message.Id) || !nonLusDict[message.Id].Contains(p.IdEtudiant))
                    .Select(p => new VuesDTO
                    {
                        Id = p.IdEtudiant,
                        Nom = etudiantDict.ContainsKey(p.IdEtudiant)
                            ? etudiantDict[p.IdEtudiant].Prenom + " " + etudiantDict[p.IdEtudiant].Nom
                            : ""
                    })
                    .ToList();
                
                messagesFormatted.Add(new MessagesVuesDTO
                {
                    IdSender = message.IdEtudiant,
                    NomSender = etudiant != null ? $"{etudiant.Prenom} {etudiant.Nom}" : "",
                    Date = message.DateEnvoi,
                    Message = message.Contenu,
                    Vues = vuesMessage
                });
            }
            else
            {
                messagesFormatted.Add(new MessagesVuesDTO
                {
                    IdSender = message.IdEtudiant,
                    NomSender = null,
                    Date = message.DateEnvoi,
                    Message = message.Contenu,
                    Vues = null
                });
            }
            
        }
    
        return messagesFormatted;
    }
}