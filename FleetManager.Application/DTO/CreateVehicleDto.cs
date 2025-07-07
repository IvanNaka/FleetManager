using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Application.DTO
{
    public class CreateVehicleDto
    {
        public string ChassisSeries { get; set; }
        public uint ChassisNumber { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
    }
}
