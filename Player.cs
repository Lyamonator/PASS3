using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using System;

namespace PASS3
{
    public class Player
    {
        //private GraphicsDeviceManager _graphics;
        //private SpriteBatch _spriteBatch;
        Texture2D textureDucking;
        Texture2D textureStanding;
        Texture2D textureJumping;
        public Rectangle rectangle;
        public Vector2 position;

        GraphicsDevice graphics;

        public Color[] textureDataDucking;
        public Color[] textureDataStanding;
        public Color[] textureDataJumping;

        public int health;

        public const int DUCKING = 1;
        public const int STANDING = 2;
        public const int JUMPING = 3;

        static public int currentPlayerOrientation = STANDING;

        float gravity = 9.8f / 60f;

        float ySpeed = 0f;

        float jumpSpeed = 6f;

        public static int onGroundY;

        public static int score;

        public static int highScore;

        public static bool newHighScore;

        public Player(int newHealth, int newScore)
        {
            health = newHealth;
            score = newScore;
        }



        public void Load(ContentManager content, GraphicsDevice newGraphics)
        {
            textureDucking = content.Load<Texture2D>("ducking");
            graphics = newGraphics;

            textureDataDucking = new Color[textureDucking.Width * textureDucking.Height];
            textureDucking.GetData(textureDataDucking);

            textureStanding = content.Load<Texture2D>("standing");
            graphics = newGraphics;

            textureDataStanding = new Color[textureStanding.Width * textureStanding.Height];
            textureStanding.GetData(textureDataStanding);

            textureJumping = content.Load<Texture2D>("jumping");
            graphics = newGraphics;

            textureDataJumping = new Color[textureJumping.Width * textureJumping.Height];
            textureJumping.GetData(textureDataJumping);

            onGroundY = 900 - textureJumping.Height;

            position = new Vector2(100, onGroundY);



            // TODO: use this.Content to load your game content here
        }

        public void Update(GameTime gameTime)
        {
            Keyboard.GetState();
            ySpeed += gravity;

            if (Keyboard.HasBeenPressed(Keys.Down) && currentPlayerOrientation != JUMPING)
            {
                if (currentPlayerOrientation == STANDING)
                {
                    currentPlayerOrientation = DUCKING;
                    position.Y += 170;
                }
                else
                {
                    currentPlayerOrientation = STANDING;
                    position.Y -= 170;
                }

            }

            switch (currentPlayerOrientation)
            {
                case DUCKING:
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        ySpeed = -jumpSpeed;
                        currentPlayerOrientation = JUMPING;
                    }
                    rectangle = new Rectangle((int)position.X, (int)position.Y, textureDucking.Width, textureDucking.Height);
                    break;
                case STANDING:
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        ySpeed = -jumpSpeed;
                        currentPlayerOrientation = JUMPING;
                    }
                    rectangle = new Rectangle((int)position.X, (int)position.Y, textureStanding.Width, textureStanding.Height);
                    break;
                case JUMPING:
                    position.Y += ySpeed;
                    if (position.Y >= onGroundY)
                    {
                        ySpeed = 0f;
                        position.Y = onGroundY;
                        currentPlayerOrientation = STANDING;
                    }
                    rectangle = new Rectangle((int)position.X, (int)position.Y, textureJumping.Width, textureJumping.Height);
                    break;
            }

        }


        public void Draw(SpriteBatch sprite)
        {
            switch (currentPlayerOrientation)
            {
                case DUCKING:
                    sprite.Draw(
                    textureDucking,
                    position,
                    Color.White
                    );
                    break;
                case STANDING:
                    sprite.Draw(
                    textureStanding,
                    position,
                    Color.White
                    );
                    break;
                case JUMPING:
                    sprite.Draw(
                    textureJumping,
                    position,
                    Color.White
                    );
                    break;
            }
        }
    }
}
