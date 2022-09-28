using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace DataSnake
{
    public class Game1 : Game
    {

        Texture2D ballTexture;
        Texture2D berryTexture;
        Vector2 ballPosition;
        Vector2 lengthPosition;
        Vector2 turnPosition;
        float ballSpeed;
        float timeSinceLastDraw = 0;
        List<Vector2> trailPos = new List<Vector2>();
        List<SoundEffect> soundEffects = new List<SoundEffect>();
        int trailCount = 0;
        int turnCount = 0;
        bool berryPickedUp = false;
        bool upPressed;
        bool downPressed;
        bool leftPressed;
        bool rightPressed;
        bool firstDraw = true;
        bool dead = false;


        //1 = Up, 2 = Down, 3 = Left, 4 = Right 
        int curDirection = 4;
        //int[] lastDirection = new int[100];
        int lastDirection = 4;

        int numLengths = 0;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteBatch _berryBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
_graphics.PreferredBackBufferHeight / 2);
            ballSpeed = 100f;
            soundEffects.Add(Content.Load<SoundEffect>("pickup"));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _berryBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ballTexture = Content.Load<Texture2D>("ball");
            berryTexture = Content.Load<Texture2D>("berry");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                numLengths++;
                spawnSnakePiece(numLengths);
                dead = false;
                //ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                // _graphics.PreferredBackBufferHeight / 2);
            }

            // TODO: Add your update logic here
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up) && curDirection != 2)
            {
                if (!downPressed && !leftPressed && !rightPressed)
                {
                    upPressed = true;
                    //lastDirection[turnCount] = curDirection;
                    lastDirection =curDirection;                  
                    curDirection = 1;
                    turnCount++;
                }
                //ballPosition.Y -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                upPressed = false;
            }

            if (kstate.IsKeyDown(Keys.Down) && curDirection != 1)
            {
                if (!upPressed && !leftPressed && !rightPressed)
                {
                    downPressed = true;
                    //lastDirection[turnCount] = curDirection;
                    lastDirection = curDirection;
                    curDirection = 2;
                    turnCount++;
                }
                //ballPosition.Y += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                downPressed = false;
            }

            if (kstate.IsKeyDown(Keys.Left) && curDirection != 4)
            {
                if (!upPressed && !downPressed && !rightPressed)
                {
                    leftPressed = true;
                    //lastDirection[turnCount] = curDirection;
                    lastDirection = curDirection;
                    curDirection = 3;
                    turnCount++;
                }
                //ballPosition.X -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                leftPressed = false;
            }

            if (kstate.IsKeyDown(Keys.Right))
            {
                if (!downPressed && !leftPressed && !upPressed && curDirection != 3)
                {
                    rightPressed = true;
                    //lastDirection[turnCount] = curDirection;
                    lastDirection = curDirection;
                    curDirection = 4;
                    turnCount++;
                }
                //ballPosition.X += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                rightPressed = false;
            }

            if (((float)gameTime.TotalGameTime.TotalSeconds - timeSinceLastDraw) > 0.025)
            {
                timeSinceLastDraw = (float)gameTime.TotalGameTime.TotalSeconds;
                trailPos.Add(ballPosition);
                trailCount++;
                if (!dead)
                {
                    if (curDirection == 1)
                    {
                        ballPosition.Y -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    if (curDirection == 2)
                    {
                        ballPosition.Y += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    if (curDirection == 3)
                    {
                        ballPosition.X -= ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    if (curDirection == 4)
                    {
                        ballPosition.X += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }


            if (ballPosition.X < (0 + (ballTexture.Width/2)) || ballPosition.X > (_graphics.PreferredBackBufferWidth - (ballTexture.Width / 2)) || ballPosition.Y < (0 + (ballTexture.Height/2)) || ballPosition.Y > (_graphics.PreferredBackBufferHeight - (ballTexture.Height/2)))
            {
                dead = true;
            }

            if (berryPickedUp == false && ballPosition.X > 45  && ballPosition.X < 105 && ballPosition.Y > 220 && ballPosition.Y < 280)
            {
                berryPickedUp = true;
                soundEffects[0].Play();
                numLengths++;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            DrawHead();
            DrawTail(1, 0, gameTime);

            if (berryPickedUp == false)
            {
                _berryBatch.Begin();
                _berryBatch.Draw(berryTexture, new Vector2(75, 250), null, Color.White);
                _berryBatch.End();
            }

            base.Draw(gameTime);
        }

        public void spawnSnakePiece(int pos)
        {

        }

        public void DrawHead()
        {
                _spriteBatch.Begin();
                _spriteBatch.Draw(
                    ballTexture,
                    ballPosition,
                    null,
                    Color.White,
                    0f,
                    new Vector2(ballTexture.Width / 2, ballTexture.Height / 2),
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                    );
                _spriteBatch.End();

        }
        public void DrawTail(int pos, int dir, GameTime gameTime)
        {
            for (int i = 1; i <= numLengths; i++)
            {
                SpriteBatch snakeTail = new SpriteBatch(GraphicsDevice);
                if (trailCount < 5)
                {
                    if (lastDirection == 1)
                    {
                        lengthPosition = new Vector2(ballPosition.X, ballPosition.Y + (i * ballTexture.Height));
                    }
                    else if (lastDirection == 2)
                    {
                        lengthPosition = new Vector2(ballPosition.X, ballPosition.Y - (i * ballTexture.Height));
                    }
                    else if (lastDirection == 3)
                    {
                        lengthPosition = new Vector2(ballPosition.X + (i * ballTexture.Width), ballPosition.Y);
                    }
                    else if (lastDirection == 4)
                    {
                        lengthPosition = new Vector2(ballPosition.X - (i * ballTexture.Width), ballPosition.Y);
                    }
                    if (lengthPosition == turnPosition)
                    {

                    }
                }
                else
                {
                        try 
                        { 
                            lengthPosition = trailPos[trailCount - (4 * i)]; 
                        }
                        catch
                        {
                        Console.WriteLine("Length is greater than the capacity of the trail array!");
                        }
                }
                snakeTail.Begin();
                snakeTail.Draw(
                        ballTexture,
                        lengthPosition,
                        null,
                        Color.White,
                        0f,
                        new Vector2(ballTexture.Width / 2, ballTexture.Height / 2),
                        Vector2.One,
                        SpriteEffects.None,
                        0f
                    );
                snakeTail.End();

                }

        }
    }
}