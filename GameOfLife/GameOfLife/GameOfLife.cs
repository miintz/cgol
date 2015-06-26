using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameOfLife
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameOfLife : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ConwayCreatures Lifer;

        public Texture2D textureFill { get; set; }
        public bool clickOccured { get; set; }
        public KeyboardState PreviousKeyboardState { get; set; }

        public Texture2D texture1px { get; set; }

        public int height { get; set; }

        public int width { get; set; }

        int gridSize;

        int cols;
        int rows;
        int centerX;
        int centerY;

        int[,] LifeMatrix;

        Boolean playing;

        public GameOfLife()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 750;
            graphics.PreferredBackBufferWidth = 750;

            centerX = graphics.PreferredBackBufferHeight / 2;
            centerY = graphics.PreferredBackBufferWidth / 2;

            height = graphics.PreferredBackBufferHeight;
            width = graphics.PreferredBackBufferWidth;

            cols = 750;
            rows = 750;

            gridSize = 5;

            Content.RootDirectory = "Content";

            LifeMatrix = new int[cols / gridSize, rows / gridSize];

            this.IsMouseVisible = true;

            //lower the framerate
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 20.0f);

            playing = false;
            
            Lifer = new ConwayCreatures();
            
            //add a few gliders
            int[,] glider = Lifer.LWSpaceShip();

            int gx = Lifer.LWSpaceShipX;
            int gy = Lifer.LWSpaceShipY;

            for (int x = 0; x < gx; x++)
            {
                for (int y = 0; y < gy; y++)
                {
                    LifeMatrix[75 + x, 75 + y] = glider[x, y];                 
                }
            }

            //add a few gliders
            int[,] glider2 = Lifer.GliderGun();

            int gx2 = Lifer.GliderGunX;
            int gy2 = Lifer.GliderGunY;

            for (int x = 0; x < gx2; x++)
            {
                for (int y = 0; y < gy2; y++)
                {
                    LifeMatrix[75 + x, 50 + y] = glider2[x, y];
                }
            }

            //add a few gliders
            int[,] glider3 = Lifer.Glider();

            int gx3 = Lifer.GliderX;
            int gy3 = Lifer.GliderY;

            for (int x = 0; x < gx3; x++)
            {
                for (int y = 0; y < gy3; y++)
                {
                    LifeMatrix[60 + x, 75 + y] = glider3[x, y];
                }
            }

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture1px = new Texture2D(graphics.GraphicsDevice, 1, 1);
            texture1px.SetData(new Color[] { Color.White });

            textureFill = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            textureFill.SetData<Color>(new Color[] { Color.Black });

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {            
            MouseState m = Mouse.GetState();
            KeyboardState k = Keyboard.GetState();

            if (k.GetPressedKeys().Contains(Keys.Space) && !PreviousKeyboardState.GetPressedKeys().Contains(Keys.Space))
            {                
                playing = (!playing) ? true : false;
            }

            PreviousKeyboardState = k;

            if (m.LeftButton == ButtonState.Pressed)
            {
                if (!playing && !clickOccured)
                {
                    int x = m.X / gridSize;
                    int y = m.Y / gridSize;

                    if (x > 0 && x < (rows / gridSize) - 1&& y > 0 && y < (cols / gridSize) - 1)
                    {
                        if (LifeMatrix[x, y] != 1)
                            LifeMatrix[x, y] = 1;
                        else
                            LifeMatrix[x, y] = 0;
                    }

                    clickOccured = true;
                }
            }
            else if (m.LeftButton == ButtonState.Released)
                clickOccured = false;

                
            if (playing)
            {
                int[,] GenMatrix = new int[cols / gridSize, rows / gridSize];

                for (uint x = 0; x < cols / gridSize; x++)
                {
                    for (uint y = 0; y < rows / gridSize; y++)
                    {
                        int topLeft = (x != 0 && y != 0) ? LifeMatrix[x - 1, y - 1] : 0;
                        int topRight = (x != (cols / gridSize) - 1 && y != 0) ? LifeMatrix[x + 1, y - 1] : 0;
                        int bottomLeft = (x != 0 && y != (rows / gridSize) - 1) ? LifeMatrix[x - 1, y + 1] : 0;
                        int bottomRight = (x != (cols / gridSize) - 1 && y != (rows / gridSize) - 1) ? LifeMatrix[x + 1, y + 1] : 0;

                        int bottom = (y != (rows / gridSize) - 1) ? LifeMatrix[x, y + 1] : 0;
                        int right = (x != (cols / gridSize) - 1) ? LifeMatrix[x + 1, y] : 0;
                        int top = (y != 0) ? LifeMatrix[x, y - 1] : 0;
                        int left = (x != 0) ? LifeMatrix[x - 1, y] : 0;

                        int sum = topLeft + topRight + bottomLeft + bottomRight + bottom + top + left + right;

                        if ((sum == 2 || sum == 3) && LifeMatrix[x, y] == 1) //if alive and surrounded by at least 2 living ones, stay alive
                        {
                            GenMatrix[x, y] = 1;
                        }

                        if (sum == 3 && LifeMatrix[x, y] == 0) //if surrounded by 3 living ones and is dead:
                        {
                            //this one is now alive
                            GenMatrix[x, y] = 1;
                        }

                        if (sum > 3) //more than 4 around it, overpopulation so:
                        {
                            //this one dies
                            GenMatrix[x, y] = 0;
                        }
                    }
                }

                LifeMatrix = GenMatrix; //set it to new generation
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            for (float x = -cols; x < cols; x++)
            {
                Rectangle rectangle = new Rectangle((int)(centerX + x * gridSize), 0, 1, height);
                spriteBatch.Draw(texture1px, rectangle, Color.DimGray);
            }

            for (float y = -rows; y < rows; y++)
            {
                Rectangle rectangle = new Rectangle(0, (int)(centerY + y * gridSize), width, 1);
                spriteBatch.Draw(texture1px, rectangle, Color.DimGray);
            }

            //draw lifematrix
            for (int x = 0; x < cols / gridSize; x++)
            {
                for (int y = 0; y < rows / gridSize; y++)
                {
                    if (LifeMatrix[x, y] == 1)
                    {
                        Rectangle rectangle = new Rectangle(x * gridSize, y * gridSize, gridSize, gridSize);
                            
                        spriteBatch.Draw(textureFill, rectangle, Color.DimGray);
                    }
                }
            }

            spriteBatch.End();
            // TODO: Add your drawing code here
        

            base.Draw(gameTime);
        }
    }

    public class ConwayCreatures
    {
        public int GliderX = 3;
        public int GliderY = 3;

        public int[,] Glider()
        {
            String RLE = "bo$2bo$3o";
            return ParseRLE(RLE, new int[GliderX, GliderY]);
        }

        public int GliderGunX = 36;
        public int GliderGunY = 9;
        public int[,] GliderGun() //faulty
        {
            //b = dead cell
            //o = living cell
            //$ = end line

            String RLE = "24bo11b$22bobo11b$12b2o6b2o12b2o$11bo3bo4b2o12b2o$2o8bo5bo3b2o14b$2o8bo3bob2o4bobo11b$10bo5bo7bo11b$11bo3bo20b$12b2o";
            return ParseRLE(RLE, new int[GliderGunX, GliderGunY]);
        }

        
        public int LWSpaceShipX = 5;
        public int LWSpaceShipY = 4;
        public int[,] LWSpaceShip() //faulty
        {
            String RLE = "bo2bo$o4b$o3bo$4o";
            return ParseRLE(RLE, new int[LWSpaceShipX, LWSpaceShipY]);
        }

        private int[,] DecompressRLE(String rle)
        {
            int[,] piece;
            char num;
            int x = 0;
            int y = 0;

            foreach (char s in rle)
            {
                if (s == 'b')
                { 
                    //x = (num == ' ') ? 1 : 
                }
                else if (s == 'o')
                { }
                else if (s == '$')
                { }
                else if (s == '!')
                    break;
                else
                { }
                
            }

            return new int[1,1];
        }

        private int[,] ParseRLE(string RLE, int[,] size)
        {
            char[] RLEChars = RLE.ToCharArray();
            int[,] rxy = size;
            
            int x = 0;
            int y = 0;

            int oldmod = 1;
            int i = 0;
            //doesnt work yet for double digit modifiers such as glider gun
            //and B3/S23 rules
            foreach (char n in RLEChars)
            {
                if (oldmod != 1)
                {
                    i++;
                    oldmod = 1;
                    continue;
                }

                int modifier = 1;
                int value = -1;
                
                switch (n)
                { 
                    case 'b':                        
                        value = 0;                        
                        break;
                    case 'o':
                        value = 1;
                        break;
                    case '$':
                        y++;
                        x = 0;
                        continue;                      
                    default:                        
                        modifier = Int32.Parse(n.ToString());                        
                        value = (RLEChars[i + 2] == 'b') ? 0 : 1;                        
                        break;                            
                }

                oldmod = modifier;

                for (int o = modifier; 0 < o; o--)
                {
                    System.Console.WriteLine(value + " " + modifier + " " + n);
                    rxy[x, y] = value;
                    x++;
                }                

                i++;
            }

            return rxy;
        }
    }
}
