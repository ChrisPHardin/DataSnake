using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace DataSnake
{
    public class Game1 : Game
    {

        Texture2D ballTexture;
        Texture2D berryTexture;
        Texture2D gameOverTexture;
        Texture2D pauseTexture;
        SpriteFont scoreFont;
        int score;
        Vector2 ballPosition;
        Vector2 lengthPosition;
        bool paused;
        int berX;
        int berY;
        int splitFactor;
        float ballSpeed;
        float speedTime;
        float timeSinceLastDraw = 0;
        float timeSinceBerryPickup = 0;
        float timeSinceUnpaused = 0;
        List<Vector2> trailPos = new List<Vector2>();
        List<SoundEffect> soundEffects = new List<SoundEffect>();
        Song gameOverMus;
        int trailCount = 0;
        int turnCount = 0;
        bool berryPickedUp = false;
        bool upPressed;
        bool downPressed;
        bool leftPressed;
        bool rightPressed;
        bool dead = false;


        //1 = Up, 2 = Down, 3 = Left, 4 = Right 
        int curDirection = 4;
        //int[] lastDirection = new int[100];
        int lastDirection = 4;

        int numLengths = 0;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteBatch _berryBatch;
        private SpriteBatch _scoreBatch;

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
            speedTime = 0.015f;
            splitFactor = 10;
            GenBerryPos();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _berryBatch = new SpriteBatch(GraphicsDevice);
            _scoreBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //Textures
            ballTexture = Content.Load<Texture2D>("ball");
            berryTexture = Content.Load<Texture2D>("berry");
            gameOverTexture = Content.Load<Texture2D>("gameover");
            pauseTexture = Content.Load<Texture2D>("paused");

            //SFX
            soundEffects.Add(Content.Load<SoundEffect>("pickup"));
            soundEffects.Add(Content.Load<SoundEffect>("diesfx"));
            soundEffects.Add(Content.Load<SoundEffect>("sm64_pause"));
            soundEffects.Add(Content.Load<SoundEffect>("unpause"));

            //Music
            this.gameOverMus = Content.Load<Song>("gameoversfx");

            //Font
            scoreFont = Content.Load<SpriteFont>("defaultfont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (!paused && !dead && ((float)gameTime.TotalGameTime.TotalSeconds - timeSinceUnpaused > 0.5)) { paused = true; soundEffects[2].Play(); } else 
                { 
                    if ((float)gameTime.TotalGameTime.TotalSeconds - timeSinceLastDraw > 0.5 && !dead)
                    {
                        paused = false;
                        soundEffects[3].Play();
                        timeSinceUnpaused = (float)gameTime.TotalGameTime.TotalSeconds;
                    }

                }
                //numLengths = 60;
                //score = 60;
                //ballSpeed = 400f;
                //splitFactor = 4;
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

            if (((float)gameTime.TotalGameTime.TotalSeconds - timeSinceLastDraw) > speedTime)
            {
                if (!dead && !paused)
                {
                    timeSinceLastDraw = (float)gameTime.TotalGameTime.TotalSeconds;
                    trailPos.Add(ballPosition);
                    trailCount++;
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
                if (!dead && !paused)
                {
                    Die();
                }
            }

            if (berryPickedUp == false && ballPosition.X > (berX - 25) && ballPosition.X < (berX + 25) && ballPosition.Y > (berY - 25) && ballPosition.Y < (berY + 25) && !dead && !paused)
            {
                timeSinceBerryPickup = (float)gameTime.TotalGameTime.TotalSeconds;
                berryPickedUp = true;
                soundEffects[0].Play();
                score++;
                numLengths++;
                if (ballSpeed < 400f)
                {
                    ballSpeed += 5;
                    if (ballSpeed >= 360f)
                    {
                        splitFactor = 4;
                        speedTime = 0.001f;
                    }
                    else if (ballSpeed >= 310f)
                    {
                        splitFactor = 5;
                        speedTime = 0.003f;
                    }
                    else if (ballSpeed >= 260f)
                    {
                        splitFactor = 6;
                        speedTime = 0.005f;
                    }
                    else if (ballSpeed >= 210f)
                    {
                        splitFactor = 7;
                        speedTime = 0.007f;
                    }
                    else if (ballSpeed >= 170f)
                    {
                        splitFactor = 8;
                        //speedTime = 0.012f;
                    }
                    else if (ballSpeed >= 130f)
                    {
                        splitFactor = 9;                        
                    }
                }


                if (score < 10)
                {
                   
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            DrawHead();
            DrawTail(1, 0, gameTime);
            if (dead && (float)gameTime.TotalGameTime.TotalSeconds - timeSinceLastDraw > 1)
            {
                DrawGameOver();
            }

            if (paused)
            {
                DrawPaused();
            }

            if (berryPickedUp == false)
            {
                
                _berryBatch.Begin();
                _berryBatch.Draw(berryTexture, new Vector2(berX, berY), null, Color.White);
                _berryBatch.End();
            }
            else
            {
                if ((float)gameTime.TotalGameTime.TotalSeconds - timeSinceBerryPickup > 1 && !dead && !paused)
                {
                    GenBerryPos();
                    berryPickedUp = false;
                }
            }

            _scoreBatch.Begin();
            _scoreBatch.DrawString(scoreFont, "Score: " + score, new Vector2(10, 10), Color.White);
            _scoreBatch.End();

            base.Draw(gameTime);
        }

        public void spawnSnakePiece(int pos)
        {

        }

        public void Die()
        {
            dead = true;
            soundEffects[1].Play();
            MediaPlayer.Play(gameOverMus);
        }

        public void GenBerryPos()
        {
            Random berryRnd = new Random();
            berX = berryRnd.Next(1, _graphics.PreferredBackBufferWidth - 50);
            berY = berryRnd.Next(1, _graphics.PreferredBackBufferHeight - 50);
        }

        public void DrawGameOver()
        {
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(
                gameOverTexture,
                new Vector2(_graphics.PreferredBackBufferWidth / 2,
_graphics.PreferredBackBufferHeight / 2),
                null,
                Color.White,
                0f,
                new Vector2(gameOverTexture.Width / 2, gameOverTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
                );
            spriteBatch.End();
        }
        public void DrawPaused()
        {
            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(
                pauseTexture,
                new Vector2(_graphics.PreferredBackBufferWidth / 2,
_graphics.PreferredBackBufferHeight / 2),
                null,
                Color.White,
                0f,
                new Vector2(gameOverTexture.Width / 2, gameOverTexture.Height / 2),
                Vector2.One,
                SpriteEffects.None,
                0f
                );
            spriteBatch.End();
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
                }
                else
                {
                        try 
                        { 
                            lengthPosition = trailPos[trailCount - (splitFactor * i)]; 
                        }
                        catch
                        {
                        Console.WriteLine("Length is greater than the capacity of the trail array!");
                        }
                }
                if (i >= 3)
                {
                    if ((lengthPosition.X - ballPosition.X) < 15 && (lengthPosition.Y - ballPosition.Y) < 15 && (lengthPosition.X - ballPosition.X) > -15 && (lengthPosition.Y - ballPosition.Y) > -15 && !dead)
                    {
                        Die();
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