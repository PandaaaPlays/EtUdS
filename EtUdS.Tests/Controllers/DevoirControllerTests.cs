using etuds.Server.Controllers;
using etuds.Server.Entities;
using etuds.Server.Services.Devoir;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EtUdS.Tests.Controllers;

public class DevoirControllerTests
{
    private readonly Mock<IDevoirService> _mockService;
    private readonly DevoirController _controller;

    public DevoirControllerTests()
    {
        _mockService = new Mock<IDevoirService>();
        _controller = new DevoirController(_mockService.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenDevoirDoesNotExist()
    {
        _mockService.Setup(s => s.GetById(It.IsAny<int>())).ReturnsAsync((Devoir)null);

        var result = await _controller.GetById(1);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}