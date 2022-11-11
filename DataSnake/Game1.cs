using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Threading.Tasks.Sources;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

namespace DataSnake
{
    public class Game1 : Game
    {

        Texture2D ballTexture;
        Texture2D berryTexture;
        Texture2D gameOverTexture;
        Texture2D pauseTexture;
        SpriteFont scoreFont;
        Vector2 ballPosition;
        Vector2 lengthPosition;
        int score;
        int highScore;
        int berX;
        int berY;
        int splitFactor = 10;
        int pageNum = 0;
        //1 = Up, 2 = Down, 3 = Left, 4 = Right 
        int curDirection = 4;
        int lastDirection = 4;
        int trailCount = 0;
        int numLengths = 0;
        float ballSpeed = 100f;
        float speedTime = 0.015f;
        float timeSinceLastDraw = 0;
        float timeSinceBerryPickup = 0;
        float timeSinceUnpaused = 0;
        float spaceSlowdown = 0;
        List<Vector2> trailPos = new List<Vector2>();
        List<SoundEffect> soundEffects = new List<SoundEffect>();
        Song gameOverMus;
        bool berryPickedUp = false;
        bool upPressed;
        bool downPressed;
        bool leftPressed;
        bool rightPressed;
        bool dead = false;
        bool paused;
        bool restarting;
        bool drawingLog;
        bool lineEnded;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteBatch _berryBatch;
        private SpriteBatch _scoreBatch;
        private SpriteBatch _logBatch;
        private SpriteBatch _highScoreBatch;
        private SpriteBatch _snakeTail;
        private SpriteBatch _pauseSprite;
        private SpriteBatch _gameOverSprite;

        Resources _resources = new Resources();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
_graphics.PreferredBackBufferHeight / 2);
            highScore = ReadScore();
            GenBerryPos();
            base.Initialize();
            WriteToLog("Game initialized.");
        }

        public static void WriteToLog(string logData)
        {
            if (!File.Exists("log.txt"))
            {
                using (StreamWriter sw = File.CreateText("log.txt"))
                {
                    sw.WriteLine(DateAndTime.Now + " - Log file created at " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\log.txt");
                    sw.WriteLine(DateAndTime.Now + " - " + logData);
                }
            }
            else
            {
                using StreamWriter file = new("log.txt", append: true);
                file.WriteLineAsync(DateAndTime.Now + " - " + logData);
            }
        }

        public static void WriteScore(int score)
        {
            string text = score.ToString();
            File.WriteAllTextAsync("highscore.txt", text);
            WriteToLog("High score " + text + " written to file " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\highscore.txt");
        }

        public static int ReadScore()
        {
            if (!File.Exists("highscore.txt"))
            {
                using (StreamWriter sw = File.CreateText("highscore.txt"))
                {
                    sw.WriteLine("0");
                    WriteToLog("High score file created with 0 value at " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\highscore.txt");
                    return 0;
                }
            } 
            else
            {
                using (StreamReader sr = File.OpenText("highscore.txt"))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        WriteToLog("High score " + s + " read from file " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\highscore.txt");
                        return Int32.Parse(s);
                    }
                }
            }

            WriteToLog("Unexpected code path in ReadScore method, returning 0.");
            return 0;
        }

        protected override void LoadContent()
        {
            //Initialize Sprite Batches
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _berryBatch = new SpriteBatch(GraphicsDevice);
            _scoreBatch = new SpriteBatch(GraphicsDevice);
            _highScoreBatch = new SpriteBatch(GraphicsDevice);
            _snakeTail = new SpriteBatch(GraphicsDevice);
            _pauseSprite = new SpriteBatch(GraphicsDevice);
            _gameOverSprite = new SpriteBatch(GraphicsDevice);
            _logBatch = new SpriteBatch(GraphicsDevice);

            //Textures
            ballTexture = _resources.LoadTexture("ball", this);
            berryTexture = _resources.LoadTexture("berry", this);
            gameOverTexture = _resources.LoadTexture("gameover", this);
            pauseTexture = _resources.LoadTexture("paused", this);

            //SFX
            soundEffects.Add(_resources.LoadSFX("pickup", this));
            soundEffects.Add(_resources.LoadSFX("diesfx", this));
            soundEffects.Add(_resources.LoadSFX("sm64_pause", this));
            soundEffects.Add(_resources.LoadSFX("unpause", this));

            //Music
            this.gameOverMus = _resources.LoadSong("gameoversfx", this);

            //Font
            scoreFont = _resources.LoadFont("defaultfont", this);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (!paused && !dead && !restarting && !drawingLog && ((float)gameTime.TotalGameTime.TotalSeconds - timeSinceUnpaused > 0.5)) { paused = true; soundEffects[2].Play(); WriteToLog("Game paused."); }
                else if (dead && !restarting && !drawingLog) { Restart(gameTime); }
                else
                {
                    if ((float)gameTime.TotalGameTime.TotalSeconds - timeSinceLastDraw > 0.5 && !dead && !restarting && !drawingLog)
                    {
                        paused = false;
                        WriteToLog("Game unpaused.");
                        soundEffects[3].Play();
                        timeSinceUnpaused = (float)gameTime.TotalGameTime.TotalSeconds;
                    }

                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F4))
            {
                if (dead | paused) { WriteToLog("Game exiting with F4."); Exit(); }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                if ((dead | paused) && !drawingLog && ((float)gameTime.TotalGameTime.TotalSeconds - timeSinceUnpaused > 0.5)) 
                { 
                    WriteToLog("Displaying log.");
                    timeSinceUnpaused = (float)gameTime.TotalGameTime.TotalSeconds;
                    drawingLog = true; 
                }
                else if (drawingLog == true && (float)gameTime.TotalGameTime.TotalSeconds - timeSinceUnpaused > 0.5)
                {
                    timeSinceUnpaused = (float)gameTime.TotalGameTime.TotalSeconds;
                    pageNum = 0;
                    drawingLog = false;
                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (drawingLog == true)
                {
                    if (spaceSlowdown == 0)
                    {
                        spaceSlowdown = (float)gameTime.TotalGameTime.TotalSeconds;
                    }
                    else if ((float) gameTime.TotalGameTime.TotalSeconds - spaceSlowdown > 0.2)
                    {
                        spaceSlowdown = (float)gameTime.TotalGameTime.TotalSeconds;
                        if (!lineEnded)
                        {
                            pageNum++;
                        }
                        else
                        {
                            pageNum = 0;
                            lineEnded = false;
                        }
                    }
                }
            }

            if (restarting)
            {
                if ((float)gameTime.TotalGameTime.TotalSeconds - timeSinceUnpaused > 0.5)
                {
                    restarting = false;
                }
            }

            if (score > highScore)
            {
                highScore = score;
            }

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up) && curDirection != 2)
            {
                if (!downPressed && !leftPressed && !rightPressed)
                {
                    upPressed = true;
                    lastDirection = curDirection;                  
                    curDirection = 1;
                }
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
                    lastDirection = curDirection;
                    curDirection = 2;
                }
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
                    lastDirection = curDirection;
                    curDirection = 3;
                }
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
                    lastDirection = curDirection;
                    curDirection = 4;
                }
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
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);
            if (!paused)
            {
                DrawHead();
                DrawTail();
            }
            else
            {
                if (!drawingLog)
                {
                    DrawPaused();
                }
            }

            if (dead && (float)gameTime.TotalGameTime.TotalSeconds - timeSinceLastDraw > 1)
            {
                if (!drawingLog)
                {
                    DrawGameOver();
                }
            }

            if (berryPickedUp == false && !paused)
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

            _highScoreBatch.Begin();
            _highScoreBatch.DrawString(scoreFont, "High Score: " + highScore, new Vector2(10, 30), Color.White);
            _highScoreBatch.End();

            if (drawingLog == true)
            {
                _logBatch.Begin();
                string line = "default";
                int posY = 30;

                for (int i = 1; i < 21; i++)
                {
                    line = File.ReadLines("log.txt").ElementAtOrDefault((pageNum * 20) + i - 1);
                    posY += 20;
                    if (line == null) 
                    {
                        lineEnded = true;
                        _logBatch.DrawString(scoreFont, "End of log reached.", new Vector2(10, posY), Color.White);
                        i = 20;
                    }
                    else
                    {
                        _logBatch.DrawString(scoreFont, line, new Vector2(10, posY), Color.White);
                    }
                }
                _logBatch.DrawString(scoreFont, "Press space to continue.", new Vector2(10, posY + 20), Color.White);
                _logBatch.End();
            }

            base.Draw(gameTime);
        }

        public void Die()
        {
            dead = true;
            WriteToLog("Player died with score " + score.ToString());
            soundEffects[1].Play();
            MediaPlayer.Play(gameOverMus);
            int currentHighScore = ReadScore();
            if (currentHighScore < score)
            {
                WriteScore(score);
            }
            
        }

        public void Restart(GameTime gameTime)
        {
            WriteToLog("Game restarting.");
            restarting = true;
            score = 0;
            splitFactor = 10;
            curDirection = 4;
            lastDirection = 4;
            trailCount = 0;
            numLengths = 0;
            ballSpeed = 100f;
            speedTime = 0.015f;
            timeSinceLastDraw = 0;
            timeSinceBerryPickup = 0;
            timeSinceUnpaused = (float)gameTime.TotalGameTime.TotalSeconds;
            trailPos = new List<Vector2>();
            berryPickedUp = false;
            upPressed = false;
            downPressed = false;
            leftPressed = false;
            rightPressed = false;
            dead = false;
            MediaPlayer.Stop();
            ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
_graphics.PreferredBackBufferHeight / 2);
            GenBerryPos();
            base.Initialize();
        }

        public void GenBerryPos()
        {
            Random berryRnd = new Random();
            berX = berryRnd.Next(1, _graphics.PreferredBackBufferWidth - 50);
            berY = berryRnd.Next(1, _graphics.PreferredBackBufferHeight - 50);
            WriteToLog("Berry getting generated at X: " + berX.ToString() + " Y: " + berY.ToString());
        }

        public void DrawGameOver()
        {
            _gameOverSprite.Begin();
            _gameOverSprite.Draw(
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
            _gameOverSprite.End();
        }
        public void DrawPaused()
        {
            _pauseSprite.Begin();
            _pauseSprite.Draw(
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
            _pauseSprite.End();
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

        public void DrawTail()
        {
            for (int i = 1; i <= numLengths; i++)
            {
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
                _snakeTail.Begin();
                _snakeTail.Draw(
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
                _snakeTail.End();

                }

        }
    }
}