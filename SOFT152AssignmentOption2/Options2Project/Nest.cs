using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOFT152SteeringLibrary;

namespace SteeringProject
{
    class Nest
    {
        /// <summary>
        /// The location of the nest in the world
        /// </summary>
        public SOFT152Vector location { get; set; }

        public Nest(SOFT152Vector position)
        {
            location = position;
        }
    }
}
