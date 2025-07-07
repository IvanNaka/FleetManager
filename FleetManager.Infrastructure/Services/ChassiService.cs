using FleetManager.Domain.Entities;
using FleetManager.Domain.Repositories;
using FleetManager.Domain.Services;

namespace FleetManager.Infrastructure.Services
{
    public class ChassiService : IChassiService
    {
        private readonly IChassiRepository _chassiRepository;

        public ChassiService(IChassiRepository chassiRepository)
        {
            _chassiRepository = chassiRepository;
        }

        public async Task<Chassi> AddChassiAsync(uint number, string serie)
        {
            if (string.IsNullOrWhiteSpace(serie))
                throw new ArgumentException("Serie must be provided.", nameof(serie));

            if (number == 0)
                throw new ArgumentException("Number must be greater than zero.", nameof(number));

            var existingChassi = await _chassiRepository.GetBySeriesAndNumber(number, serie);
            if (existingChassi != null)
                return existingChassi;
            var chassi = new Chassi(serie, number);
            var chassiCreated = await _chassiRepository.AddAsync(chassi);
            return chassiCreated;
        }

        public async Task<Chassi?> GetBySeriesAndNumberAsync(uint number, string serie)
        {
            if (string.IsNullOrWhiteSpace(serie))
                throw new ArgumentException("Serie must be provided.", nameof(serie));
            if (number == 0)
                throw new ArgumentException("Number must be greater than zero.", nameof(number));

            return await _chassiRepository.GetBySeriesAndNumber(number, serie);
        }
    }
}
