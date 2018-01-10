﻿using System;
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
        public WorkerAntAgent following;
        public bool hasFollowing;

        public SOFT152Vector lastAntFoundWithFood;
        public bool hasPreviousAntFound;

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