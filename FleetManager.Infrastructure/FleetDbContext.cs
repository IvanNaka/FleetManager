using FleetManager.Domain.Entities;
using FleetManager.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Infrastructure
{
    public class FleetDbContext : DbContext
    {
        public FleetDbContext(DbContextOptions<FleetDbContext> options, IConfiguration configuration) : base(options) 
        {
        }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Chassi> Chassis { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(IConfigurationScan).Assembly);
        }
    }
}
