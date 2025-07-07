using FleetManager.Domain.Entities;
using FleetManager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FleetManager.Infrastructure.Repositories
{
    public class ChassiRepository : IChassiRepository
    {
        private readonly FleetDbContext _context;
        public ChassiRepository(FleetDbContext context)
        {
            _context = context;
        }
        public async Task<Chassi> AddAsync(Chassi chassi)
        {
            var chassiCreated = await _context.Chassis.AddAsync(chassi);
            await _context.SaveChangesAsync();
            return chassiCreated.Entity;
        }
        public async Task<Chassi?> GetBySeriesAndNumber(uint number, string serie)
        {

            return await _context.Chassis
                .Where(v => v.Serie.Equals(serie) && v.Number.Equals(number))
                .FirstOrDefaultAsync();
        }
    }
}
