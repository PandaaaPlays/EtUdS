using EtUdS.Server.Dtos;
using etuds.Server.Entities;
using etuds.Server.Repositories.Etudiant;
using etuds.Server.Services.Etudiant;
using Moq;

namespace EtUdS.Tests.Services;

public class EtudiantServiceTests
{
    private readonly Mock<IEtudiantRepository> _mockEtudiantRepository;
    private readonly EtudiantService _etudiantService;

    public EtudiantServiceTests()
    {
        _mockEtudiantRepository = new Mock<IEtudiantRepository>();

        _etudiantService = new EtudiantService(
            _mockEtudiantRepository.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsEtudiant_IfExists()
    {
        // Arrange
        var etudiant = new Etudiant { Id = 1, Nom = "Nom", Prenom = "Prenom", Courriel = "Email", MotDePasse = "Test"};
        _mockEtudiantRepository.Setup(repo => repo.GetById<Etudiant>(1))
            .ReturnsAsync(etudiant);

        // Act
        var result = await _etudiantService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Prenom", result.Prenom);
        Assert.Equal("Nom", result.Nom);
        Assert.Equal("Email", result.Courriel);
        Assert.IsType<EtudiantDTO>(result);
        _mockEtudiantRepository.Verify(repo => repo.GetById<Etudiant>(1), Times.Once);
    }
    
    [Fact]
    public async Task GetByCourriel_ReturnsEtudiant_IfExists()
    {
        // Arrange
        var etudiant = new Etudiant { Id = 1, Nom = "Nom", Prenom = "Prenom", Courriel = "Email", MotDePasse = "Test"};
        _mockEtudiantRepository.Setup(repo => repo.GetByCourriel("Email"))
            .ReturnsAsync(etudiant);

        // Act
        var result = await _etudiantService.GetByCourriel("Email");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Prenom", result.Prenom);
        Assert.Equal("Nom", result.Nom);
        Assert.Equal("Email", result.Courriel);
        Assert.IsType<Etudiant>(result);
        _mockEtudiantRepository.Verify(repo => repo.GetByCourriel("Email"), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNull_IfEtudiantNotExists()
    {
        // Arrange
        _mockEtudiantRepository.Setup(repo => repo.GetById<Etudiant>(1))
            .ReturnsAsync((Etudiant)null);

        // Act
        var result = await _etudiantService.GetById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAvecPassword_ReturnsEtudiant_IfExists()
    {
        // Arrange
        var etudiant = new Etudiant { Id = 1, Nom = "Nom", Prenom = "Prenom", Courriel = "Email", MotDePasse = "Test"};
        _mockEtudiantRepository.Setup(repo => repo.GetById<Etudiant>(1))
            .ReturnsAsync(etudiant);

        // Act
        var result = await _etudiantService.GetByIdWithPassword(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Prenom", result.Prenom);
        Assert.Equal("Nom", result.Nom);
        Assert.Equal("Email", result.Courriel);
        Assert.Equal("Test", result.MotDePasse);
        Assert.IsType<Etudiant>(result);
        _mockEtudiantRepository.Verify(repo => repo.GetById<Etudiant>(1), Times.Once);
    }

    [Fact]
    public async Task Add_CallsRepository_AddsEtudiant()
    {
        // Arrange
        var etudiant = new Etudiant { Id = 1, Prenom = "Prenom" };
        _mockEtudiantRepository.Setup(repo => repo.Add(It.IsAny<Etudiant>()))
            .ReturnsAsync(etudiant);

        // Act
        var result = await _etudiantService.Add(etudiant);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Prenom", result.Prenom);
        _mockEtudiantRepository.Verify(repo => repo.Add(It.IsAny<Etudiant>()), Times.Once);
        _mockEtudiantRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Delete_CallsRepository_DeleteMethod()
    {
        // Arrange
        var etudiant = new Etudiant { Id = 1, Prenom = "Prenom" };
        _mockEtudiantRepository.Setup(repo => repo.Delete(etudiant));

        // Act
        await _etudiantService.Delete(etudiant);

        // Assert
        _mockEtudiantRepository.Verify(repo => repo.Delete(etudiant), Times.Once);
        _mockEtudiantRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Update_CallsRepository_UpdateMethod()
    {
        // Arrange
        var etudiant = new Etudiant { Id = 1, Prenom = "Ancien prenom", Nom = "Ancien nom", Courriel = "Ancien email", MotDePasse = "Ancien mot de passe"};
        var updatedEtudiant = new Etudiant { Id = 1, Prenom = "Nouveau prenom", Nom = "Nouveau nom", Courriel = "Nouveau email", MotDePasse = "Nouveau mot de passe"};
        _mockEtudiantRepository.Setup(repo => repo.Update(etudiant));

        // Act
        await _etudiantService.Update(etudiant, updatedEtudiant);

        // Assert
        Assert.Equal("Nouveau email", etudiant.Courriel);
        Assert.Equal("Nouveau prenom", etudiant.Prenom);
        Assert.Equal("Nouveau nom", etudiant.Nom);
        Assert.Equal("Nouveau mot de passe", etudiant.MotDePasse);
        _mockEtudiantRepository.Verify(repo => repo.Update(etudiant), Times.Once);
        _mockEtudiantRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsListOfEtudiants()
    {
        // Arrange
        var etudiants = new List<Etudiant>
        {
            new Etudiant { Id = 1, Nom = "Etudiant 1" },
            new Etudiant { Id = 2, Nom = "Etudiant 2" }
        };

        _mockEtudiantRepository.Setup(repo => repo.GetAll<Etudiant>())
            .ReturnsAsync(etudiants);

        // Act
        var result = await _etudiantService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockEtudiantRepository.Verify(repo => repo.GetAll<Etudiant>(), Times.Once);
    }
}