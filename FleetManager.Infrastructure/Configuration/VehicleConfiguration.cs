using FleetManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Infrastructure.Configuration
{
    public class VehicleConfiguration : BaseConfiguration<Vehicle>
    {
        public override void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            base.Configure(builder);

            builder.HasNoKey();
            builder.ToTable("Vehicle");
            builder.Property(v => v.ChassiId).HasColumnType("uniqueidentifier").HasColumnName("ChassisId").IsRequired();
            builder.Property(v => v.Color).HasColumnName("Color").HasMaxLength(50).IsRequired();
            builder.Property(v => v.Type).HasColumnName("Type").HasMaxLength(50).IsRequired();
            builder.Property(v => v.NumberOfPassengers).HasColumnName("NumberOfPassengers").IsRequired();
            builder.HasOne(x => x.Chassi)
                .WithMany()
                .HasForeignKey(x => x.ChassiId);
        }
    }
}
