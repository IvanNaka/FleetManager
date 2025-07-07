using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Domain.Entities
{
    public class Vehicle : BaseEntity
    {
        public virtual Guid Id { get; set; }
        public virtual Guid ChassiId { get; }
        public virtual Chassi Chassi { get;}
        public virtual string Type { get;}
        public virtual int NumberOfPassengers { get;}
        public virtual string Color { get; set;}

        private Vehicle() { }
        protected Vehicle(Chassi chassi, string color, string type)
        {
            Chassi = chassi;
            Color = color;
            Type = type;
        }
    }
}
