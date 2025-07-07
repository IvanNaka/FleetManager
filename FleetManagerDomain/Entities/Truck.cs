using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Domain.Entities
{
    public class Truck : Vehicle
    {
        public override string Type => "Truck";
        public override int NumberOfPassengers => 1;

        // Implementing the missing ChassiId property
        public override Guid ChassiId => Chassi?.Id ?? Guid.Empty;

        public Truck(Chassi chassisId, string color) : base(chassisId, color, "Truck")
        {
        }
    }
}
