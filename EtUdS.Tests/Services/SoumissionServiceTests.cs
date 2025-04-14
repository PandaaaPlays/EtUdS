using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using etuds.Server.Services.Devoir;
using etuds.Server.Services.Equipe;
using etuds.Server.Services.Membre;
using etuds.Server.Services.Soumission;
using Moq;

namespace EtUdS.Tests.Services;

public class SoumissionServiceTests
{
    
    private readonly Mock<IBaseRepository> _mockRepository;
    private readonly Mock<IDevoirService> _mockDevoirService;
    private readonly Mock<IEquipeService> _mockEquipeService;
    private readonly Mock<IMembreService> _mockMembreService;
    private readonly SoumissionService _soumissionService;

    public SoumissionServiceTests()
    {
        _mockRepository = new Mock<IBaseRepository>();
        _mockDevoirService = new Mock<IDevoirService>();
        _mockEquipeService = new Mock<IEquipeService>();
        _mockMembreService = new Mock<IMembreService>();

        _soumissionService = new SoumissionService(
            _mockRepository.Object,
            _mockDevoirService.Object,
            _mockEquipeService.Object,
            _mockMembreService.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsSoumission_IfExists()
    {
        // Arrange
        var soumission = new Soumission { Id = 1, IdEquipe = 1 };
        _mockRepository.Setup(repo => repo.GetById<Soumission>(1))
            .ReturnsAsync(soumission);

        // Act
        var result = await _soumissionService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }
    
    [Fact]
    public async Task GetById_ReturnsNull_IfSoumissionNotExists()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetById<Soumission>(1))
            .ReturnsAsync((Soumission)null);

        // Act
        var result = await _soumissionService.GetById(1);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetAll_ReturnsListOfSoumission()
    {
        // Arrange
        var soumissions = new List<Soumission>
        {
            new Soumission { Id = 1 },
            new Soumission { Id = 2 }
        };

        _mockRepository.Setup(repo => repo.GetAll<Soumission>())
            .ReturnsAsync(soumissions);

        // Act
        var result = await _soumissionService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(repo => repo.GetAll<Soumission>(), Times.Once);
    }
    
    [Fact]
    public async Task GetByEquipeDevoir_ReturnsListOfSoumission_OfEquipeOnly()
    {
        // Arrange
        var soumissions = new List<Soumission>
        {
            new Soumission { Id = 1, IdEquipe = 1 },
            new Soumission { Id = 2, IdEquipe = 2 },
            new Soumission { Id = 3, IdEquipe = 1 }
        };
        var equipe = new Equipe { Id = 1, IdDevoir = 1 };

        _mockRepository.Setup(repo => repo.GetAll<Soumission>())
            .ReturnsAsync(soumissions);
        _mockDevoirService.Setup(s => s.GetEquipeOfCurrentEtudiantPourDevoir(1, 1))
            .ReturnsAsync(equipe);

        // Act
        var result = await _soumissionService.GetByEquipeDevoir(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(repo => repo.GetAll<Soumission>(), Times.Once);
    }
    
    [Fact]
    public async Task DownloadById_ReturnsDownload_IfExists()
    {
        // Arrange
        var soumission = new Soumission { Id = 1, IdEquipe = 1, NomFichier = "Test.txt", NomFichierCorrection = "Test_Corr.txt" };
        var equipe = new Equipe { Id = 1, IdDevoir = 1 };
        var membre = new Membre { Id = 1, IdEquipe = 1, IdEtudiant = 1, Accepte = true };

        string tempPath = Path.Combine(Path.GetTempPath(), "soumissions", $"devoir-{equipe.IdDevoir}", $"equipe-{equipe.Id}");
        Directory.CreateDirectory(tempPath); 

        var filePath = Path.Combine(tempPath, "Test.txt");
        var fileContent = new byte[] { 1, 2, 3, 4, 5 };
        File.WriteAllBytes(filePath, fileContent);
        
        _mockRepository.Setup(repo => repo.GetById<Soumission>(1)).ReturnsAsync(soumission);
        _mockEquipeService.Setup(s => s.GetById(1)).ReturnsAsync(equipe);
        _mockMembreService.Setup(s => s.GetAllMembresByEquipeId(1)).ReturnsAsync(new[] { membre });

        string originalDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(Path.GetTempPath());

        try
        {
            // Act
            var result = await _soumissionService.DownloadById(1, 1, false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileContent, result.Value.Item1);
            Assert.Equal("application/octet-stream", result.Value.Item2);
            Assert.Equal("Test.txt", result.Value.Item3);
        }
        finally
        {
            // Cleanup
            File.Delete(filePath);
            Directory.SetCurrentDirectory(originalDirectory);
        }
    }

    [Fact]
    public async Task DownloadById_DeletesSoumission_IfFileNotExists()
    {
        // Arrange
        var soumission = new Soumission { Id = 1, IdEquipe = 1, NomFichier = "Test.txt", NomFichierCorrection = "Test_Corr.txt" };
        var equipe = new Equipe { Id = 1, IdDevoir = 1 };
        var membre = new Membre { Id = 1, IdEquipe = 1, IdEtudiant = 1, Accepte = true };
        
        _mockRepository.Setup(repo => repo.GetById<Soumission>(1)).ReturnsAsync(soumission);
        _mockEquipeService.Setup(s => s.GetById(1)).ReturnsAsync(equipe);
        _mockMembreService.Setup(s => s.GetAllMembresByEquipeId(1)).ReturnsAsync(new[] { membre });
        
        // Act
        var result = await _soumissionService.DownloadById(1, 1, false);
        
        // Assert
        Assert.Null(result);
        _mockRepository.Verify(repo => repo.Delete(soumission), Times.Once);
        
    }
}