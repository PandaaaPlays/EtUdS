using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using etuds.Server.Services.Professeur;
using Moq;

namespace EtUdS.Tests.Services;

public class ProfesseurServiceTests
{
    private readonly Mock<IBaseRepository> _mockRepository;
    private readonly ProfesseurService _professeurService;

    public ProfesseurServiceTests()
    {
        _mockRepository = new Mock<IBaseRepository>();

        _professeurService = new ProfesseurService(
            _mockRepository.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsProfesseur_IfExists()
    {
        // Arrange
        var professeur = new Professeur { Id = 1, Prenom = "Test", Nom = "Test", Courriel = "Courriel"};
        _mockRepository.Setup(repo => repo.GetById<Professeur>(1))
            .ReturnsAsync(professeur);

        // Act
        var result = await _professeurService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", result.Prenom);
        Assert.Equal("Test", result.Nom);
        Assert.Equal("Courriel", result.Courriel);
    }
    
}