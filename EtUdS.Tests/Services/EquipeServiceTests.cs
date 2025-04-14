using etuds.Server.Entities;
using EtUdS.Server.Repositories.Equipe;
using etuds.Server.Repositories.Etudiant;
using EtUdS.Server.Repositories.Membre;
using etuds.Server.Services.Equipe;
using Moq;

namespace EtUdS.Tests.Services;

public class EquipeServiceTests
{
    private readonly Mock<IEquipeRepository> _mockEquipeRepository;
    private readonly Mock<IMembreRepository> _mockMembreRepository;
    private readonly Mock<IEtudiantRepository> _mockEtudiantRepository;
    private readonly EquipeService _equipeService;

    public EquipeServiceTests()
    {
        _mockEquipeRepository = new Mock<IEquipeRepository>();
        _mockMembreRepository = new Mock<IMembreRepository>();
        _mockEtudiantRepository = new Mock<IEtudiantRepository>();

        _equipeService = new EquipeService(
            _mockEquipeRepository.Object,
            _mockMembreRepository.Object,
            _mockEtudiantRepository.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsEquipe_IfExists()
    {
        // Arrange
        var equipe = new Equipe { Id = 1 };
        _mockEquipeRepository.Setup(repo => repo.GetById<Equipe>(1))
            .ReturnsAsync(equipe);

        // Act
        var result = await _equipeService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _mockEquipeRepository.Verify(repo => repo.GetById<Equipe>(1), Times.Once);
    }
    
    [Fact]
    public async Task GetDetailsMembre_ReturnsEquipeWithDetails_IfExists()
    {
        // TODO
    }
    
    [Fact]
    public async Task GetById_ReturnsNull_IfEquipeNotExists()
    {
        // Arrange
        _mockEquipeRepository.Setup(repo => repo.GetById<Equipe>(1))
            .ReturnsAsync((Equipe)null);

        // Act
        var result = await _equipeService.GetById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Add_CallsRepository_AddsEquipe()
    {
        // Arrange
        var equipe = new Equipe { Id = 1 };
        _mockEquipeRepository.Setup(repo => repo.Add(It.IsAny<Equipe>()))
            .ReturnsAsync(equipe);

        // Act
        var result = await _equipeService.Add(equipe);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        _mockEquipeRepository.Verify(repo => repo.Add(It.IsAny<Equipe>()), Times.Once);
        _mockEquipeRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Delete_CallsRepository_DeleteMethod()
    {
        // Arrange
        var equipe = new Equipe { Id = 1 };
        _mockEquipeRepository.Setup(repo => repo.Delete(equipe));

        // Act
        await _equipeService.Delete(equipe);

        // Assert
        _mockEquipeRepository.Verify(repo => repo.Delete(equipe), Times.Once);
        _mockEquipeRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Update_CallsRepository_UpdateMethod()
    {
        // Arrange
        var equipe = new Equipe { Id = 1, IdDevoir = 1, EtatSoumission = EtatSoumission.Evalue, Note = 1 };
        var updatedEquipe = new Equipe { Id = 1, IdDevoir = 2, EtatSoumission = EtatSoumission.AFaire, Note = 2 };
        _mockEquipeRepository.Setup(repo => repo.Update(equipe));

        // Act
        await _equipeService.Update(equipe, updatedEquipe);

        // Assert
        Assert.Equal(2, equipe.IdDevoir);
        Assert.Equal(EtatSoumission.AFaire, equipe.EtatSoumission);
        Assert.Equal(2, equipe.Note);
        _mockEquipeRepository.Verify(repo => repo.Update(equipe), Times.Once);
        _mockEquipeRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsListOfEquipes()
    {
        // Arrange
        var equipes = new List<Equipe>
        {
            new Equipe { Id = 1 },
            new Equipe { Id = 2 }
        };

        _mockEquipeRepository.Setup(repo => repo.GetAll<Equipe>())
            .ReturnsAsync(equipes);

        // Act
        var result = await _equipeService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockEquipeRepository.Verify(repo => repo.GetAll<Equipe>(), Times.Once);
    }
}