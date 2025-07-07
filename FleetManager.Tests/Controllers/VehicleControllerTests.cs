using System.Security.Claims;
using Faker;
using FleetManager.Application.DTO;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Services;
using FleetManager.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FleetManager.Tests.Controller
{
    public class VehicleControllerTests
    {
        private readonly Mock<IVehicleService> _vehicleServiceMock;
        private readonly Mock<IChassiService> _chassiServiceMock;
        private readonly VehicleController _controller;

        public VehicleControllerTests()
        {
            _vehicleServiceMock = new Mock<IVehicleService>();
            _chassiServiceMock = new Mock<IChassiService>();
            _controller = new VehicleController(_vehicleServiceMock.Object, _chassiServiceMock.Object);
        }

        [Fact]
        public async Task CreateVehicle_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var dto = new CreateVehicleDto
            {
                ChassisNumber = (uint)new Random().Next(1, int.MaxValue),
                ChassisSeries = Name.First(),
                Type = "bus",
                Color = Name.First()
            };
            var chassi = new Chassi(dto.ChassisSeries, dto.ChassisNumber);
            var vehicle = new Bus(chassi, dto.Color);

            _chassiServiceMock.Setup(x => x.AddChassiAsync(dto.ChassisNumber, dto.ChassisSeries)).ReturnsAsync(chassi);
            _vehicleServiceMock.Setup(x => x.CreateVehicleAsync(chassi, dto.Type, dto.Color)).ReturnsAsync(vehicle);

            // Act
            var result = await _controller.CreateVehicle(dto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(vehicle, options => options
                .ComparingByMembers<Bus>()
                .ComparingByMembers<Chassi>());
        }

        [Fact]
        public async Task CreateVehicle_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var dto = new CreateVehicleDto
            {
                ChassisNumber = (uint)new Random().Next(1, int.MaxValue),
                ChassisSeries = Name.First(),
                Type = "bus",
                Color = Name.First()
            };
            _chassiServiceMock
                .Setup(x => x.AddChassiAsync(It.IsAny<uint>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("error"));

            // Act
            var result = await _controller.CreateVehicle(dto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.Value.Should().Be("error");
        }

        [Fact]
        public async Task GetVehicle_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var serie = Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);
            var chassi = new Chassi(serie, number);
            var vehicle = new Bus(chassi, Name.First());

            _vehicleServiceMock.Setup(x => x.GetByChassisIdAsync(serie, number)).ReturnsAsync(vehicle);

            // Act
            var result = await _controller.GetVehicle(serie, number);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(vehicle);
        }

        [Fact]
        public async Task GetVehicle_ShouldReturnNotFound_WhenExceptionThrown()
        {
            // Arrange
            var serie = Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);
            _vehicleServiceMock
                .Setup(x => x.GetByChassisIdAsync(serie, number))
                .ThrowsAsync(new Exception("not found"));

            // Simulate user claims
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(x => x.FindFirst("oid")).Returns((Claim)null);
            userMock.Setup(x => x.FindFirst("sub")).Returns(new Claim("sub", Guid.NewGuid().ToString()));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userMock.Object }
            };

            // Act
            var result = await _controller.GetVehicle(serie, number);

            // Assert
            var notFound = result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound!.Value.Should().Be("not found");
        }

        [Fact]
        public async Task UpdateVehicleColor_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var serie = Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);
            var dto = new UpdateVehicleColorDto { NewColor = Name.First() };
            var chassi = new Chassi(serie, number);
            var vehicle = new Bus(chassi, dto.NewColor);

            _vehicleServiceMock.Setup(x => x.UpdateVehicleColorAsync(serie, number, dto.NewColor)).ReturnsAsync(vehicle);

            // Act
            var result = await _controller.UpdateVehicleColor(serie, number, dto);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(vehicle);
        }

        [Fact]
        public async Task UpdateVehicleColor_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var serie = Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);
            var dto = new UpdateVehicleColorDto { NewColor = Name.First() };

            _vehicleServiceMock
                .Setup(x => x.UpdateVehicleColorAsync(serie, number, dto.NewColor))
                .ThrowsAsync(new Exception("bad color"));

            // Act
            var result = await _controller.UpdateVehicleColor(serie, number, dto);

            // Assert
            var badRequest = result as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.Value.Should().Be("bad color");
        }

        [Fact]
        public async Task GetAllVehicles_ShouldReturnOkWithVehicles()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Bus(new Chassi(Name.First(), 1), Name.First()),
                new Car(new Chassi(Name.First(), 2), Name.First())
            };
            _vehicleServiceMock.Setup(x => x.GetAllVehiclesAsync()).ReturnsAsync(vehicles);

            // Act
            var result = await _controller.GetAllVehicles();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(vehicles);
        }
    }
}