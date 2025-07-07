using FleetManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Domain.Services
{
    public interface IVehicleService
    {
        Task<Vehicle> CreateVehicleAsync(Chassi chassi, string type, string color);
        Task<Vehicle> UpdateVehicleColorAsync(string chassisSeries, uint chassisNumber, string newColor);
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle> GetByChassisIdAsync(string serie, uint number);
    }
}
