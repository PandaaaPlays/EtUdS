using etuds.Server.Entities;
using EtUdS.Server.Repositories;
using etuds.Server.Services.Seance;
using Moq;

namespace EtUdS.Tests.Services;

public class SeanceServiceTests
{
    private readonly SeanceService _seanceService;
    private readonly Mock<IBaseRepository> _mockRepository;

    public SeanceServiceTests()
    {
        _mockRepository = new Mock<IBaseRepository>();

        _seanceService = new SeanceService(
            _mockRepository.Object
        );
    }

    [Fact]
    public async Task GetById_ReturnsSeance_IfExists()
    {
        // Arrange
        var seance = new Seance
        {
            Id = 1, IdCours = 1, Laboratoire = false, Local = "Test",
            DebutSeance = new DateTime(2000, 1, 1),
            FinSeance = new DateTime(2001, 1, 1)
        };
        _mockRepository.Setup(repo => repo.GetById<Seance>(1))
            .ReturnsAsync(seance);

        // Act
        var result = await _seanceService.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.IdCours);
        Assert.False(result.Laboratoire);
        Assert.Equal("Test", result.Local);
        _mockRepository.Verify(repo => repo.GetById<Seance>(1), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsNull_IfSeanceNotExists()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetById<Seance>(1))
            .ReturnsAsync((Seance)null);

        // Act
        var result = await _seanceService.GetById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Add_CallsRepository_AddsSeance()
    {
        // Arrange
        var seance = new Seance { Id = 1, IdCours = 1 };
        _mockRepository.Setup(repo => repo.Add(It.IsAny<Seance>()))
            .ReturnsAsync(seance);

        // Act
        var result = await _seanceService.Add(seance);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.IdCours);
        _mockRepository.Verify(repo => repo.Add(It.IsAny<Seance>()), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task AddSerie_CallsRepository_AddsSeanceMultiple()
    {
        // Arrange
        var seance = new Seance { Id = 1, IdCours = 1, Laboratoire = false, Local = "Local" };

        // Act
        var result = await _seanceService.AddSerie(seance, new DateTime(2000, 1, 1), new DateTime(2000, 12, 31));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(52, result.Count());
    }
    
    [Fact]
    public async Task Delete_CallsRepository_DeleteMethod()
    {
        // Arrange
        var seance = new Seance { Id = 1, IdCours = 1 };
        _mockRepository.Setup(repo => repo.Delete(seance));

        // Act
        await _seanceService.Delete(seance);

        // Assert
        _mockRepository.Verify(repo => repo.Delete(seance), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public async Task Update_CallsRepository_UpdateMethod()
    {
        // Arrange
        var seance = new Seance
        {
            Id = 1, IdCours = 1, Laboratoire = false, Local = "Test",
            DebutSeance = new DateTime(2000, 1, 1),
            FinSeance = new DateTime(2001, 1, 1)
        };
        var updatedSeance = new Seance
        {
            Id = 1, IdCours = 2, Laboratoire = true, Local = "Test Update",
            DebutSeance = new DateTime(2002, 1, 1),
            FinSeance = new DateTime(2003, 1, 1)
        };
        _mockRepository.Setup(repo => repo.Update(seance));

        // Act
        await _seanceService.Update(seance, updatedSeance);

        // Assert
        Assert.True(seance.Laboratoire);
        Assert.Equal(2, seance.IdCours);
        Assert.Equal("Test Update", seance.Local);
        Assert.Equal(2002, seance.DebutSeance.Year);
        Assert.Equal(2003, seance.FinSeance.Year);
        _mockRepository.Verify(repo => repo.Update(seance), Times.Once);
        _mockRepository.Verify(repo => repo.Save(), Times.Once);
    }
    
    [Fact]
    public async Task GetAll_ReturnsListOfSeance()
    {
        // Arrange
        var seance = new List<Seance>
        {
            new Seance { Id = 1, Local = "Local 1" },
            new Seance { Id = 2, Local = "Local 2" }
        };

        _mockRepository.Setup(repo => repo.GetAll<Seance>())
            .ReturnsAsync(seance);

        // Act
        var result = await _seanceService.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(repo => repo.GetAll<Seance>(), Times.Once);
    }
}