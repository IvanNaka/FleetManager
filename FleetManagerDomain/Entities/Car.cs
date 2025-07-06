using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Domain.Entities
{
    public class Car : Vehicle
    {
        public override string Type => "Car";
        public override int NumberOfPassengers => 4;
        public override Guid ChassiId => Chassi?.Id ?? Guid.Empty;
        public Car(Chassi chassisId, string color) : base(chassisId, color)
        { 
        }
    }
}
