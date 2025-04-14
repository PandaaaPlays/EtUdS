using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using etuds.Server.Services.Devoir;
using etuds.Server.Services.Document;
using etuds.Server.Services.Equipe;
using etuds.Server.Services.Membre;
using Moq;

namespace EtUdS.Tests.Services;

public class DocumentServiceTests
{
    private readonly Mock<IBaseRepository> _mockRepository;
    private readonly Mock<IDevoirService> _mockDevoirService;
    private readonly Mock<IMembreService> _mockMembreService;
    private readonly Mock<IEquipeService> _mockEquipeService;
    private readonly DocumentService _documentService;

    public DocumentServiceTests()
    {
        _mockRepository = new Mock<IBaseRepository>();
        _mockDevoirService = new Mock<IDevoirService>();
        _mockEquipeService = new Mock<IEquipeService>();
        _mockMembreService = new Mock<IMembreService>();

        _documentService = new DocumentService(
            _mockRepository.Object,
            _mockDevoirService.Object,
            _mockMembreService.Object,
            _mockEquipeService.Object
            
        );
    }

    [Fact]
    public async Task GetById_ReturnsDocument_IfExists()
    {
        // Arrange
        var document = new Document { Id = 1, NomFichier = "Document" };
        _mockRepository.Setup(repo => repo.GetById<Document>(1))
            .ReturnsAsync(document);

        // Act
        var result = await _documentService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Document", result.NomFichier);
        _mockRepository.Verify(repo => repo.GetById<Document>(1), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNull_IfDocumentNotExists()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetById<Document>(1))
            .ReturnsAsync((Document)null);

        // Act
        var result = await _documentService.GetById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Add_CallsRepository_AddsDocument()
    {
        // Arrange
        var document = new Document { Id = 1, NomFichier = "Document" };
        _mockRepository.Setup(repo => repo.Add(It.IsAny<Document>()))
            .ReturnsAsync(document);

        // Act
        var result = await _documentService.Add(document);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Document", result.NomFichier);
        _mockRepository.Verify(repo => repo.Add(It.IsAny<Document>()), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Exactly(1));
    }
    
    [Fact]
    public async Task Delete_CallsRepository_DeleteMethod()
    {
        // Arrange
        var document = new Document { Id = 1, IdMembre = 1, IdEquipe = 1, NomFichier = "Document" };
        var membre = new Membre { Id = 1, IdEtudiant = 1, IdEquipe = 1 };
        var equipe = new Equipe { Id = 1, IdDevoir = 1, EtatSoumission = EtatSoumission.Evalue, Note = 95 };
        _mockRepository.Setup(repo => repo.Delete(document));
        _mockRepository.Setup(repo => repo.GetById<Document>(1))
            .ReturnsAsync(document);
        _mockMembreService.Setup(s => s.GetById(1))
            .ReturnsAsync(membre);
        _mockEquipeService.Setup(s => s.GetById(1))
            .ReturnsAsync(equipe);

        // Act
        await _documentService.Delete(1, 1);

        // Assert
        _mockRepository.Verify(repo => repo.Delete(document), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Update_CallsRepository_UpdateMethod()
    {
        // Arrange
        var document = new Document { Id = 1, NomFichier = "Document" };
        var updatedDocument = new Document { Id = 1, NomFichier = "Document update" };
        _mockRepository.Setup(repo => repo.Update(document));

        // Act
        await _documentService.Update(document, updatedDocument);

        // Assert
        Assert.Equal("Document update", document.NomFichier);
        _mockRepository.Verify(repo => repo.Update(document), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }
    
    [Fact]
    public async Task DownloadById_ReturnsDownload_IfExists()
    {
        // Arrange
        var document = new Document { Id = 1, IdEquipe = 1, IdMembre = 1, NomFichier = "Test.txt" };
        var equipe = new Equipe { Id = 1, IdDevoir = 1 };
        var membre = new Membre { Id = 1, IdEquipe = 1, IdEtudiant = 1, Accepte = true };

        string tempPath = Path.Combine(Path.GetTempPath(), "devoirs", $"devoir-{equipe.IdDevoir}", $"equipe-{equipe.Id}");
        Directory.CreateDirectory(tempPath); 

        var filePath = Path.Combine(tempPath, "Test.txt");
        var fileContent = new byte[] { 1, 2, 3, 4, 5 };
        File.WriteAllBytes(filePath, fileContent);
        
        _mockRepository.Setup(repo => repo.GetById<Document>(1)).ReturnsAsync(document);
        _mockEquipeService.Setup(s => s.GetById(1)).ReturnsAsync(equipe);
        _mockMembreService.Setup(s => s.GetAllMembresByEquipeId(1)).ReturnsAsync(new[] { membre });

        string originalDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(Path.GetTempPath());

        try
        {
            // Act
            var result = await _documentService.DownloadById(1, 1);

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
}