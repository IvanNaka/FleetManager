using FleetManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Domain.Repositories
{
    public interface IChassiRepository
    {
        Task<Chassi> AddAsync(Chassi chassi);
        Task<Chassi?> GetBySeriesAndNumber(uint number, String serie);
    }
}
