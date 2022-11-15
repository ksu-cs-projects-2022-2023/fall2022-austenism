using Austen_sYetUntitledPlatformer.Collisions;
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
        //these are the size of the texture file not whats on screen
        private int texturewidth = 64;
        private int textureheight = 96;
        private int width;
        private int height;

        private Vector2 origin;
        private double animationTimer;
        private short animationFrame = 0;

        private bool colliding = false;
        private List<BoundingRectangle> collides = new List<BoundingRectangle>();
        private Vector2 previousPosition;



        public static float Scale
        {
            get;
            private set;
        } = (float)0.4;

        public Vector2 Position;
        public BoundingRectangle PlayerCollisionBox;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        const float maxVerticalVelocity = 1000;
        const float maxHorizontalVelocity = 1000;
        const float accelerationDueToGravity = 1000;

        public PlayerController(Vector2 position)
        {
            Position = position;
            width = (int)(texturewidth * Scale);
            height = (int)(textureheight * Scale);
            PlayerCollisionBox = new BoundingRectangle(Position.X, Position.Y, height, width);
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            //origin = new Vector2(width / 2, height);
        }
        public void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("newsprite_filler");
        }
        public void Update(GameTime gameTime, List<Collisions.BoundingRectangle> levelCollision)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //lets take input next
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.D))
            {//go right
                if (Velocity.X < maxHorizontalVelocity) Velocity += new Vector2(10, 0);
            }
            if (keyboard.IsKeyDown(Keys.A))
            {//go left
                if (Velocity.X > -maxHorizontalVelocity) Velocity += new Vector2(-10, 0);
            }
            if (keyboard.IsKeyDown(Keys.W))
            { //go up
                if (Velocity.Y > -maxVerticalVelocity) Velocity += new Vector2(0, -10);
            }
            if (keyboard.IsKeyDown(Keys.S))
            {//go down
                if (Velocity.Y < maxVerticalVelocity) Velocity += new Vector2(0, 10);
            }

            if (!keyboard.IsKeyDown(Keys.D) && !keyboard.IsKeyDown(Keys.A) && Velocity.X != 0)
            {//slowdown if not pressing either direction
                if (Velocity.X < 0)
                    Velocity.X += 10;
                if (Velocity.X > 0)
                    Velocity.X -= 10;
            }
            if (!keyboard.IsKeyDown(Keys.W) && !keyboard.IsKeyDown(Keys.S) && Velocity.Y != 0)
            {//slowdown if not pressing either direction
                if (Velocity.Y < 0)
                    Velocity.Y += 10;
                if (Velocity.Y > 0)
                    Velocity.Y -= 10;
            }

            //how about physics heh
            //if (Velocity.Y < maxVerticalVelocity) Acceleration.Y = accelerationDueToGravity;

            Velocity += Acceleration * t;


            Position += Velocity * t;
            PlayerCollisionBox.X = Position.X;
            PlayerCollisionBox.Y = Position.Y;
            colliding = false;

            collides = new List<BoundingRectangle>();
            foreach(BoundingRectangle b in levelCollision)
            {
                if (PlayerCollisionBox.CollidesWith(b))
                {
                    colliding = true;
                    collides.Add(b);
                }
            }
            //order them from closest to furthest
            bool ordered = false;
            while (!ordered && collides.Count > 0)
            {
                ordered = true;
                for (int i = 0; i < collides.Count - 1; i++)
                {
                    float distance1 = (float)Math.Abs(Math.Sqrt(Math.Pow((collides[i].X - PlayerCollisionBox.X), 2) + Math.Pow((collides[i].Y - PlayerCollisionBox.Y), 2)));
                    float distance2 = (float)Math.Abs(Math.Sqrt(Math.Pow((collides[i + 1].X - PlayerCollisionBox.X), 2) + Math.Pow((collides[i + 1].Y - PlayerCollisionBox.Y), 2)));
                    if (distance1 > distance2)
                    {
                        BoundingRectangle temp = collides[i];
                        collides[i] = collides[i + 1];
                        collides[i + 1] = temp;
                        ordered = false;
                    }
                }
            }
            //run through all those tiles now and attempt to resolve it
            foreach (BoundingRectangle b in collides)
            {
                if(PlayerCollisionBox.CollidesWith(b))
                {
                    float overlapTop = b.Bottom - PlayerCollisionBox.Top;
                    float overlapBottom = b.Top - PlayerCollisionBox.Bottom;
                    float overlapLeft = b.Right - PlayerCollisionBox.Left;
                    float overlapRight = b.Left - PlayerCollisionBox.Right;

                    if (Math.Abs(overlapTop) < Math.Abs(overlapBottom) && //ifTop is the smallest
                        Math.Abs(overlapTop) < Math.Abs(overlapLeft) &&
                        Math.Abs(overlapTop) < Math.Abs(overlapRight))
                    {
                        Position.Y += overlapTop;
                        Velocity.Y = 0;
                    }
                    else if (Math.Abs(overlapBottom) < Math.Abs(overlapLeft) &&  //if bottom is the smallest
                             Math.Abs(overlapBottom) < Math.Abs(overlapRight) &&
                             Math.Abs(overlapBottom) < Math.Abs(overlapTop))
                    {
                        Position.Y += overlapBottom;
                        Velocity.Y = 0;
                    }
                    else if (Math.Abs(overlapLeft) < Math.Abs(overlapRight) && //if left is smallest
                             Math.Abs(overlapLeft) < Math.Abs(overlapTop) &&
                             Math.Abs(overlapLeft) < Math.Abs(overlapBottom))
                    {
                        Position.X += overlapLeft;
                        Velocity.X = 0;
                    }
                    else//if right is the smallest
                    {
                        Position.X += overlapRight;
                        Velocity.X = 0;
                    }




                    PlayerCollisionBox.X = Position.X;
                    PlayerCollisionBox.Y = Position.Y;
                }
            }



            

        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //update animation timer
            //animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            //lets get the animation source rectangle
            Rectangle source;
            source = new Rectangle(animationFrame * texturewidth + 1, 0, texturewidth - 2, textureheight);

            //if (colliding) //THIS IS FOR DEBUGGING COLLISION
            //{
            //    spriteBatch.Draw(texture, Position, source, Color.Red, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            //    foreach (BoundingRectangle b in collides)
            //        spriteBatch.Draw(texture, new Vector2(b.X, b.Y), source, Color.Red, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            //}
            else
                spriteBatch.Draw(texture, Position, source, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }


    }
}
