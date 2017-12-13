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
        public int quantity { set; get; }

        public SOFT152Vector location { set; get; }

        public Food(SOFT152Vector position)
        {
            location = position;
            quantity = 100;
        }

        public Food(SOFT152Vector postion, int amount)
        {
            location = postion;
            quantity = amount;
        }
    }
}
