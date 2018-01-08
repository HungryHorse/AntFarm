using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteeringProject;
using SOFT152SteeringLibrary;

namespace SOFT152Steering
{
    public partial class AntFoodForm : Form
    {
        // Declares list of agents
        private List<WorkerAntAgent> antList;

        // Declares list of nests
        private List<Nest> nestList;

        // Declares list of food
        private List<Food> foodList;

        // Declare a stationary object
        private SOFT152Vector someObject;

        // the random object given to each Ant agent
        private Random randomGenerator;

        // A bitmap image used for double buffering
        private Bitmap backgroundImage;


        public AntFoodForm()
        {        
            InitializeComponent();

            CreateBackgroundImage();

            nestList = new List<Nest>();
            foodList = new List<Food>();

            CreateAnts();
        }

        /// <summary>
        /// Creates all the ants when the simulation is first executed
        /// </summary>
        private void CreateAnts()
        {
            antList = new List<WorkerAntAgent>();
            WorkerAntAgent tempAgent;
            Rectangle worldLimits;
            int randX;
            int randY;
            int antLimit;

            // create a radnom object to pass to the ants
            randomGenerator = new Random();

            // define some world size for the ants to move around on
            // assume the size of the world is the same size as the panel
            // on which they are displayed
            worldLimits = new Rectangle(0, 0, drawingPanel.Width, drawingPanel.Height);

            // sets the number of ants to make
            // TODO: allow user to delcare 
            antLimit = 100;

            // generates ants in random locations and adds them to the dynamic list 
            for (int i = 0; i < antLimit; i++)
            {
                randX = randomGenerator.Next(0, worldLimits.Width + 1);
                randY = randomGenerator.Next(0, worldLimits.Height + 1);
                tempAgent = new WorkerAntAgent(new SOFT152Vector(randX, randY), randomGenerator, worldLimits);
                antList.Add(tempAgent);
                antList[i].AgentSpeed = 1.0;
                antList[i].WanderLimits = 0.25;
                antList[i].usedUpFoodList = new List<SOFT152Vector>();

                // keep the agent within the world
                antList[i].ShouldStayInWorldBounds = true;
            }

            someObject = new SOFT152Vector(250, 250);
        }

        private void CreateFoodSource(SOFT152Vector position)
        { 
            Food tempFood;

            tempFood = new Food(position);
            foodList.Add(tempFood);
        }

        private void CreateNest(SOFT152Vector position)
        {
            Nest tempNest;

            tempNest = new Nest(position);
            nestList.Add(tempNest);
        }

        /// <summary>
        ///  Creates the background image to be used in double buffering 
        /// </summary>
        private void CreateBackgroundImage()
        {
            int imageWidth;
            int imageHeight;

            // the backgroundImage can be any size
            // assume it is the same size as the panel 
            // on which the Ants are drawn
            imageWidth = drawingPanel.Width;
            imageHeight = drawingPanel.Height;

            backgroundImage = new Bitmap(drawingPanel.Width, drawingPanel.Height);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // may be useful at a later date
            SOFT152Vector tempPosition;

                        
            for (int i = 0; i < antList.Count; i++)
            {
                // the current ant being checked for conditions eg. if close to food
                WorkerAntAgent primaryAnt;
                primaryAnt = antList[i];

                // this will be checked and will remove the ants memory of food if it gets within 2 units of it's food memory
                if (primaryAnt.hasFoodLocation)
                {
                    if (primaryAnt.AgentPosition.Distance(primaryAnt.FoodPosMemory) < 2)
                    {
                        primaryAnt.usedUpFoodList.Add(new SOFT152Vector(primaryAnt.FoodPosMemory));
                        primaryAnt.ErasedFoodLocation = new SOFT152Vector(primaryAnt.FoodPosMemory);
                        primaryAnt.hasErasedLocation = true;
                        primaryAnt.FoodPosMemory = null;
                        primaryAnt.hasFoodLocation = false;
                    }
                }

                // cycles through all food sources to check if the primary agent is close enough to
                // interact with it
                for (int j = 0; j < foodList.Count; j++)
                {

                    // sets the current food that is being checked against the primary agent
                    Food food;
                    food = foodList[j];

                    // if the primary agent is within a 5 unit radius i will interact with the
                    // the food (either destroying its memory of it or collecting food)
                    if (primaryAnt.AgentPosition.Distance(food.location) < 5)
                    {

                        // if the food has been depleted the ant will forget the food source 
                        // and add it too a bank of forgoten food sources
                        if (food.quantity <= 0)
                        {

                            if (primaryAnt.hasFoodLocation)
                            {
                                primaryAnt.usedUpFoodList.Add(new SOFT152Vector(primaryAnt.FoodPosMemory));
                                primaryAnt.ErasedFoodLocation = new SOFT152Vector(primaryAnt.FoodPosMemory);
                                primaryAnt.hasErasedLocation = true;
                            }

                            primaryAnt.FoodPosMemory = null;
                            primaryAnt.hasFoodLocation = false;

                            foodList.RemoveAt(j);
                        }

                        // if the food isn't depleted and the ant isn't carrying food then it will pick up food
                        else if(!primaryAnt.isCarryingFood)
                        {
                            food.quantity -= 1;
                            primaryAnt.isCarryingFood = true;
                        }

                    }

                    // if the ant is within 40 units of the food source it will check if it know that the food has 
                    // been erased or if it is in the list of food sources that have been erased
                    // before, if it is then it will forget the new information straight away
                    else if (primaryAnt.AgentPosition.Distance(food.location) < 40 && primaryAnt.hasErasedLocation)
                    {
                        if (!MatchingVectors(primaryAnt.ErasedFoodLocation, food.location))
                        {
                            primaryAnt.FoodPosMemory = food.location;
                            primaryAnt.hasFoodLocation = true;
                        }
                        else
                        {
                            foreach (SOFT152Vector erased in primaryAnt.usedUpFoodList)
                            {
                                if(MatchingVectors(erased, primaryAnt.AgentPosition))
                                {
                                    primaryAnt.FoodPosMemory = null;
                                    primaryAnt.hasFoodLocation = false;
                                }
                            }
                        }
                    }

                    else if(primaryAnt.AgentPosition.Distance(foodList[j].location) < 40)
                    {
                        primaryAnt.FoodPosMemory = food.location;
                        primaryAnt.hasFoodLocation = true;
                    }
                }

                //Checks to see if the ant is within the radius of the nest - if it is then deposit food
                //If the ant doesn't know where a nest is already, it will learn
                for (int k = 0; k < nestList.Count; k++)
                {

                    Nest nest;
                    nest = nestList[k];

                    if(primaryAnt.AgentPosition.Distance(nest.location) < 5)
                    {
                        primaryAnt.isCarryingFood = false;
                    }

                    else if (primaryAnt.AgentPosition.Distance(nest.location) < 40)
                    {
                        primaryAnt.NestPosMemory = nest.location;
                        primaryAnt.hasNestLocation = true;
                    }
                }

                //Interacting with other ants and passing information between them
                for (int l = 0; l < antList.Count; l++)
                {

                    WorkerAntAgent secondaryAnt;
                    secondaryAnt = antList[l];

                    if (primaryAnt.AgentPosition.Distance(secondaryAnt.AgentPosition) < 5 && i != l)
                    {

                        if (secondaryAnt.hasNestLocation && !primaryAnt.hasNestLocation)
                        {
                            primaryAnt.NestPosMemory = new SOFT152Vector(secondaryAnt.NestPosMemory);
                            primaryAnt.hasNestLocation = true;
                        }

                        if (secondaryAnt.hasFoodLocation && !primaryAnt.hasFoodLocation)
                        {
                            if (!KnowsDestoryedFoodSource(primaryAnt, secondaryAnt.FoodPosMemory))
                            {
                                primaryAnt.FoodPosMemory = new SOFT152Vector(secondaryAnt.FoodPosMemory);
                                primaryAnt.hasFoodLocation = true;
                            }
                        }

                    }
                }

                //Random chance for ants to forget the information that they have learned
                if(randomGenerator.Next(0, 501) <= 2)
                {
                    primaryAnt.FoodPosMemory = null;
                    primaryAnt.hasFoodLocation = false;
                }
                if (randomGenerator.Next(0, 501) <= 2)
                {
                    primaryAnt.NestPosMemory = null;
                    primaryAnt.hasNestLocation = false;
                }


                if (primaryAnt.hasFoodLocation && !primaryAnt.isCarryingFood)
                {
                    primaryAnt.Approach(primaryAnt.FoodPosMemory);
                }
                else if (primaryAnt.hasNestLocation && primaryAnt.isCarryingFood)
                {
                    primaryAnt.Approach(primaryAnt.NestPosMemory);
                }
                // let agent1 wander
                else
                {
                    primaryAnt.Wander();
                }
            }

            DrawAgentsDoubleBuffering();

        }
        
        private bool KnowsDestoryedFoodSource(WorkerAntAgent ant, SOFT152Vector passingVector)
        {
            foreach( SOFT152Vector vector in ant.usedUpFoodList)
            {
                if(MatchingVectors(vector, passingVector))
                {
                    return true;
                }
            }
            return false;
        }

        private bool MatchingVectors(SOFT152Vector primaryVector, SOFT152Vector secondaryVector)
        {
            if (primaryVector.X == secondaryVector.X && primaryVector.Y == secondaryVector.Y)
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Draws the ants and any stationary objects using double buffering
        /// </summary>
        private void DrawAgentsDoubleBuffering()
        {
            AntAgent agent1;
            Food food1;
            Nest nest1;
            // using FillRectangle to draw the agents
            // so declare variables to draw with
            float agentXPosition;
            float agentYPosition;

            float foodXPosition;
            float foodYPosition;

            float nestXPosition;
            float nestYPosition;

            // some arbitary size to draw the Ant
            float antSize;

            int foodPercent;

            antSize = 5.0f;

            Brush solidBrush;

            // get the graphics context of the background image
            using (Graphics backgroundGraphics =  Graphics.FromImage(backgroundImage))
            {
                
                backgroundGraphics.Clear(Color.White);

                for (int i = 0; i < antList.Count; i++)
                {
                    solidBrush = new SolidBrush(Color.Red);
                    agent1 = antList[i];
                    agentXPosition = (float)agent1.AgentPosition.X;
                    agentYPosition = (float)agent1.AgentPosition.Y;

                    // create a brush
                    if (agent1.isCarryingFood)
                    {
                        solidBrush = new SolidBrush(Color.Purple);
                    }

                    // draw the 1st agent on the backgroundImage
                    backgroundGraphics.FillRectangle(solidBrush, agentXPosition, agentYPosition, antSize, antSize);
                }

                solidBrush = new SolidBrush(Color.Blue);                

                for(int i = 0; i < foodList.Count; i++)
                {
                    food1 = foodList[i];
                    foodXPosition = (float)food1.location.X;
                    foodYPosition = (float)food1.location.Y;

                    foodPercent = (int)((food1.quantity / 100.0) * 20.0);

                    backgroundGraphics.FillEllipse(solidBrush, foodXPosition - (foodPercent/2), foodYPosition - (foodPercent/2), foodPercent, foodPercent);
                }

                solidBrush = new SolidBrush(Color.Green);

                for (int i = 0; i < nestList.Count; i++)
                {
                    nest1 = nestList[i];
                    nestXPosition = (float)nest1.location.X;
                    nestYPosition = (float)nest1.location.Y;

                    backgroundGraphics.FillEllipse(solidBrush, nestXPosition - 10, nestYPosition - 10, 20, 20);
                }
            }

            // now draw the image on the panel
            using (Graphics g = drawingPanel.CreateGraphics())
            {
                g.DrawImage(backgroundImage, 0, 0, drawingPanel.Width, drawingPanel.Height);
            }

                // dispose of resources
                solidBrush.Dispose();
        }



        private void stopButton_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void drawingPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                SOFT152Vector position;
                position = new SOFT152Vector(e.Location.X, e.Location.Y);
                CreateFoodSource(position);
            }
            else if(e.Button == MouseButtons.Right)
            {
                SOFT152Vector position;
                position = new SOFT152Vector(e.Location.X, e.Location.Y);
                CreateNest(position);
            }
        }
    }
}



//private void DrawAgents(AntAgent agent1)
//{

//    // using FillRectangle to draw the agents
//    // so declare variables to draw with
//    float agentXPosition;
//    float agentYPosition;

//    // some arbitary size to draw the Ant
//    float antSize;

//    antSize = 5.0f;

//    Brush solidBrush;

//    // get the graphics context of the panel
//    using (Graphics g = drawingPanel.CreateGraphics())
//    {
//        // get the 1st agent position
//        agentXPosition = (float)agent1.AgentPosition.X;
//        agentYPosition = (float)agent1.AgentPosition.Y;

//        // create a brush
//        solidBrush = new SolidBrush(Color.Red);

//        // draw the 1st agent
//        g.FillRectangle(solidBrush, agentXPosition, agentYPosition, antSize, antSize);


//    }

//    // dispose of resources
//    solidBrush.Dispose();
//}