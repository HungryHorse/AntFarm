using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SOFT152SteeringLibrary;

namespace SOFT152Steering
{
    public partial class AntFoodForm : Form
    {

        // decalare two sample agents
        private   AntAgent agent1;
        private   AntAgent agent2;

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

            CreateAnts(); 
        }

        private void CreateAnts()
        {
            Rectangle worldLimits;

            // create a radnom object to pass to the ants
            randomGenerator = new Random();

            // define some world size for the ants to move around on
            // assume the size of the world is the same size as the panel
            // on which they are displayed
            worldLimits = new Rectangle(0, 0, drawingPanel.Width, drawingPanel.Height);

            // create a couple of agents at some postions 
            agent1 = new AntAgent(new SOFT152Vector(100, 150), randomGenerator, worldLimits);

            agent2 = new AntAgent(new SOFT152Vector(200, 150), randomGenerator, worldLimits);


            // create an object at a arbitary position
            someObject = new SOFT152Vector(250, 250);
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
            agent1.AgentSpeed = 1.0;
            agent1.WanderLimits = 0.25;

            // keep the agent within the world
            agent1.ShouldStayInWorldBounds = true;

            // let agent1 wander
            agent1.Wander();


            // again set some values for agent2 before moving
            // agent2 is slower than agent1 to show some following behaviour
            agent2.AgentSpeed = 0.9;

            agent2.AvoidDistance = 100;
            agent2.ShouldStayInWorldBounds = true;

            // get agent1 position
            tempPosition = agent1.AgentPosition;

           // agent2.FleeFrom(tempPosition);

             agent2.Approach(tempPosition);


            // or get the agent to approach or flee from the stationary object
      
        //    agent2.Approach(someObject);


            // after making a movement, now draw the agents
            // DrawAgents();

            DrawAgentsDoubleBuffering();
        }

        private void DrawAgents()
        {

            // using FillRectangle to draw the agents
            // so declare variables to draw with
            float agentXPosition;
            float agentYPosition;

            // some arbitary size to draw the Ant
            float antSize;

            antSize = 5.0f;

            Brush solidBrush;

            // get the graphics context of the panel
            using (Graphics g = drawingPanel.CreateGraphics())
            {
                g.Clear(Color.White);

                // get the 1st agent position
                agentXPosition = (float)agent1.AgentPosition.X;
                agentYPosition = (float)agent1.AgentPosition.Y;

                // create a brush
                solidBrush = new SolidBrush(Color.Red);

                // draw the 1st agent
                g.FillRectangle(solidBrush, agentXPosition, agentYPosition, antSize, antSize);


                // get the 2nd agent position
                agentXPosition = (float)agent2.AgentPosition.X;
                agentYPosition = (float)agent2.AgentPosition.Y;

                // change colour of brush
                solidBrush = new SolidBrush(Color.Blue);

                // draw the 2nd agent
                g.FillRectangle(solidBrush, agentXPosition, agentYPosition, antSize, antSize);

                // now draw the stationary object
                // change colour of brush
                solidBrush = new SolidBrush(Color.Green);
                g.FillRectangle(solidBrush, (float)someObject.X, (float)someObject.Y, 20, 20);


            }

            // dispose of resources
            solidBrush.Dispose();
        }

        /// <summary>
        /// Draws the ants and any stationary objects using double buffering
        /// </summary>
        private void DrawAgentsDoubleBuffering()
        {

            // using FillRectangle to draw the agents
            // so declare variables to draw with
            float agentXPosition;
            float agentYPosition;

            // some arbitary size to draw the Ant
            float antSize;

            antSize = 5.0f;

            Brush solidBrush;

            // get the graphics context of the background image
            using (Graphics backgroundGraphics =  Graphics.FromImage(backgroundImage))
            {
                backgroundGraphics.Clear(Color.White);

                // get the 1st agent position
                agentXPosition = (float)agent1.AgentPosition.X;
                agentYPosition = (float)agent1.AgentPosition.Y;

                // create a brush
                solidBrush = new SolidBrush(Color.Red);

                // draw the 1st agent on the backgroundImage
                backgroundGraphics.FillRectangle(solidBrush, agentXPosition, agentYPosition, antSize, antSize);


                // get the 2nd agent position
                agentXPosition = (float)agent2.AgentPosition.X;
                agentYPosition = (float)agent2.AgentPosition.Y;

                // change colour of brush
                solidBrush = new SolidBrush(Color.Blue);

                // draw the 2nd agent on the backgroundImage
                backgroundGraphics.FillRectangle(solidBrush, agentXPosition, agentYPosition, antSize, antSize);

                // now draw the stationary object
                // change colour of brush
                solidBrush = new SolidBrush(Color.Green);
                backgroundGraphics.FillRectangle(solidBrush, (float)someObject.X, (float)someObject.Y, 20, 20);

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
