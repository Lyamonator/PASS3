using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PASS3
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private const int MENU = 0;
        private const int SETTINGS = 1;
        private const int INSTRUCTIONS = 2;
        private const int GAMEPLAY = 3;
        private const int PAUSE = 4;
        private const int ENDGAME = 5;

        public int gameState = MENU;

        const int MNU_PLAY = 1;
        const int MNU_INSTRUCTIONS = 2;
        const int MNU_EXIT = 3;

        const int NUM_OPTIONS = 3;

        public int menuChoice = MNU_PLAY;

        int obstacleType;

        const int WALL = 1;

        const int HIGH = 2;

        const int LOW = 3;

        const int OBSTACLE_SPEED = 13;

        const float DELTA_TIME = 16.66666f;

        int timer = 1500;

        Vector2 playLocation;
        Texture2D playTexture;

        Vector2 exitLocation;
        Texture2D exitTexture;

        Vector2 instructionsLocation;
        Texture2D instructionsTexture;

        Vector2 pointerLocation;
        Texture2D pointerTexture;

        Texture2D healthTexture;
        Rectangle healthRectangle;

        Texture2D spawnTexture;
        Rectangle spawnRectangle;

        SpriteFont Arial;
        SpriteFont BigArial;

        Player player;

        Projectile projectile;

        Random rng = new Random();

        Texture2D wall;
        List<Rectangle> wallRectangles = new List<Rectangle>();
        List<Vector2> walls = new List<Vector2>();
        Color[] wallData;

        Texture2D high;
        List<Rectangle> highRectangles = new List<Rectangle>();
        List<Vector2> highs = new List<Vector2>();
        Color[] highData;

        Texture2D low;
        List<Rectangle> lowRectangles = new List<Rectangle>();
        List<Vector2> lows = new List<Vector2>();
        Color[] lowData;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1800;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Artwork by me in google drawings (except red and orange are from google)
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            playTexture = Content.Load<Texture2D>("play");
            playLocation = new Vector2(_graphics.PreferredBackBufferWidth / 4 - playTexture.Width / 2, _graphics.PreferredBackBufferHeight / 2 - playTexture.Height);

            instructionsTexture = Content.Load<Texture2D>("instructions");
            instructionsLocation = new Vector2(_graphics.PreferredBackBufferWidth / 2 - instructionsTexture.Width / 2, _graphics.PreferredBackBufferHeight / 2 - instructionsTexture.Height);

            exitTexture = Content.Load<Texture2D>("exit");
            exitLocation = new Vector2(_graphics.PreferredBackBufferWidth / 4 * 3 - exitTexture.Width / 2, _graphics.PreferredBackBufferHeight / 2 - exitTexture.Height);

            pointerTexture = Content.Load<Texture2D>("pointer");
            pointerLocation = new Vector2(_graphics.PreferredBackBufferWidth / 4 - playTexture.Width, playLocation.Y + playTexture.Height + pointerTexture.Height);

            Arial = Content.Load<SpriteFont>("Arial");
            BigArial = Content.Load<SpriteFont>("BigArial");

            player = new Player(3, 0);
            player.Load(Content, GraphicsDevice);

            projectile = new Projectile(player.position.Y, player.position.X);
            projectile.Load(Content, GraphicsDevice);

            wall = Content.Load<Texture2D>("wall");
            wallData = new Color[wall.Width * wall.Height];
            wall.GetData(wallData);

            high = Content.Load<Texture2D>("high");
            highData = new Color[high.Width * high.Height];
            high.GetData(highData);

            low = Content.Load<Texture2D>("low");
            lowData = new Color[low.Width * low.Height];
            low.GetData(lowData);

            healthTexture = Content.Load<Texture2D>("red");
            healthRectangle = new Rectangle(375, 135, player.health * 100, 20);

            spawnTexture = Content.Load<Texture2D>("orange");
            spawnRectangle = new Rectangle(375, 200, timer / 5, 20);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            // Exit();

            switch (gameState)
            {
                case MENU:
                    UpdateMenu(gameTime);
                    break;
                case SETTINGS:
                    //Get and apply changes to game settings
                    break;
                case INSTRUCTIONS:
                    UpdateInstructions(gameTime);
                    //Get user input to return to MENU
                    break;
                case GAMEPLAY:

                    UpdateGameplay(gameTime);
                    //Implement standared game logic (input, update game objects, apply physics, collision detection)
                    break;
                case PAUSE:
                    //Get user input to resume the game
                    break;
                case ENDGAME:
                    UpdateEndgame(gameTime);
                    //Wait for final input based on end of game options (end, restart, etc.)
                    break;
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (gameState)
            {
                case MENU:
                    //Draw the possible menu options
                    DrawMenu(gameTime);
                    break;
                case SETTINGS:
                    //Draw the settings with prompts
                    break;
                case INSTRUCTIONS:
                    //Draw the game instructions including prompt to return to MENU
                    DrawInstructions(gameTime);
                    break;
                case GAMEPLAY:
                    DrawGameplay(gameTime);
                    //Draw all game objects on each layers (background, middleground, foreground and user interface)
                    break;
                case PAUSE:
                    //Draw the pause screen, this may include the full game drawing behind
                    break;
                case ENDGAME:
                    DrawEndgame(gameTime);
                    //Draw the final feedback and prompt for available options (exit,restart, etc.)
                    break;
            }

            base.Draw(gameTime);
        }

        //https://youtu.be/asU7afngQ8U
        static bool IntersectsPixel(Rectangle rect1, Color[] data1, Rectangle rect2, Color[] data2)
        {
            int top = MathHelper.Max(rect1.Top, rect2.Top);
            int bottom = MathHelper.Min(rect1.Bottom, rect2.Bottom);
            int left = MathHelper.Max(rect1.Left, rect2.Left);
            int right = MathHelper.Min(rect1.Right, rect2.Right);

            for (int y = top; y < bottom; y++)
                for (int x = left; x < right; x++)
                {
                    Color colour1 = data1[(x - rect1.Left) +
                        (y - rect1.Top) * rect1.Width];
                    Color colour2 = data2[(x - rect2.Left) + (y - rect2.Top) * rect2.Width];

                    if (colour1.A != 0 && colour2.A != 0)
                        return true;
                }
            return false;
        }
        protected void UpdateMenu(GameTime gameTime)
        {
            Keyboard.GetState();


            if (Keyboard.HasBeenPressed(Keys.Right))
            {
                if (menuChoice < NUM_OPTIONS)
                {
                    menuChoice++;
                    pointerLocation.X += _graphics.PreferredBackBufferWidth / 4 + 20;
                }
            }
            else if (Keyboard.HasBeenPressed(Keys.Left))
            {
                if (menuChoice > 1)
                {
                    menuChoice--;
                    pointerLocation.X -= _graphics.PreferredBackBufferWidth / 4 + 20;
                }
            }
            else if (Keyboard.HasBeenPressed(Keys.Enter))
            {
                switch (menuChoice)
                {
                    case MNU_PLAY:
                        gameState = GAMEPLAY;
                        break;
                    case MNU_INSTRUCTIONS:
                        gameState = INSTRUCTIONS;
                        break;
                    case MNU_EXIT:
                        Exit();
                        break;
                }
            }
        }
        protected void DrawMenu(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(playTexture, playLocation, Color.White);
            _spriteBatch.Draw(instructionsTexture, instructionsLocation, Color.White);
            _spriteBatch.Draw(exitTexture, exitLocation, Color.White);
            _spriteBatch.Draw(pointerTexture, pointerLocation, Color.White);
            _spriteBatch.DrawString(BigArial, "Runner", new Vector2(_graphics.PreferredBackBufferWidth / 2 - 200, 0), Color.Red);
            _spriteBatch.End();
        }
        protected void UpdateInstructions(GameTime gameTime)
        {
            Keyboard.GetState();
            if (Keyboard.HasBeenPressed(Keys.Enter))
            {
                gameState = MENU;
            }
        }
        protected void DrawInstructions(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(Arial, "How To Play", new Vector2(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 4), Color.White);
            _spriteBatch.DrawString(Arial, "Up = Jump\nDown = Duck\nSpace = Throw", new Vector2(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 3), Color.Blue);
            _spriteBatch.DrawString(Arial, "Stay Alive!", new Vector2(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 2), Color.Yellow);
            _spriteBatch.DrawString(Arial, "Press <Enter> to return to the Menu", new Vector2(_graphics.PreferredBackBufferWidth / 3, _graphics.PreferredBackBufferHeight / 1.2f), Color.Green);
            _spriteBatch.End();
        }
        protected void UpdateGameplay(GameTime gameTime)
        {
            player.Update(gameTime);
            projectile.playerHeight = player.position.Y;
            projectile.playerWidth = player.position.X;
            projectile.Update(gameTime);
            timer -= (int)DELTA_TIME;
            if (timer <= 0)
            {
                timer = 1500;

                obstacleType = rng.Next(1, 7);
                switch (obstacleType)
                {
                    case WALL:
                        walls.Add(new Vector2(_graphics.PreferredBackBufferWidth - wall.Width, 475));
                        wallRectangles.Add(new Rectangle((int)(walls[walls.Count - 1].X - (wall.Width / 2)),
              (int)(walls[walls.Count - 1].Y - (wall.Height / 2)), wall.Width, wall.Height));
                        break;
                    case HIGH:
                        highs.Add(new Vector2(_graphics.PreferredBackBufferWidth - high.Width, 500));
                        highRectangles.Add(new Rectangle((int)(highs[highs.Count - 1].X - (high.Width / 2)),
              (int)(highs[highs.Count - 1].Y - (high.Height / 2)), high.Width, high.Height));
                        break;
                    case LOW:
                        lows.Add(new Vector2(_graphics.PreferredBackBufferWidth - low.Width, 850));
                        lowRectangles.Add(new Rectangle((int)(lows[lows.Count - 1].X - (low.Width / 2)),
              (int)(lows[lows.Count - 1].Y - (low.Height / 2)), low.Width, low.Height));
                        break;
                }
            }
            for (int i = 0; i < walls.Count; i++)
            {
                Vector2 wall = walls[i];
                wall.X -= OBSTACLE_SPEED;
                walls.RemoveAt(i);
                walls.Insert(i, wall);
                Rectangle rect = wallRectangles[i];
                rect.X -= OBSTACLE_SPEED;
                wallRectangles.RemoveAt(i);
                wallRectangles.Insert(i, rect);
                rect.Y += 45;
                if (Projectile.projTimer > 0 && projectile.rectangle.Intersects(wallRectangles[i]) && IntersectsPixel(projectile.rectangle, projectile.textureData, rect, wallData))
                {
                    walls.RemoveAt(i);
                    wallRectangles.RemoveAt(i);
                    Projectile.projTimer = 0;
                    Player.score += 10;
                }
                else
                {
                    if (Player.currentPlayerOrientation == Player.DUCKING)
                    {
                        if (player.rectangle.Intersects(wallRectangles[i]) && IntersectsPixel(player.rectangle, player.textureDataDucking, rect, wallData))
                        {
                            player.health -= 1;
                            walls.RemoveAt(i);
                            wallRectangles.RemoveAt(i);
                        }
                    }
                    else if (Player.currentPlayerOrientation == Player.STANDING)
                    {
                        if (player.rectangle.Intersects(wallRectangles[i]) && IntersectsPixel(player.rectangle, player.textureDataStanding, rect, wallData))
                        {
                            player.health -= 1;
                            walls.RemoveAt(i);
                            wallRectangles.RemoveAt(i);
                        }
                    }
                    else if (Player.currentPlayerOrientation == Player.JUMPING)
                    {
                        if (player.rectangle.Intersects(wallRectangles[i]) && IntersectsPixel(player.rectangle, player.textureDataJumping, rect, wallData))
                        {
                            player.health -= 1;
                            walls.RemoveAt(i);
                            wallRectangles.RemoveAt(i);
                        }

                    }
                }
            }

            for (int i = 0; i < highs.Count; i++)
            {
                Vector2 high = highs[i];
                high.X -= OBSTACLE_SPEED;
                highs.RemoveAt(i);
                highs.Insert(i, high);
                Rectangle rect = highRectangles[i];
                rect.X -= OBSTACLE_SPEED;
                highRectangles.RemoveAt(i);
                highRectangles.Insert(i, rect);
                rect.Y += 45;
                if (highs[i].X <= 0)
                {
                    highs.RemoveAt(i);
                    highRectangles.RemoveAt(i);
                    Player.score += 10;
                }
                else
                {
                    if (Player.currentPlayerOrientation == Player.STANDING)
                    {
                        if (player.rectangle.Intersects(highRectangles[i]) && IntersectsPixel(player.rectangle, player.textureDataStanding, rect, highData))
                        {
                            player.health -= 1;
                            highs.RemoveAt(i);
                            highRectangles.RemoveAt(i);
                        }
                    }
                    else if (Player.currentPlayerOrientation == Player.JUMPING)
                    {
                        if (player.rectangle.Intersects(highRectangles[i]) && IntersectsPixel(player.rectangle, player.textureDataJumping, rect, highData))
                        {
                            player.health -= 1;
                            highs.RemoveAt(i);
                            highRectangles.RemoveAt(i);
                        }
                    }
                }
            }



            for (int i = 0; i < lows.Count; i++)
            {
                Vector2 low = lows[i];
                low.X -= OBSTACLE_SPEED;
                lows.RemoveAt(i);
                lows.Insert(i, low);
                Rectangle rect = lowRectangles[i];
                rect.X -= OBSTACLE_SPEED;
                lowRectangles.RemoveAt(i);
                lowRectangles.Insert(i, rect);
                rect.Y += 45;
                rect.X += 90;
                if (lows[i].X <= 0)
                {
                    lows.RemoveAt(i);
                    lowRectangles.RemoveAt(i);
                    Player.score += 10;
                }
                else
                {
                    if (Player.currentPlayerOrientation == Player.DUCKING)
                    {
                        if (player.rectangle.Intersects(lowRectangles[i]) && IntersectsPixel(player.rectangle, player.textureDataDucking, rect, lowData))
                        {
                            player.health -= 1;
                            lows.RemoveAt(i);
                            lowRectangles.RemoveAt(i);
                        }
                    }
                    else if (Player.currentPlayerOrientation == Player.STANDING)
                    {
                        if (player.rectangle.Intersects(lowRectangles[i]) && IntersectsPixel(player.rectangle, player.textureDataStanding, rect, lowData))
                        {
                            player.health -= 1;
                            lows.RemoveAt(i);
                            lowRectangles.RemoveAt(i);
                        }
                    }
                    //if they didn't jump high enough yet or all falling they can still intersect
                    else if (Player.currentPlayerOrientation == Player.JUMPING)
                    {
                        if (player.rectangle.Intersects(lowRectangles[i]) && IntersectsPixel(player.rectangle, player.textureDataJumping, rect, lowData))
                        {
                            player.health -= 1;
                            lows.RemoveAt(i);
                            lowRectangles.RemoveAt(i);
                        }
                    }
                }
            }


            //if gameend, player.position = original position
            //player.orientation = standing
            if (player.health <= 0)
            {
                gameState = ENDGAME;
                player.health = 3;
                Projectile.projTimer = 0;
                Player.currentPlayerOrientation = Player.STANDING;
                player.position = new Vector2(100, Player.onGroundY);

                while (walls.Count > 0)
                {
                    walls.RemoveAt(0);
                }
                while (wallRectangles.Count > 0)
                {
                    wallRectangles.RemoveAt(0);
                }
                while (highs.Count > 0)
                {
                    highs.RemoveAt(0);
                }
                while (highRectangles.Count > 0)
                {
                    highRectangles.RemoveAt(0);
                }
                while (lows.Count > 0)
                {
                    lows.RemoveAt(0);
                }
                while (lowRectangles.Count > 0)
                {
                    lowRectangles.RemoveAt(0);
                }
            }
            if (Player.score > Player.highScore)
            {
                Player.highScore = Player.score;
                Player.newHighScore = true;
            }
            
            healthRectangle.Width = player.health * 100;
            spawnRectangle.Width = timer / 5;
        }
        protected void DrawGameplay(GameTime gameTime)
        {
            _spriteBatch.Begin();
            player.Draw(_spriteBatch);
            projectile.Draw(_spriteBatch);

            for (int i = 0; i < walls.Count; i++)
            {
                _spriteBatch.Draw(wall, walls[i], Color.Gray);
            }

            for (int i = 0; i < highs.Count; i++)
            {
                _spriteBatch.Draw(high, highs[i], Color.Red);
            }

            for (int i = 0; i < lows.Count; i++)
            {
                _spriteBatch.Draw(low, lows[i], Color.Orange);
            }
            _spriteBatch.DrawString(Arial, "Score: " + Player.score, new Vector2(0, 0), Color.Cyan);
            _spriteBatch.DrawString(Arial, "High Score: " + Player.highScore, new Vector2(0, 60), Color.White);
            _spriteBatch.DrawString(Arial, "Health Points: " + player.health + "/3", new Vector2(0, 120), Color.Red);
            _spriteBatch.DrawString(Arial, "Spawn Timer: ", new Vector2(0, 180), Color.Orange);
            _spriteBatch.Draw(spawnTexture, spawnRectangle, Color.White);
            _spriteBatch.Draw(healthTexture, healthRectangle, Color.White);

            _spriteBatch.End();
        }
        protected void UpdateEndgame(GameTime gameTime)
        {
            Keyboard.GetState();
            if (Keyboard.HasBeenPressed(Keys.Enter))
            {
                gameState = MENU;
                Player.score = 0;
                Player.newHighScore = false;
            }
        }
        protected void DrawEndgame(GameTime gameTime)
        {
            _spriteBatch.Begin();
            if (Player.newHighScore)
            {
                _spriteBatch.DrawString(Arial, "Congratulations! New High Score!", new Vector2(_graphics.PreferredBackBufferWidth/2 - 350, _graphics.PreferredBackBufferHeight/1.2f), Color.Yellow);
            }
            _spriteBatch.DrawString(Arial, "Press <Enter> to return to the Menu", new Vector2(_graphics.PreferredBackBufferWidth / 2 - 350, _graphics.PreferredBackBufferHeight / 2 + 100), Color.Green);
            _spriteBatch.DrawString(Arial, "Final score: " + Player.score, new Vector2(_graphics.PreferredBackBufferWidth / 2 - 350, _graphics.PreferredBackBufferHeight / 2), Color.White);
            _spriteBatch.End();
        }
    }
}
