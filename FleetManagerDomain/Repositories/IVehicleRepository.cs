using FleetManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Domain.Repositories
{
    public interface IVehicleRepository
    {
        Task AddAsync(Vehicle vehicle);
        Task UpdateAsync(Vehicle vehicle);
        Task<Vehicle> GetByChassisIdAsync(Chassi chassisId);
        Task<IEnumerable<Vehicle>> GetAllAsync();
    }
}
