using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Domain.Entities
{
    public abstract class BaseEntity
    {
        public DateTime CreationDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool? Active { get; private set; } = true;
        public virtual void Activate()
            => Active = true;

        public virtual void Delete()
            => Active = false;
    }
}
