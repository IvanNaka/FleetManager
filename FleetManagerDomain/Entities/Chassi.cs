using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Domain.Entities
{
    public class Chassi : BaseEntity
    {
        public Guid Id;
        public readonly string Serie;
        public readonly uint Number;

        public Chassi(string serie, uint number)
        {
            Serie = serie;
            Number = number;
        }
    }
}
