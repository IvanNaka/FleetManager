using FleetManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Domain.Services
{
    public interface IChassiService
    {
        Task<Chassi> AddChassiAsync(uint number, string serie);
        Task<Chassi?> GetBySeriesAndNumberAsync(uint number, string serie);
    }
}
