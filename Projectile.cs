using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PASS3
{
    public class Projectile
    {
        //private GraphicsDeviceManager _graphics;
        //private SpriteBatch _spriteBatch;

        GraphicsDevice graphics;

        Texture2D texture;

        public Color[] textureData;

        public Rectangle rectangle;

        public Vector2 position;

        public float playerHeight;

        public float playerWidth;

        float throwSpeed = 0.8f;
        float xProjSpeed = 0f;
        float yProjSpeed = 0f;
        //collision: if(. . . && Projectile.projTimer>0)
        //Projectile.projTimer
        static public float projTimer = 0f;

        float gravity = 9.8f / 60f;

        const float DELTA_TIME = 16.66666f;

        public Projectile(float newPlayerHeight, float newPlayerWidth)
        {
            playerHeight = newPlayerHeight;
            playerWidth = newPlayerWidth;
        }

        public void Load(ContentManager content, GraphicsDevice newGraphics)
        {

            texture = content.Load<Texture2D>("projectile");
            graphics = newGraphics;

            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);

            position = new Vector2(2000, 2000);

        }

        public void Update(GameTime gameTime)
        {

            yProjSpeed += gravity;

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && projTimer <= 0)
            {
                position = new Vector2(100 + playerWidth, playerHeight);
                xProjSpeed = 8 * throwSpeed;
                yProjSpeed = -throwSpeed;
                projTimer = 1;

            }
            else if (position.Y > 800)
            {
                yProjSpeed = 0;
                xProjSpeed = 0;
            }
            if (projTimer > 0)
            {
                projTimer -= DELTA_TIME / 1000;
                position.Y += yProjSpeed;
                position.X += xProjSpeed;
            }
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public void Draw(SpriteBatch sprite)
        {
            if (projTimer > 0)
            {
                sprite.Draw(
                        texture,
                        position,
                        Color.White
                        );
            }
        }
    }
}

