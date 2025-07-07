using Faker;
using FleetManager.Domain.Entities;
using FleetManager.Domain.Repositories;
using FleetManager.Domain.Services;
using FleetManager.Infrastructure.Services;
using FluentAssertions;
using Moq;

namespace FleetManager.Tests.Services
{
    public class VehicleServiceTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IChassiService> _chassiServiceMock;
        private readonly VehicleService _vehicleService;

        public VehicleServiceTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _chassiServiceMock = new Mock<IChassiService>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _chassiServiceMock.Object);
        }

        [Theory]
        [InlineData("bus")]
        [InlineData("BUS")]
        [InlineData("truck")]
        [InlineData("car")]
        public async Task CreateVehicleAsync_ShouldCreateVehicle_WhenTypeIsValidAndNotExists(string type)
        {
            // Arrange
            var chassi = new Chassi(Name.First(), (uint)new Random().Next(1, int.MaxValue));
            var color = Name.First();
            _vehicleRepositoryMock.Setup(r => r.GetByChassisIdAsync(chassi)).ReturnsAsync((Vehicle?)null);

            // Act
            var result = await _vehicleService.CreateVehicleAsync(chassi, type, color);

            // Assert
            result.Should().NotBeNull();
            result.Chassi.Should().Be(chassi);
            result.Color.Should().Be(color);
            switch (type.ToLower())
            {
                case "bus": result.Should().BeOfType<Bus>(); break;
                case "truck": result.Should().BeOfType<Truck>(); break;
                case "car": result.Should().BeOfType<Car>(); break;
            }
            _vehicleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Once);
        }

        [Fact]
        public async Task CreateVehicleAsync_ShouldThrow_WhenVehicleAlreadyExists()
        {
            // Arrange
            var chassi = new Chassi(Name.First(), (uint)new Random().Next(1, int.MaxValue));
            var color = Name.First();
            var existingVehicle = new Bus(chassi, color);
            _vehicleRepositoryMock.Setup(r => r.GetByChassisIdAsync(chassi)).ReturnsAsync(existingVehicle);

            // Act
            Func<Task> act = async () => await _vehicleService.CreateVehicleAsync(chassi, "bus", color);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Veículo com chassi '{chassi.Serie}-{chassi.Number}' já existe.");
        }

        [Fact]
        public async Task CreateVehicleAsync_ShouldThrow_WhenTypeIsInvalid()
        {
            // Arrange
            var chassi = new Chassi(Name.First(), (uint)new Random().Next(1, int.MaxValue));
            var color = Name.First();
            _vehicleRepositoryMock.Setup(r => r.GetByChassisIdAsync(chassi)).ReturnsAsync((Vehicle?)null);

            // Act
            Func<Task> act = async () => await _vehicleService.CreateVehicleAsync(chassi, "plane", color);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Tipo de veículo inválido.*")
                .Where(e => e.ParamName == "type");
        }

        [Fact]
        public async Task UpdateVehicleColorAsync_ShouldUpdateColor_WhenVehicleExists()
        {
            // Arrange
            var serie = Faker.Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);
            var oldColor = Name.First();
            var newColor = Name.First();
            var chassi = new Chassi(serie, number);
            var vehicle = new Bus(chassi, oldColor);

            _chassiServiceMock.Setup(s => s.GetBySeriesAndNumberAsync(number, serie)).ReturnsAsync(chassi);
            _vehicleRepositoryMock.Setup(r => r.GetByChassisIdAsync(chassi)).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleService.UpdateVehicleColorAsync(serie, number, newColor);

            // Assert
            result.Should().Be(vehicle);
            result.Color.Should().Be(newColor);
            _vehicleRepositoryMock.Verify(r => r.UpdateAsync(vehicle), Times.Once);
        }

        [Fact]
        public async Task UpdateVehicleColorAsync_ShouldThrow_WhenColorIsNullOrWhitespace()
        {
            // Arrange
            var serie = Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);
            var chassi = new Chassi(serie, number);
            var vehicle = new Bus(chassi, Name.First());

            _chassiServiceMock.Setup(s => s.GetBySeriesAndNumberAsync(number, serie)).ReturnsAsync(chassi);
            _vehicleRepositoryMock.Setup(r => r.GetByChassisIdAsync(chassi)).ReturnsAsync(vehicle);

            foreach (var invalidColor in new[] { null, "", "   " })
            {
                // Act
                Func<Task> act = async () => await _vehicleService.UpdateVehicleColorAsync(serie, number, invalidColor!);

                // Assert
                await act.Should().ThrowAsync<ArgumentException>()
                    .WithMessage("Color must be provided.*")
                    .Where(e => e.ParamName == "newColor");
            }
        }

        [Fact]
        public async Task UpdateVehicleColorAsync_ShouldThrow_WhenVehicleDoesNotExist()
        {
            // Arrange
            var serie = Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);
            var chassi = new Chassi(serie, number);

            _chassiServiceMock.Setup(s => s.GetBySeriesAndNumberAsync(number, serie)).ReturnsAsync(chassi);
            _vehicleRepositoryMock.Setup(r => r.GetByChassisIdAsync(chassi)).ReturnsAsync((Vehicle?)null);

            // Act
            Func<Task> act = async () => await _vehicleService.UpdateVehicleColorAsync(serie, number, Name.First());

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Vehicle with chassi '{serie}-{number}' does not exist.");
        }

        [Fact]
        public async Task GetAllVehiclesAsync_ShouldReturnAllVehicles()
        {
            // Arrange
            var vehicles = new List<Vehicle>
            {
                new Bus(new Chassi(Name.First(), 1), Name.First()),
                new Car(new Chassi(Name.First(), 1), Name.First())
            };
            _vehicleRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(vehicles);

            // Act
            var result = await _vehicleService.GetAllVehiclesAsync();

            // Assert
            result.Should().BeEquivalentTo(vehicles);
        }

        [Fact]
        public async Task GetByChassisIdAsync_ShouldReturnVehicle_WhenExists()
        {
            // Arrange
            var serie = Faker.Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);
            var chassi = new Chassi(serie, number);
            var vehicle = new Bus(chassi, Name.First());

            _chassiServiceMock.Setup(s => s.GetBySeriesAndNumberAsync(number, serie)).ReturnsAsync(chassi);
            _vehicleRepositoryMock.Setup(r => r.GetByChassisIdAsync(chassi)).ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleService.GetByChassisIdAsync(serie, number);

            // Assert
            result.Should().Be(vehicle);
        }

        [Fact]
        public async Task GetByChassisIdAsync_ShouldThrow_WhenChassiDoesNotExist()
        {
            // Arrange
            var serie = Faker.Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);

            _chassiServiceMock.Setup(s => s.GetBySeriesAndNumberAsync(number, serie)).ReturnsAsync((Chassi?)null);

            // Act
            Func<Task> act = async () => await _vehicleService.GetByChassisIdAsync(serie, number);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Chassi '{serie}-{number}' does not exist.");
        }

        [Fact]
        public async Task GetByChassisIdAsync_ShouldThrow_WhenVehicleDoesNotExist()
        {
            // Arrange
            var serie = Faker.Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);
            var chassi = new Chassi(serie, number);

            _chassiServiceMock.Setup(s => s.GetBySeriesAndNumberAsync(number, serie)).ReturnsAsync(chassi);
            _vehicleRepositoryMock.Setup(r => r.GetByChassisIdAsync(chassi)).ReturnsAsync((Vehicle?)null);

            // Act
            Func<Task> act = async () => await _vehicleService.GetByChassisIdAsync(serie, number);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Vehicle with chassi '{serie}-{number}' does not exist.");
        }

        [Fact]
        public async Task UpdateVehicleColorAsync_ShouldThrow_WhenVehicleIsNull()
        {
            // Arrange
            var serie = Name.First();
            var number = (uint)new Random().Next(1, int.MaxValue);
            var chassi = new Chassi(serie, number);

            _chassiServiceMock.Setup(s => s.GetBySeriesAndNumberAsync(number, serie)).ReturnsAsync(chassi);
            _vehicleRepositoryMock.Setup(r => r.GetByChassisIdAsync(chassi)).ReturnsAsync((Vehicle?)null);

            // Act
            Func<Task> act = async () => await _vehicleService.UpdateVehicleColorAsync(serie, number, Name.First());

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"Vehicle with chassi '{serie}-{number}' does not exist.");
        }
    }
}