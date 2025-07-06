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
    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(prop => prop.CreationDate).HasColumnType("datetime").HasColumnName("CreationDate").IsRequired();
            builder.Property(prop => prop.LastUpdate).HasColumnType("datetime").HasColumnName("LastUpdate").IsRequired(false);
            builder.Property(prop => prop.Active).HasColumnName("Active").HasDefaultValue(true).IsRequired();
        }
    }
}
