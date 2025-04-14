using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using etuds.Server.Repositories.Etudiant;
using etuds.Server.Services.Conversation;
using Moq;

namespace EtUdS.Tests.Services;

public class ConversationServiceTests
{
    private readonly ConversationService _conversationService;
    private readonly Mock<IBaseRepository> _mockRepository;
    private readonly Mock<IEtudiantRepository> _mockEtudiantRepository;

    public ConversationServiceTests()
    {
        _mockRepository = new Mock<IBaseRepository>();
        _mockEtudiantRepository = new Mock<IEtudiantRepository>();

        _conversationService = new ConversationService(
            _mockRepository.Object,
            _mockEtudiantRepository.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsConversation_IfExists()
    {
        // Arrange
        var conversation = new Conversation { Id = 1, Titre = "Conversation" };
        _mockRepository.Setup(repo => repo.GetById<Conversation>(1))
            .ReturnsAsync(conversation);

        // Act
        var result = await _conversationService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Conversation", result.Titre);
        _mockRepository.Verify(repo => repo.GetById<Conversation>(1), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNull_IfConversationNotExists()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetById<Conversation>(1))
            .ReturnsAsync((Conversation)null);

        // Act
        var result = await _conversationService.GetById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Add_CallsRepository_AddsConversation()
    {
        // Arrange
        var conversation = new Conversation { Id = 1, Titre = "Conversation" };
        _mockRepository.Setup(repo => repo.Add(It.IsAny<Conversation>()))
            .ReturnsAsync(conversation);

        // Act
        var result = await _conversationService.Add(1, conversation);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Conversation", result.Titre);
        _mockRepository.Verify(repo => repo.Add(It.IsAny<Conversation>()), Times.Once);
        _mockRepository.Verify(repo => repo.Add(It.IsAny<ConversationParticipant>()), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Exactly(2));
    }
    
    [Fact]
    public async Task Delete_CallsRepository_DeleteMethod()
    {
        // Arrange
        var conversation = new Conversation { Id = 1 };
        _mockRepository.Setup(repo => repo.Delete(conversation));

        // Act
        await _conversationService.Delete(conversation);

        // Assert
        _mockRepository.Verify(repo => repo.Delete(conversation), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Update_CallsRepository_UpdateMethod()
    {
        // Arrange
        var conversation = new Conversation { Id = 1, Titre = "Conversation" };
        var updatedConversation = new Conversation { Id = 1, Titre = "Conversation update" };
        _mockRepository.Setup(repo => repo.Update(conversation));

        // Act
        await _conversationService.Update(conversation, updatedConversation);

        // Assert
        Assert.Equal("Conversation update", conversation.Titre);
        _mockRepository.Verify(repo => repo.Update(conversation), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }
    
    [Fact]
    public async Task GetParticipants_ReturnsParticipants_IfAny()
    {
        // Arrange
        var participants = new List<ConversationParticipant>()
        {
            new ConversationParticipant { Id = 1, IdConversation = 1, IdEtudiant = 1 },
            new ConversationParticipant { Id = 2, IdConversation = 1, IdEtudiant = 2 },
            new ConversationParticipant { Id = 3, IdConversation = 2, IdEtudiant = 3 }
        };
        _mockRepository.Setup(repo => repo.GetAll<ConversationParticipant>())
            .ReturnsAsync(participants);

        // Act
        var result = await _conversationService.GetParticipants(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    
    [Fact]
    public async Task GetParticipantsEtudiant_ReturnsParticipants_IfAny()
    {
        // Arrange
        var participants = new List<ConversationParticipant>()
        {
            new ConversationParticipant { Id = 1, IdConversation = 1, IdEtudiant = 1 },
            new ConversationParticipant { Id = 2, IdConversation = 1, IdEtudiant = 2 },
            new ConversationParticipant { Id = 3, IdConversation = 2, IdEtudiant = 3 }
        };
        var etudiants = new List<Etudiant>()
        {
            new Etudiant { Id = 1, Prenom = "Test" },
            new Etudiant { Id = 2, Prenom = "Test2" },
            new Etudiant { Id = 3, Prenom = "Test3" }
        };
        
        _mockRepository.Setup(repo => repo.GetAll<ConversationParticipant>())
            .ReturnsAsync(participants);
        for (int i = 1; i < 4; i++)
        {
            _mockEtudiantRepository.Setup(repo => repo.GetById<Etudiant>(i))
                .ReturnsAsync(etudiants[i-1]);
        }

        // Act
        var result = await _conversationService.GetParticipantsEtudiants(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Test", result.ElementAt(0).Prenom);
    }
    
    [Fact]
    public async Task GetAll_ReturnsConversationsOfCurrent()
    {
        // Arrange
        var conversations = new List<Conversation>
        {
            new Conversation { Id = 1, Titre = "Conversation 1" },
            new Conversation { Id = 2, Titre = "Conversation 2" }
        };
        var participants = new List<ConversationParticipant>
        {
            new ConversationParticipant { Id = 1, IdConversation = 1, IdEtudiant = 1 },
            new ConversationParticipant { Id = 2, IdConversation = 1, IdEtudiant = 2 },
            new ConversationParticipant { Id = 3, IdConversation = 2, IdEtudiant = 3 }
        };
        var messages = new List<Message>
        {
            new Message { Id = 1, IdEtudiant = 1, IdConversation = 1, Contenu = "test" },
            new Message { Id = 2, IdEtudiant = 2, IdConversation = 1, Contenu = "test" }
        };
        var etudiant = new Etudiant { Id = 1, Prenom = "Test" };
        var etudiant2 = new Etudiant { Id = 2, Prenom = "Test2" };
        
        _mockRepository.Setup(repo => repo.GetAll<ConversationParticipant>())
            .ReturnsAsync(participants);
        _mockRepository.Setup(repo => repo.GetAll<Conversation>())
            .ReturnsAsync(conversations);
        _mockRepository.Setup(repo => repo.GetAll<MessageNonLu>())
            .ReturnsAsync(new List<MessageNonLu> { new MessageNonLu {IdMessage = 1 }});
        _mockRepository.Setup(repo => repo.GetAll<Message>())
            .ReturnsAsync(messages);
        _mockEtudiantRepository.Setup(s => s.GetById<Etudiant>(1))
            .ReturnsAsync(etudiant);
        _mockEtudiantRepository.Setup(s => s.GetById<Etudiant>(2))
            .ReturnsAsync(etudiant2);

        // Act
        var result = await _conversationService.GetAll(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}