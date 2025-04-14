using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using EtUdS.Server.Repositories.Equipe;
using etuds.Server.Repositories.Etudiant;
using EtUdS.Server.Repositories.Membre;
using etuds.Server.Services.Conversation;
using etuds.Server.Services.Devoir;
using etuds.Server.Services.Message;
using Moq;

namespace EtUdS.Tests.Services;

public class MessageServiceTests
{
    private readonly Mock<IBaseRepository> _mockRepository;
    private readonly Mock<IConversationService> _mockConversationService;
    private readonly MessageService _messageService;

    public MessageServiceTests()
    {
        _mockRepository = new Mock<IBaseRepository>();
        _mockConversationService = new Mock<IConversationService>();

        _messageService = new MessageService(
            _mockRepository.Object,
            _mockConversationService.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsMessage_IfExists()
    {
        // Arrange
        var message = new Message { Id = 1, Contenu = "Test" };
        _mockRepository.Setup(repo => repo.GetById<Message>(1))
            .ReturnsAsync(message);

        // Act
        var result = await _messageService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", message.Contenu);
        _mockRepository.Verify(repo => repo.GetById<Message>(1), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNull_IfMessageNotExists()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetById<Message>(1))
            .ReturnsAsync((Message)null);

        // Act
        var result = await _messageService.GetById(1);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetAll_ReturnsMessages_IfAny()
    {
        // Arrange
        var message = new Message { Id = 1, Contenu = "Test" };
        var message2 = new Message { Id = 2, Contenu = "Test2" };
        _mockRepository.Setup(repo => repo.GetAll<Message>())
            .ReturnsAsync(new []{message, message2});

        // Act
        var result = await _messageService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(repo => repo.GetAll<Message>(), Times.Once);
    }
    
    [Fact]
    public async Task Add_CallsRepository_AddsMessage()
    {
        // Arrange
        var participant = new ConversationParticipant { Id = 1, IdConversation = 1};
        var participant2 = new ConversationParticipant { Id = 2, IdConversation = 1};
        var message = new Message { Id = 1, IdConversation = 1, Contenu = "Test" };
        _mockRepository.Setup(repo => repo.Add(It.IsAny<Message>()))
            .ReturnsAsync(message);
        _mockConversationService.Setup(service => service.GetParticipants(1))
            .ReturnsAsync(new[] { participant, participant2 });

        // Act
        var result = await _messageService.Add(1, message);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Contenu);
        _mockRepository.Verify(repo => repo.Add(It.IsAny<Message>()), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Exactly(2));
    }
    
    [Fact]
    public async Task Update_CallsRepository_UpdateMethod()
    {
        // Arrange
        var message = new Message { Id = 1, Contenu = "Test", IdEtudiant = 1, IdConversation = 1, DateEnvoi = new DateTime(2000, 1, 1) };
        var updatedMessage = new Message { Id = 1, Contenu = "Test2", IdEtudiant = 2, IdConversation = 2, DateEnvoi = new DateTime(2001, 2, 2) };
        _mockRepository.Setup(repo => repo.Update(message));

        // Act
        await _messageService.Update(message, updatedMessage);

        // Assert
        Assert.Equal("Test2", message.Contenu);
        Assert.Equal(2, message.IdEtudiant);
        Assert.Equal(2, message.IdConversation);
        Assert.Equal(new DateTime(2001, 2, 2), message.DateEnvoi);
        _mockRepository.Verify(repo => repo.Update(message), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }
    
    [Fact]
    public async Task Delete_CallsRepository_DeleteMethod()
    {
        // Arrange
        var message = new Message { Id = 1, Contenu = "Test" };
        _mockRepository.Setup(repo => repo.Delete(message));

        // Act
        await _messageService.Delete(message);

        // Assert
        _mockRepository.Verify(repo => repo.Delete(message), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }
    
    [Fact]
    public async Task GettingMessagesOfConversation_ReturnsConversationMessage_IfAny()
    {
        // Arrange
        var participant = new ConversationParticipant { Id = 1, IdConversation = 1, IdEtudiant = 1};
        var message = new Message { Id = 1, IdConversation = 1, Contenu = "Test", IdEtudiant = 2 };
        var message2 = new Message { Id = 2, IdConversation = 1, Contenu = "Test2", IdEtudiant = 3 };
        var messageNonLu = new MessageNonLu { Id = 1, IdMessage = 2, IdEtudiant = 1 };
        var messageHorsConversation = new Message { Id = 3, IdConversation = 2, Contenu = "Test3", IdEtudiant = 1};
        _mockConversationService.Setup(service => service.GetParticipants(1))
            .ReturnsAsync(new[] { participant });
        _mockRepository.Setup(repo => repo.GetAll<Message>())
            .ReturnsAsync(new []{message, message2, messageHorsConversation});
        _mockRepository.Setup(repo => repo.GetAll<MessageNonLu>())
            .ReturnsAsync(new []{messageNonLu});
        
        // Act
        var results = await _messageService.GetMessagesOfConversation(1, 1);

        // Assert
        Assert.Equal(2, results.Count());
        _mockRepository.Verify(repo => repo.Delete(messageNonLu), Times.Once);
    }

}