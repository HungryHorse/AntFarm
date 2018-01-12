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
    class AgressiveAntAgent : AntAgent
    {

        //The worker ant that the aggressive ant is currently following as well as
        //a boolean to show whether or not it has a target
        public WorkerAntAgent following;
        public bool hasFollowing;

        //The position of the most recently located worker ant that had food as well as a boolean
        //to quickly check whether or not it has an ant in memory
        public SOFT152Vector lastAntFoundWithFood;
        public bool hasPreviousAntFound;

        //The position of the aggressive ant nest and whether or not one is known
        public SOFT152Vector NestPosMemory { set; get; }
        public bool hasNestLocation;

        public AgressiveAntAgent(SOFT152Vector position, Random random, Rectangle bounds) : base(position, random, bounds)
        {
            agentPosition = new SOFT152Vector(position.X, position.Y);

            worldBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);

            randomNumberGenerator = random;

            InitialiseAgent();
        }
    }
}
