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
        // decalers list of agents
        private List<AntAgent> antList;

        // decalers list of nests
        private List<Nest> nestList;

        // decalers list of food
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

            HardCodeFoodLocation();

            HardCodeNestLocation(); 
        }

        private void CreateAnts()
        {
            antList = new List<AntAgent>();
            AntAgent tempAgent;
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

            antLimit = 100;

            for (int i = 0; i < antLimit; i++)
            {
                randX = randomGenerator.Next(0, worldLimits.Width + 1);
                randY = randomGenerator.Next(0, worldLimits.Height + 1);
                tempAgent = new AntAgent(new SOFT152Vector(randX, randY), randomGenerator, worldLimits);
                antList.Add(tempAgent);
                antList[i].AgentSpeed = 1.0;
                antList[i].WanderLimits = 0.25;

                // keep the agent within the world
                antList[i].ShouldStayInWorldBounds = true;
            }

            someObject = new SOFT152Vector(250, 250);
        }

        private void HardCodeFoodLocation()
        {
            SOFT152Vector vector;

            vector = new SOFT152Vector(30, 40);
            CreateFoodSource(vector);
        }

        private void HardCodeNestLocation()
        {
            SOFT152Vector vector;

            vector = new SOFT152Vector(200, 100);
            CreateNest(vector);
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

            // the backgroundImage  can be any size
            // assume it is the same size as the panel 
            // on which the Ants are drawn
            imageWidth = drawingPanel.Width;
            imageHeight = drawingPanel.Height;

            backgroundImage = new Bitmap(drawingPanel.Width, drawingPanel.Height);
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            SOFT152Vector tempPosition;

            // one each time tick each of the two agents makes one movment

            // set some values for agent1
            // before it moves
            
            for (int i = 0; i < antList.Count; i++)
            {
                for (int j = 0; j < foodList.Count; j++)
                {
                    if(antList[i].AgentPosition.Distance(foodList[j].location) < 5)
                    {
                        antList[i].isCarryingFood = true;
                    }
                    else if (antList[i].AgentPosition.Distance(foodList[j].location) < 20)
                    {
                        antList[i].FoodPosMemory = foodList[j].location;
                    }
                }

                for (int k = 0; k < nestList.Count; k++)
                {
                    if(antList[i].AgentPosition.Distance(nestList[k].location) < 5)
                    {
                        antList[i].isCarryingFood = false;
                    }
                    else if (antList[i].AgentPosition.Distance(nestList[k].location) < 20)
                    {
                        antList[i].NestPosMemory = nestList[k].location;
                    }
                }

                if (antList[i].FoodPosMemory != null && !antList[i].isCarryingFood)
                {
                    antList[i].Approach(antList[i].FoodPosMemory);
                }
                else if (antList[i].NestPosMemory != null && antList[i].isCarryingFood)
                {
                    antList[i].Approach(antList[i].NestPosMemory);
                }
                // let agent1 wander
                else
                {
                    antList[i].Wander();
                }
            }

            DrawAgentsDoubleBuffering();

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

                    backgroundGraphics.FillEllipse(solidBrush, foodXPosition, foodYPosition, (food1.quantity / 100) * 20, (food1.quantity / 100) * 20);
                }

                solidBrush = new SolidBrush(Color.Green);

                for (int i = 0; i < nestList.Count; i++)
                {
                    nest1 = nestList[i];
                    nestXPosition = (float)nest1.location.X;
                    nestYPosition = (float)nest1.location.Y;

                    backgroundGraphics.FillEllipse(solidBrush, nestXPosition, nestYPosition, 20, 20);
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