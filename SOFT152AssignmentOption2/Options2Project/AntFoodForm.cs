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
        private List<AntAgent> antList;

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
                    if (antList[i].AgentPosition.Distance(foodList[j].location) < 5)
                    {
                        if (foodList[j].quantity <= 0)
                        {
                            if (antList[i].FoodPosMemory != null)
                            {
                                antList[i].ErasedFoodLocation = new SOFT152Vector(antList[i].FoodPosMemory);
                            }
                            antList[i].FoodPosMemory = null;
                        }
                        else if(!antList[i].isCarryingFood)
                        {
                            foodList[j].quantity -= 1;
                            antList[i].isCarryingFood = true;
                        }
                    }

                    else if (antList[i].AgentPosition.Distance(foodList[j].location) < 40 && antList[i].ErasedFoodLocation != null)
                    {
                        if (antList[i].ErasedFoodLocation.X != foodList[j].location.X && antList[i].ErasedFoodLocation.Y != foodList[j].location.Y)
                        {
                            antList[i].FoodPosMemory = foodList[j].location;
                        }
                    }
                    else if(antList[i].AgentPosition.Distance(foodList[j].location) < 40)
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
                    else if (antList[i].AgentPosition.Distance(nestList[k].location) < 40)
                    {
                        antList[i].NestPosMemory = nestList[k].location;
                    }
                }

                for (int l = 0; l < antList.Count; l++)
                {
                    if (antList[i].AgentPosition.Distance(antList[l].AgentPosition) < 5 && i != l)
                    {
                        if (antList[l].NestPosMemory != null && antList[i].NestPosMemory == null)
                        {
                            antList[i].NestPosMemory = new SOFT152Vector(antList[l].NestPosMemory);
                        }
                        if (antList[l].FoodPosMemory != null && antList[i].FoodPosMemory == null)
                        {
                            if (antList[i].ErasedFoodLocation != null && antList[l].FoodPosMemory != null)
                            {
                                if (antList[i].ErasedFoodLocation.X != antList[l].FoodPosMemory.X && antList[i].ErasedFoodLocation.Y != antList[l].FoodPosMemory.Y)
                                {
                                    antList[i].FoodPosMemory = new SOFT152Vector(antList[l].FoodPosMemory);
                                }
                            }
                            else
                            {
                                antList[i].FoodPosMemory = new SOFT152Vector(antList[l].FoodPosMemory);
                            }
                        }
                    }
                }

                if(randomGenerator.Next(0, 501) <= 2)
                {
                    antList[i].FoodPosMemory = null;
                }
                if (randomGenerator.Next(0, 501) <= 2)
                {
                    antList[i].NestPosMemory = null;
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