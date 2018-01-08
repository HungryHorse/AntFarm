using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOFT152Steering;
using SOFT152SteeringLibrary;
using System.Drawing;

namespace SteeringProject
{
    class WorkerAntAgent : AntAgent
    {
        /// <summary>
        /// The location of the current food and nest known to the agent 
        /// </summary>
        public SOFT152Vector NestPosMemory { set; get; }
        public bool hasNestLocation;

        public SOFT152Vector FoodPosMemory { set; get; }
        public bool hasFoodLocation;

        public SOFT152Vector ErasedFoodLocation { set; get; }
        public bool hasErasedLocation;

        public List<SOFT152Vector> usedUpFoodList;


        public WorkerAntAgent(SOFT152Vector position, Random random, Rectangle bounds) : base(position, random, bounds)
        {
            agentPosition = new SOFT152Vector(position.X, position.Y);

            worldBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);

            randomNumberGenerator = random;

            InitialiseAgent();
        }
    }
}
