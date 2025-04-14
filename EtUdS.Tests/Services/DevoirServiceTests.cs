using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using EtUdS.Server.Repositories.Equipe;
using etuds.Server.Repositories.Etudiant;
using EtUdS.Server.Repositories.Membre;
using etuds.Server.Services.Devoir;
using Moq;

namespace EtUdS.Tests.Services;

public class DevoirServiceTests
{
    private readonly Mock<IBaseRepository> _mockRepository;
    private readonly Mock<IEtudiantRepository> _mockEtudiantRepository;
    private readonly Mock<IMembreRepository> _mockMembreRepository;
    private readonly Mock<IEquipeRepository> _mockEquipeRepository;
    private readonly DevoirService _devoirService;

    public DevoirServiceTests()
    {
        _mockRepository = new Mock<IBaseRepository>();
        _mockEtudiantRepository = new Mock<IEtudiantRepository>();
        _mockMembreRepository = new Mock<IMembreRepository>();
        _mockEquipeRepository = new Mock<IEquipeRepository>();

        _devoirService = new DevoirService(
            _mockRepository.Object,
            _mockEtudiantRepository.Object,
            _mockMembreRepository.Object,
            _mockEquipeRepository.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsDevoir_IfExists()
    {
        // Arrange
        var devoir = new Devoir { Id = 1, IdCours = 1, Nom = "Test Devoir", DateLimiteSoumission = new DateTime(2025, 01, 01), TailleMaxEquipes = 2 };
        _mockRepository.Setup(repo => repo.GetById<Devoir>(1))
            .ReturnsAsync(devoir);

        // Act
        var result = await _devoirService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.IdCours);
        Assert.Equal("Test Devoir", result.Nom);
        Assert.Equal(new DateTime(2025, 01, 01), result.DateLimiteSoumission);
        Assert.Equal(2, result.TailleMaxEquipes);
        _mockRepository.Verify(repo => repo.GetById<Devoir>(1), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNull_IfDevoirNotExists()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetById<Devoir>(1))
            .ReturnsAsync((Devoir)null);

        // Act
        var result = await _devoirService.GetById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_Updates_Etat_IfDevoirExists()
    {
        // Arrange
        var devoir = new Devoir { Id = 1, IdCours = 1, Nom = "Test Devoir", DateLimiteSoumission = new DateTime(2025, 01, 01), TailleMaxEquipes = 2 };
        var equipeDejaEvalue = new Equipe { Id = 1, IdDevoir = 1, EtatSoumission = EtatSoumission.Evalue, Note = 95 };
        var equipeEvalue = new Equipe { Id = 2, IdDevoir = 1, EtatSoumission = EtatSoumission.AFaire, Note = 95 };
        var equipeEnRetard = new Equipe { Id = 3, IdDevoir = 1, EtatSoumission = EtatSoumission.AFaire, Note = null };
        _mockRepository.Setup(repo => repo.GetById<Devoir>(1))
            .ReturnsAsync(devoir);
        _mockEquipeRepository.Setup(repo => repo.GetAll<Equipe>())
            .ReturnsAsync(new List<Equipe>() { equipeDejaEvalue, equipeEvalue, equipeEnRetard });

        // Act
        _ = await _devoirService.GetById(1);

        // Assert
        Assert.Equal(EtatSoumission.Evalue, equipeDejaEvalue.EtatSoumission);
        Assert.Equal(EtatSoumission.EnRetard, equipeEnRetard.EtatSoumission);
        Assert.Equal(EtatSoumission.Evalue, equipeEvalue.EtatSoumission);
    }

    [Fact]
    public async Task GetByIdAvecDetails_ReturnsDevoir_IfExists()
    {
        // Arrange
        var equipe = new Equipe { Id = 1, IdDevoir = 1, EtatSoumission = EtatSoumission.Evalue, Note = 95 };
        var cours = new Cours { Id = 1, Sigle = "IFT123", Titre = "Test" };
        var devoir = new Devoir { Id = 1, IdCours = 1, Nom = "Devoir" };
        var membre = new Membre { Id = 1, IdEquipe = 1, IdEtudiant = 1, Accepte = true };
        
        _mockEquipeRepository.Setup(repo => repo.GetAll<Equipe>())
            .ReturnsAsync(new List<Equipe> { equipe });
        _mockRepository.Setup(repo => repo.GetById<Devoir>(1))
            .ReturnsAsync(devoir);
        _mockRepository.Setup(repo => repo.GetById<Cours>(1))
            .ReturnsAsync(cours);
        _mockMembreRepository.Setup(repo => repo.GetAllMembresByEquipeId(1))
            .ReturnsAsync(new List<Membre> { membre });

        // Act
        var result = await _devoirService.GetByIdAvecDetails(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Devoir", result.Nom);
        Assert.Equal("IFT123", result.Sigle);
        Assert.Equal(95, result.Note);
    }

    /* This doesnt work...
    [Fact]
    public async Task GetEtudiantsDisponiblePourDevoir_ReturnsEtudiants()
    {
        // Arrange
        var devoir = new Devoir { Id = 1 };
        var etudiant = new Etudiant { Id = 1 };
        var inscription = new Inscription { Id = 1, IdCours = 1, IdEtudiant = 1 };
        var etudiantNonDispo = new Etudiant { Id = 2 }; 
        var inscriptionNonDispo = new Inscription { Id = 2, IdCours = 1, IdEtudiant = 2 };
        var etudiantPasDansCours = new Etudiant { Id = 3 }; 
        var inscriptionPasDansCours = new Inscription { Id = 3, IdCours = 2, IdEtudiant = 3 };
        _mockRepository.Setup(repo => repo.GetById<Devoir>(1))
            .ReturnsAsync(devoir);
        _mockRepository.Setup(repo => repo.GetAll<Inscription>())
            .ReturnsAsync(new List<Inscription> { inscription, inscriptionNonDispo, inscriptionPasDansCours });
        _mockRepository.Setup(repo => repo.GetById<Etudiant>(1))
            .ReturnsAsync(etudiant);
        _mockRepository.Setup(repo => repo.GetById<Etudiant>(2))
            .ReturnsAsync(etudiantNonDispo);
        _mockRepository.Setup(repo => repo.GetById<Etudiant>(3))
            .ReturnsAsync(etudiantPasDansCours);
        _mockMembreRepository.Setup(repo => repo.GetIdEtudiantsNonDisponiblesPourDevoir(1))
            .ReturnsAsync(new List<int> { 2, 3 });
        
        // Act
        var result = await _devoirService.GetEtudiantsDisponiblePourDevoir(1);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(result, r => r.Id == 1); // Assuming student with Id 1 should be available
        Assert.DoesNotContain(result, r => r.Id == 2 && r.Id == 3);
    }*/

    [Fact]
    public async Task Add_CallsRepository_AddsDevoir()
    {
        // Arrange
        var devoir = new Devoir { Id = 1, Nom = "Devoir", IdCours = 1 };
        _mockRepository.Setup(repo => repo.Add(It.IsAny<Devoir>()))
            .ReturnsAsync(devoir);

        // Act
        var result = await _devoirService.Add(devoir);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Devoir", result.Nom);
        _mockRepository.Verify(repo => repo.Add(It.IsAny<Devoir>()), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Delete_CallsRepository_DeleteMethod()
    {
        // Arrange
        var devoir = new Devoir { Id = 1, Nom = "Devoir", IdCours = 1 };
        _mockRepository.Setup(repo => repo.Delete(devoir));

        // Act
        await _devoirService.Delete(devoir);

        // Assert
        _mockRepository.Verify(repo => repo.Delete(devoir), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Update_CallsRepository_UpdateMethod()
    {
        // Arrange
        var devoir = new Devoir { Id = 1, Nom = "Ancien nom", IdCours = 1 };
        var updatedDevoir = new Devoir { Id = 1, Nom = "Nouveau nom", IdCours = 2 };
        _mockRepository.Setup(repo => repo.Update(devoir));

        // Act
        await _devoirService.Update(devoir, updatedDevoir);

        // Assert
        Assert.Equal("Nouveau nom", devoir.Nom);
        Assert.Equal(2, devoir.IdCours);
        _mockRepository.Verify(repo => repo.Update(devoir), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsListOfDevoirs()
    {
        // Arrange
        var devoirs = new List<Devoir>
        {
            new Devoir { Id = 1, Nom = "Devoir 1", IdCours = 1 },
            new Devoir { Id = 2, Nom = "Devoir 2", IdCours = 2 }
        };

        _mockRepository.Setup(repo => repo.GetAll<Devoir>())
            .ReturnsAsync(devoirs);

        // Act
        var result = await _devoirService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(repo => repo.GetAll<Devoir>(), Times.Once);
    }
    
    [Fact]
    public async Task GetMembresIdEtudiant_ReturnsListOfMembres()
    {
        // Arrange
        var equipes = new List<Equipe>
        {
            new Equipe { Id = 1, IdDevoir = 1 },
            new Equipe { Id = 2, IdDevoir = 1 },
            new Equipe { Id = 3, IdDevoir = 2 }
        };
        
        var membres = new List<Membre>
        {
            new Membre { Id = 1, IdEtudiant = 1, IdEquipe = 1, Accepte = false },
            new Membre { Id = 2, IdEtudiant = 2, IdEquipe = 1, Accepte = false },
            new Membre { Id = 3, IdEtudiant = 2, IdEquipe = 2, Accepte = false }
        };

        _mockEquipeRepository.Setup(repo => repo.GetAll<Equipe>())
            .ReturnsAsync(equipes);
        _mockMembreRepository.Setup(repo => repo.GetAllMembresByEquipeId(1))
            .ReturnsAsync(membres.Where(x => x.Id == 1 || x.Id == 2));

        // Act
        var result = await _devoirService.GetMembresIdEtudiant(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Count());
    }
}