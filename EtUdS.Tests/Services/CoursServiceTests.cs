using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using etuds.Server.Services.Cours;
using Moq;

namespace EtUdS.Tests.Services;

public class CoursServiceTests
{
    private readonly CoursService _coursService;
    private readonly Mock<IBaseRepository> _mockRepository;

    public CoursServiceTests()
    {
        _mockRepository = new Mock<IBaseRepository>();

        _coursService = new CoursService(
            _mockRepository.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsCours_IfExists()
    {
        // Arrange
        var cours = new Cours { Id = 1, Sigle = "IFT123", Titre = "Test", IdProfesseur = 1 };
        _mockRepository.Setup(repo => repo.GetById<Cours>(1))
            .ReturnsAsync(cours);

        // Act
        var result = await _coursService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.IdProfesseur);
        Assert.Equal("Test", result.Titre);
        Assert.Equal("IFT123", result.Sigle);
        _mockRepository.Verify(repo => repo.GetById<Cours>(1), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNull_IfCoursNotExists()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetById<Cours>(1))
            .ReturnsAsync((Cours)null);

        // Act
        var result = await _coursService.GetById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Add_CallsRepository_AddsCours()
    {
        // Arrange
        var cours = new Cours { Id = 1, Titre = "Test" };
        _mockRepository.Setup(repo => repo.Add(It.IsAny<Cours>()))
            .ReturnsAsync(cours);

        // Act
        var result = await _coursService.Add(cours);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Titre);
        _mockRepository.Verify(repo => repo.Add(It.IsAny<Cours>()), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Delete_CallsRepository_DeleteMethod()
    {
        // Arrange
        var cours = new Cours { Id = 1, Titre = "Test" };
        _mockRepository.Setup(repo => repo.Delete(cours));

        // Act
        await _coursService.Delete(cours);

        // Assert
        _mockRepository.Verify(repo => repo.Delete(cours), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Update_CallsRepository_UpdateMethod()
    {
        // Arrange
        var cours = new Cours { Id = 1, Titre = "Ancien nom", IdProfesseur = 1, Sigle = "IFT123" };
        var updatedCours = new Cours { Id = 1, Titre = "Nouveau nom", IdProfesseur = 2, Sigle = "IFT456" };
        _mockRepository.Setup(repo => repo.Update(cours));

        // Act
        await _coursService.Update(cours, updatedCours);

        // Assert
        Assert.Equal("Nouveau nom", cours.Titre);
        Assert.Equal(2, cours.IdProfesseur);
        Assert.Equal("IFT456", cours.Sigle);
        _mockRepository.Verify(repo => repo.Update(cours), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task GetAllCoursEtudiant_ReturnsCoursEtudiant()
    {
        // Arrange
        var cours = new List<Cours>
        {
            new Cours { Id = 1, Titre = "Cours 1", IdProfesseur = 1, Sigle = "IFT123" },
            new Cours { Id = 2, Titre = "Cours 2", IdProfesseur = 2, Sigle = "IFT456" },
            new Cours { Id = 3, Titre = "Cours 3", IdProfesseur = 3, Sigle = "IFT789" }
        };

        var inscriptions = new List<Inscription>
        {
            new Inscription { Id = 1, IdCours = 1, IdEtudiant = 1 },
            new Inscription { Id = 2, IdCours = 2, IdEtudiant = 1 },
            new Inscription { Id = 3, IdCours = 3, IdEtudiant = 2 }
        };

        _mockRepository.Setup(repo => repo.GetAll<Inscription>())
            .ReturnsAsync(inscriptions);
        _mockRepository.Setup(repo => repo.GetAll<Cours>())
            .ReturnsAsync(cours);

        // Act
        var result = await _coursService.GetAllCoursEtudiant(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    
    [Fact]
    public async Task GetAll_ReturnsListOfCours()
    {
        // Arrange
        var cours = new List<Cours>
        {
            new Cours { Id = 1, Titre = "Cours 1", IdProfesseur = 1, Sigle = "IFT123" },
            new Cours { Id = 2, Titre = "Cours 2", IdProfesseur = 2, Sigle = "IFT456" }
        };

        _mockRepository.Setup(repo => repo.GetAll<Cours>())
            .ReturnsAsync(cours);

        // Act
        var result = await _coursService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(repo => repo.GetAll<Cours>(), Times.Once);
    }
}