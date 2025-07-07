using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Domain.Entities
{
    public class Bus: Vehicle
    {
        public override string Type => "Bus";
        public override int NumberOfPassengers => 42;
        public override Guid ChassiId => Chassi?.Id ?? Guid.Empty;
        public Bus(Chassi chassisId, string color) : base(chassisId, color, "Bus")
        {

        }
    }
}
