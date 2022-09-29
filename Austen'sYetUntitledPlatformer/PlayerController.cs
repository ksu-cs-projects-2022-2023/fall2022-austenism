using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austen_sYetUntitledPlatformer
{
    internal class PlayerController
    {
        Texture2D texture;
        string texturepath;
        //these are the size of the texture file not on screen
        private int width = 32;
        private int height = 32;

        private Vector2 origin;
        private double animationTimer;
        private short animationFrame = 0;


        public static float Scale
        {
            get;
            private set;
        } = (float)0.4;

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        const float maxVerticalVelocity = 500;
        const float maxHorizontalVelocity = 100;
        const float accelerationDueToGravity = 8;

        public PlayerController(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            origin = new Vector2(width / 2, height / 2);
        }
        public void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("sprite_filler");
        }
        public void Update(GameTime gameTime)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //lets take input next
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.D))
            {//go right
                if (Velocity.X < maxHorizontalVelocity) Velocity += new Vector2(10, 0);
            }

            //how about physics heh
            if (Velocity.Y < maxVerticalVelocity) Velocity += new Vector2(0, accelerationDueToGravity);

            Velocity += Acceleration * t;
            Position += Velocity * t;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //update animation timer
            //animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            //lets get the animation source rectangle
            Rectangle source;
            source = new Rectangle(animationFrame * width + 1, 0, width - 2, height);

            spriteBatch.Draw(texture, Position, source, Color.White, 0, origin, Scale, SpriteEffects.None, 0);

        }
    }
}
