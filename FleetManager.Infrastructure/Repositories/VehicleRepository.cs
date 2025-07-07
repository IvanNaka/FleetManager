using FleetManager.Domain.Entities;
using FleetManager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly FleetDbContext _context;

        public VehicleRepository(FleetDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<Vehicle?> GetByChassisIdAsync(Chassi chassi)
        {
            return await _context.Vehicles.Where(v => v.ChassiId.Equals(chassi.Id)).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles.ToListAsync();
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }
    }
}
