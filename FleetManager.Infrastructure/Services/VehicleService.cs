using FleetManager.Domain.Entities;
using FleetManager.Domain.Repositories;
using FleetManager.Domain.Services;


namespace FleetManager.Infrastructure.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IChassiService _chassiService;

        public VehicleService(IVehicleRepository vehicleRepository, IChassiService chassiService)
        {
            _vehicleRepository = vehicleRepository;
            _chassiService = chassiService;
        }

        public async Task<Vehicle> CreateVehicleAsync(Chassi chassi, string type, string color)
        {

            if (await _vehicleRepository.GetByChassisIdAsync(chassi) != null)
            {
                throw new InvalidOperationException($"Veículo com chassi '{chassi.Serie}-{chassi.Number}' já existe.");
            }

            Vehicle newVehicle = type.ToLower() switch
            {
                "bus" => new Bus(chassi, color),
                "truck" => new Truck(chassi, color),
                "car" => new Car(chassi, color),
                _ => throw new ArgumentException("Tipo de veículo inválido.", nameof(type))
            };

            await _vehicleRepository.AddAsync(newVehicle);
            return newVehicle;
        }

        public async Task<Vehicle> UpdateVehicleColorAsync(string chassisSeries, uint chassisNumber, string newColor)
        {
            var vehicle = await GetByChassisIdAsync(chassisSeries, chassisNumber);
            if (string.IsNullOrWhiteSpace(newColor))
                throw new ArgumentException("Color must be provided.", nameof(newColor));

            if (vehicle == null)
            {
                throw new InvalidOperationException($"Vehicle with chassis '{chassisSeries}-{chassisNumber}' does not exist.");
            }

            vehicle.Color = newColor;
            await _vehicleRepository.UpdateAsync(vehicle);

            return vehicle;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return await _vehicleRepository.GetAllAsync();
        }

        public async Task<Vehicle> GetByChassisIdAsync(string serie, uint number)
        {
            var chassi = await _chassiService.GetBySeriesAndNumberAsync(number, serie);
            if (chassi == null)
            {
                throw new InvalidOperationException($"Chassi '{serie}-{number}' does not exist.");
            }
            var vehicle = await _vehicleRepository.GetByChassisIdAsync(chassi);
            if (vehicle == null)
            {
                throw new InvalidOperationException($"Vehicle with chassi '{serie}-{number}' does not exist.");
            }
            return vehicle;
        }
    }
}
