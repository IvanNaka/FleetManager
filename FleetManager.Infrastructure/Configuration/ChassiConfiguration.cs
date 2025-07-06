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
    public class ChassiConfiguration : BaseConfiguration<Chassi>
    {
        public override void Configure(EntityTypeBuilder<Chassi> builder)
        {
            base.Configure(builder);

            builder.ToTable("Chassi");

            builder.HasKey(prop => prop.Id);
            builder.Property(v => v.Id).HasColumnType("uniqueidentifier").ValueGeneratedOnAdd();
            builder.Property(v => v.Number).HasColumnType("int").HasColumnName("ChassisId").IsRequired();
            builder.Property(v => v.Serie).HasColumnName("Serie").HasMaxLength(50).IsRequired();
        }
    }
}
