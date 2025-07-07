using FleetManager.Domain.Entities;
using FleetManager.Infrastructure.Services;
using FluentAssertions;
using FleetManager.Domain.Repositories;
using Moq;

namespace FleetManager.Tests.Services
{
    public class ChassiServiceTests
    {
        private readonly Mock<IChassiRepository> _chassiRepositoryMock;
        private readonly ChassiService _chassiService;

        public ChassiServiceTests()
        {
            _chassiRepositoryMock = new Mock<IChassiRepository>();
            _chassiService = new ChassiService(_chassiRepositoryMock.Object);
        }

        [Fact]
        public async Task AddChassiAsync_ShouldThrow_WhenSerieIsNullOrWhitespace()
        {
            // Arrange  
            var number = (uint)new Random().Next(1, int.MaxValue);
            string?[] invalidSeries = { null, "", "   " };

            foreach (var serie in invalidSeries)
            {
                // Act  
                Func<Task> act = async () => await _chassiService.AddChassiAsync(number, serie!);

                // Assert  
                await act.Should().ThrowAsync<ArgumentException>()
                    .WithMessage("*Serie must be provided.*")
                    .Where(e => e.ParamName == "serie");
            }
        }

        [Fact]
        public async Task AddChassiAsync_ShouldThrow_WhenNumberIsZero()
        {
            // Arrange  
            var serie = Faker.Name.First();
            uint number = 0;

            // Act  
            Func<Task> act = async () => await _chassiService.AddChassiAsync(number, serie);

            // Assert  
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Number must be greater than zero.*")
                .Where(e => e.ParamName == "number");
        }

        [Fact]
        public async Task AddChassiAsync_ShouldReturnExistingChassi_IfExists()
        {
            // Arrange  
            var number = (uint)new Random().Next(1, int.MaxValue);
            var serie = Faker.Name.First();
            var existingChassi = new Chassi(serie, number);

            _chassiRepositoryMock
                .Setup(r => r.GetBySeriesAndNumber(number, serie))
                .ReturnsAsync(existingChassi);

            // Act  
            var result = await _chassiService.AddChassiAsync(number, serie);

            // Assert  
            result.Should().BeSameAs(existingChassi);
            _chassiRepositoryMock.Verify(r => r.GetBySeriesAndNumber(number, serie), Times.Once);
            _chassiRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Chassi>()), Times.Never);
        }

        [Fact]
        public async Task AddChassiAsync_ShouldAddAndReturnChassi_IfNotExists()
        {
            // Arrange  
            var number = (uint)new Random().Next(1, int.MaxValue);
            var serie = Faker.Name.First();

            _chassiRepositoryMock
                .Setup(r => r.GetBySeriesAndNumber(number, serie))
                .ReturnsAsync((Chassi?)null);

            var createdChassi = new Chassi(serie, number);

            _chassiRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Chassi>()))
                .ReturnsAsync(createdChassi);

            // Act  
            var result = await _chassiService.AddChassiAsync(number, serie);

            // Assert  
            result.Should().BeSameAs(createdChassi);
            _chassiRepositoryMock.Verify(r => r.GetBySeriesAndNumber(number, serie), Times.Once);
            _chassiRepositoryMock.Verify(r => r.AddAsync(It.Is<Chassi>(c => c.Serie == serie && c.Number == number)), Times.Once);
        }

        [Fact]
        public async Task GetBySeriesAndNumberAsync_ShouldThrow_WhenSerieIsNullOrWhitespace()
        {
            // Arrange  
            var number = (uint)new Random().Next(1, int.MaxValue);
            string?[] invalidSeries = { null, "", "   " };

            foreach (var serie in invalidSeries)
            {
                // Act  
                Func<Task> act = async () => await _chassiService.GetBySeriesAndNumberAsync(number, serie!);

                // Assert  
                await act.Should().ThrowAsync<ArgumentException>()
                    .WithMessage("*Serie must be provided.*")
                    .Where(e => e.ParamName == "serie");
            }
        }

        [Fact]
        public async Task GetBySeriesAndNumberAsync_ShouldThrow_WhenNumberIsZero()
        {
            // Arrange  
            var serie = Faker.Name.First();
            uint number = 0;

            // Act  
            Func<Task> act = async () => await _chassiService.GetBySeriesAndNumberAsync(number, serie);

            // Assert  
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Number must be greater than zero.*")
                .Where(e => e.ParamName == "number");
        }

        [Fact]
        public async Task GetBySeriesAndNumberAsync_ShouldReturnChassi_IfExists()
        {
            // Arrange  
            var number = (uint)new Random().Next(1, int.MaxValue);
            var serie = Faker.Name.First();
            var chassi = new Chassi(serie, number);

            _chassiRepositoryMock
                .Setup(r => r.GetBySeriesAndNumber(number, serie))
                .ReturnsAsync(chassi);

            // Act  
            var result = await _chassiService.GetBySeriesAndNumberAsync(number, serie);

            // Assert  
            result.Should().BeSameAs(chassi);
            _chassiRepositoryMock.Verify(r => r.GetBySeriesAndNumber(number, serie), Times.Once);
        }

        [Fact]
        public async Task GetBySeriesAndNumberAsync_ShouldReturnNull_IfNotExists()
        {
            // Arrange  
            var number = (uint)new Random().Next(1, int.MaxValue);
            var serie = Faker.Name.First();

            _chassiRepositoryMock
                .Setup(r => r.GetBySeriesAndNumber(number, serie))
                .ReturnsAsync((Chassi?)null);

            // Act  
            var result = await _chassiService.GetBySeriesAndNumberAsync(number, serie);

            // Assert  
            result.Should().BeNull();
            _chassiRepositoryMock.Verify(r => r.GetBySeriesAndNumber(number, serie), Times.Once);
        }
    }
}