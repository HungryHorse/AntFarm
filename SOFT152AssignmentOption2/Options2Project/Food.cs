using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOFT152SteeringLibrary;

namespace SteeringProject
{
    class Food
    {

        /// <summary>
        /// The quantity of food remaining in the nest
        /// </summary>
        public double quantity { set; get; }
        /// <summary>
        /// The location of the food source in the world
        /// </summary>
        public SOFT152Vector location { set; get; }

        public bool isEmpty;

        public Food(SOFT152Vector position)
        {
            location = position;
            quantity = 100;
            isEmpty = false;
        }

        public Food(SOFT152Vector postion, int amount)
        {
            location = postion;
            quantity = amount;
            isEmpty = false;
        }
    }
}
