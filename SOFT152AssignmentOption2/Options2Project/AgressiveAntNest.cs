using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOFT152Steering;
using SOFT152SteeringLibrary;

namespace SteeringProject
{
    class AgressiveAntNest : Nest
    {
        public AgressiveAntNest(SOFT152Vector position) : base(position)
        {
            location = position;
        }
    }
}
