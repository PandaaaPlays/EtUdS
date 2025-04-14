using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using EtUdS.Server.Repositories.Membre;
using etuds.Server.Services.Devoir;
using etuds.Server.Services.Equipe;
using etuds.Server.Services.Membre;
using Moq;

namespace EtUdS.Tests.Services;

public class MembreServiceTests
{
    private readonly Mock<IMembreRepository> _mockMembreRepository;
    private readonly Mock<IEquipeService> _mockEquipeService;
    private readonly Mock<IDevoirService> _mockDevoirService;
    private readonly Mock<IBaseRepository> _mockDocumentRepository;
    private readonly MembreService _membreService;

    public MembreServiceTests()
    {
        _mockMembreRepository = new Mock<IMembreRepository>();
        _mockEquipeService = new Mock<IEquipeService>();
        _mockDevoirService = new Mock<IDevoirService>();
        _mockDocumentRepository = new Mock<IBaseRepository>();
        
        _membreService = new MembreService(
            _mockMembreRepository.Object,
            _mockEquipeService.Object,
            _mockDevoirService.Object,
            _mockDocumentRepository.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsMembre_IfExists()
    {
        // Arrange
        var membre = new Membre { Id = 1 };
        _mockMembreRepository.Setup(repo => repo.GetById<Membre>(1))
            .ReturnsAsync(membre);

        // Act
        var result = await _membreService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _mockMembreRepository.Verify(repo => repo.GetById<Membre>(1), Times.Once);
    }
    
    [Fact]
    public async Task GetById_ReturnsNull_IfMembreNotExists()
    {
        // Arrange
        _mockMembreRepository.Setup(repo => repo.GetById<Membre>(1))
            .ReturnsAsync((Membre)null);

        // Act
        var result = await _membreService.GetById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Add_CallsRepository_AddsMembre()
    {
        // Arrange
        var membre = new Membre { Id = 1 };
        _mockMembreRepository.Setup(repo => repo.Add(It.IsAny<Membre>()))
            .ReturnsAsync(membre);

        // Act
        var result = await _membreService.Add(membre);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _mockMembreRepository.Verify(repo => repo.Add(It.IsAny<Membre>()), Times.Once);
        _mockMembreRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Delete_CallsRepository_DeleteMethod()
    {
        // Arrange
        var membre = new Membre { Id = 1 };
        _mockMembreRepository.Setup(repo => repo.Delete(membre));

        // Act
        await _membreService.Delete(membre);

        // Assert
        _mockMembreRepository.Verify(repo => repo.Delete(membre), Times.Once);
        _mockMembreRepository.Verify(repo => repo.Save(), Times.Exactly(2));
    }

    [Fact]
    public async Task Update_CallsRepository_UpdateMethod()
    {
        // Arrange
        var membre = new Membre { Id = 1, IdEtudiant = 1, Accepte = false, IdEquipe = 1 };
        var updatedMembre = new Membre { Id = 1, IdEtudiant = 2, Accepte = true, IdEquipe = 2 };
        _mockMembreRepository.Setup(repo => repo.Update(membre));

        // Act
        await _membreService.Update(membre, updatedMembre);

        // Assert
        Assert.True(membre.Accepte);
        Assert.Equal(2, membre.IdEquipe);
        Assert.Equal(2, membre.IdEtudiant);
        _mockMembreRepository.Verify(repo => repo.Update(membre), Times.Once);
        _mockMembreRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsListOfMembres()
    {
        // Arrange
        var membres = new List<Membre>
        {
            new Membre { Id = 1 },
            new Membre { Id = 2 }
        };

        _mockMembreRepository.Setup(repo => repo.GetAll<Membre>())
            .ReturnsAsync(membres);

        // Act
        var result = await _membreService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockMembreRepository.Verify(repo => repo.GetAll<Membre>(), Times.Once);
    }
    
    [Fact]
    public async Task AccepterInvitation_ChangeEtatMembre()
    {
        // Arrange
        var membres = new List<Membre>
        {
            new Membre { Id = 1, IdEtudiant = 1, Accepte = false, IdEquipe = 1 },
            new Membre { Id = 2, IdEtudiant = 2, Accepte = true, IdEquipe = 1 },
            new Membre { Id = 3, IdEtudiant = 3, Accepte = true, IdEquipe = 2 }
        };

        _mockMembreRepository.Setup(repo => repo.GetAll<Membre>())
            .ReturnsAsync(membres);
        _mockMembreRepository.Setup(repo => repo.GetAllMembresByEquipeId(1))
            .ReturnsAsync(membres.Where(x => x.IdEquipe == 1));
        _mockEquipeService.Setup(s => s.GetById(1))
            .ReturnsAsync(new Equipe { Id = 1, IdDevoir = 1 });
        _mockEquipeService.Setup(s => s.GetById(2))
            .ReturnsAsync(new Equipe { Id = 2, IdDevoir = 1 });

        // Act
        await _membreService.AccepterInvitation(1, 1);

        // Assert
        Assert.True(membres.ElementAt(0).Accepte);
        _mockMembreRepository.Verify(repo => repo.Update(membres.ElementAt(0)), Times.Once);
    }
}