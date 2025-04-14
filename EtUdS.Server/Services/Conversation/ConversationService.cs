using EtUdS.Server.Dtos;
using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using etuds.Server.Repositories.Etudiant;
using EtUdS.Server.Utils;

namespace etuds.Server.Services.Conversation;

public class ConversationService: IConversationService
{
    private readonly IBaseRepository _repository;
    private readonly IEtudiantRepository _etudiantRepository;
    
    public ConversationService(IBaseRepository repository, IEtudiantRepository etudiantRepository)
    {
        _repository = repository;
        _etudiantRepository = etudiantRepository;
    }

    public async Task<Entities.Conversation> GetById(int id)
    {
        var conversation = await _repository.GetById<Entities.Conversation>(id);
        return conversation;
    }
    
    public async Task<IEnumerable<ConversationDetailsDTO>> GetAll(int idEtudiant)
    {
        var conversations = await _repository.GetAll<Entities.Conversation>();
        var messagesNonLus = await _repository.GetAll<MessageNonLu>();
        var filteredConversations = new List<ConversationDetailsDTO>();
        foreach (var conversation in conversations)
        {
            var participants = await GetParticipants(conversation.Id);
            if (participants.Select(x => x.IdEtudiant).Contains(idEtudiant))
            {
                var messages = await _repository.GetAll<Entities.Message>();
                var message = messages
                    .Where(m => m.IdConversation == conversation.Id && m.IdEtudiant != null)
                    .OrderByDescending(m => m.DateEnvoi)
                    .FirstOrDefault();
                
                Entities.Etudiant? etudiant = message != null && message.IdEtudiant != null ? await _etudiantRepository.GetById<Entities.Etudiant>(message.IdEtudiant.GetValueOrDefault()) : null;
                // Le dernier sera nécessairement non lu si jamais il y en a d'autres qui sont non lus.
                var messageNonLus = messagesNonLus.ToList();
                var nonLus = message != null && messageNonLus.Any(m => message.Id == m.IdMessage && m.IdEtudiant == idEtudiant);
                
                filteredConversations.Add(new ConversationDetailsDTO
                {
                    Id = conversation.Id,
                    Nom = conversation.Titre,
                    Prenom = etudiant != null ? etudiant.Prenom : null,
                    Message = message != null ? message.Contenu : null,
                    Date = message != null ? message.DateEnvoi : (DateTime?) null,
                    NonLus = nonLus
                });
            }
        }
        return filteredConversations;
    }

    public async Task<IEnumerable<ConversationParticipant>> GetParticipants(int id)
    {
        var participants = await _repository.GetAll<ConversationParticipant>();
        return participants.Where(c => c.IdConversation == id);
    }
    
    public async Task<IEnumerable<ParticipantEtudiantDTO>> GetParticipantsEtudiants(int id)
    {
        var participants = await GetParticipants(id);
        var etudiants = new List<ParticipantEtudiantDTO>();
        foreach (var participant in participants)
        {
            var etudiant = await _etudiantRepository.GetById<Entities.Etudiant>(participant.IdEtudiant);
            etudiants.Add(new ParticipantEtudiantDTO
            {
                IdEtudiant = participant.IdEtudiant,
                IdParticipant = participant.Id,
                Prenom = etudiant.Prenom,
                Nom = etudiant.Nom,
            });
        }

        return etudiants;
    }
    
    public async Task<Entities.Conversation> Add(int idEtudiant, Entities.Conversation conversation)
    {
        var addedConversation = await _repository.Add(conversation);
        await _repository.Save();
        await _repository.Add(new ConversationParticipant
        {
            IdConversation = addedConversation.Id,
            IdEtudiant = idEtudiant
        } );
        await _repository.Save();
        
        return addedConversation;
    }

    public async Task AjouterParticipants(int etudiantId, int conversationId, List<int> etudiantIds)
    {
        var participants = await GetParticipants(conversationId);
        if (!participants.Any(p => p.IdEtudiant == etudiantId))
            throw new Exception("L'étudiant actuel n'est pas dans la conversation.");

        foreach (int id in etudiantIds)
        {
            if (!participants.Any(p => p.IdEtudiant == id))
            {
                await _repository.Add(new ConversationParticipant
                {
                    IdConversation = conversationId,
                    IdEtudiant = id
                });
            }
        }

        await _repository.Save();

    }
    
    public async Task ModifierParticipants(int etudiantId, int conversationId, List<int> etudiantIds)
    {
        var participants = await GetParticipants(conversationId);
        if (!participants.Any(p => p.IdEtudiant == etudiantId))
            throw new Exception("L'étudiant actuel n'est pas dans la conversation.");
        
        var messagesNonLus = await _repository.GetAll<MessageNonLu>();

        foreach (var participant in participants)
        {
            if (!etudiantIds.Any(id => participant.IdEtudiant == id) && participant.IdEtudiant != etudiantId)
            {
                var messagesNonLusParticipant = messagesNonLus.Where(m => m.IdEtudiant == participant.IdEtudiant);
                foreach (var messageNonLu in messagesNonLusParticipant)
                {
                    var message = await _repository.GetById<Entities.Message>(messageNonLu.IdMessage);
                    if (message.IdConversation == conversationId)
                    {
                        _repository.Delete(messageNonLu);
                    }
                }
                
                var etudiant = await _etudiantRepository.GetById<Entities.Etudiant>(participant.IdEtudiant);
                await _repository.Add(new Entities.Message 
                {
                    IdConversation = conversationId,
                    IdEtudiant = null,
                    Contenu = etudiant.Prenom + " " + etudiant.Nom + " à été retiré de la conversation.",
                    DateEnvoi = Dates.GetTimeNowEst().ToUniversalTime()
                });
                
                _repository.Delete(participant);
            }
            
        }
        await _repository.Save();
        
        foreach (int id in etudiantIds)
        {
            if (!participants.Any(p => p.IdEtudiant == id))
            {
                await _repository.Add(new ConversationParticipant
                {
                    IdConversation = conversationId,
                    IdEtudiant = id
                });

                var etudiant = await _etudiantRepository.GetById<Entities.Etudiant>(id);
                await _repository.Add(new Entities.Message 
                {
                    IdConversation = conversationId,
                    IdEtudiant = null,
                    Contenu = etudiant.Prenom + " " + etudiant.Nom + " à été ajouté à la conversation.",
                    DateEnvoi = Dates.GetTimeNowEst().ToUniversalTime()
                });
            }
        }
        await _repository.Save();

    }

    public async Task Quitter(int etudiantId, int conversationId)
    {
        var participant = await GetParticipants(conversationId);
        _repository.Delete(participant.First(p => p.IdEtudiant == etudiantId));
        var etudiant = await _etudiantRepository.GetById<Entities.Etudiant>(etudiantId);
        await _repository.Add(new Entities.Message
        {
            IdConversation = conversationId,
            IdEtudiant = null,
            Contenu = etudiant.Prenom + " " + etudiant.Nom + " à quitté la conversation.",
            DateEnvoi = Dates.GetTimeNowEst().ToUniversalTime()
        });
        await _repository.Save();
    }

    public async Task Update(Entities.Conversation conversation, Entities.Conversation updatedConversation)
    {
        var titre = conversation.Titre;
        
        conversation.Titre = updatedConversation.Titre;
        if (titre != updatedConversation.Titre)
        {
            await _repository.Add(new Entities.Message
            {
                IdConversation = updatedConversation.Id,
                IdEtudiant = null,
                Contenu = "Le titre de la conversation à été modifié pour " + conversation.Titre + ".",
                DateEnvoi = Dates.GetTimeNowEst().ToUniversalTime()
            });
        }

        _repository.Update(conversation);
        await _repository.Save();
    }

    public async Task Delete(Entities.Conversation conversation)
    {
        _repository.Delete(conversation);
        await _repository.Save();
    }
}