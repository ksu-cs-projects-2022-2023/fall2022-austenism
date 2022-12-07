using Austen_sYetUntitledPlatformer.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austen_sYetUntitledPlatformer
{
    internal class Box : Object
    {
        Texture2D texture;
        string texturepath = "boxSprite";
        //these are the size of the texture file not whats on screen
        private int texturewidth = 32;
        private int textureheight = 32;
        private int width;
        private int height;

        private bool colliding = false;
        private bool collidingLeft = false;
        private bool collidingRight = false;
        private List<BoundingRectangle> collides = new List<BoundingRectangle>();

        private bool grounded = false;


        public static float Scale
        {
            get;
            private set;
        } = (float)0.9;

        const float maxVerticalVelocity = 800;
        const float maxHorizontalVelocity = 370;
        const float accelerationDueToGravity = 1000;
        const float movementSpeed = 8;

        //object specific things
        

        public Box(Vector2 position)
        {
            Position = position;
            StartingPosition = position;
            width = (int)(texturewidth * Scale);
            height = (int)(textureheight * Scale);
            CollisionBox = new BoundingRectangle(Position.X, Position.Y, height, width);
            CollisionBox.IsObject = true;
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            moveable = true;
            OtherBoxes = new List<BoundingRectangle>();
        }

        public override void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>(texturepath);
        }

        public override void Update(GameTime gameTime, List<BoundingRectangle> levelCollision, List<Enemy> enemies, BoundingRectangle playerCollisionBox, List<Object> objects)
        {
            //this is the section that every physics object needs to function. Handles collision and gravity and such
            #region Physics

            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //how about physics heh
            Acceleration.Y = 0;
            if (Velocity.Y < maxVerticalVelocity) Acceleration.Y = accelerationDueToGravity;
            Velocity += Acceleration * t;
            //real quick lets check and make sure that velocity isnt higher than it should be.
            if (Math.Abs(Velocity.Y) > maxVerticalVelocity)
            {
                if (Velocity.Y > 0)
                    Velocity.Y = maxVerticalVelocity;
                else if (Velocity.Y < 0)
                    Velocity.Y = -maxVerticalVelocity;
            }
            if (Math.Abs(Velocity.X) > maxHorizontalVelocity)
            {
                if (Velocity.X > 0)
                    Velocity.X = maxHorizontalVelocity;
                else if (Velocity.X < 0)
                    Velocity.X = -maxHorizontalVelocity;
            }
            //heres some physics for falling off the map
            float tempor = Position.X;
            Position += Velocity * t;
            //this one teleports you from bottom to top if you fall off
            if (Position.Y > 832)
                Position.Y = -50;
            //this one disallows you from walking on top of the map when you get teleported
            if (Position.Y < -33 || Position.Y > 736)
                Position.X = tempor;

            CollisionBox.X = Position.X;
            CollisionBox.Y = Position.Y;
            collidingLeft = false;
            collidingRight = false;
            colliding = false;
            collides = new List<BoundingRectangle>();
            foreach (BoundingRectangle b in levelCollision)
            {
                if (CollisionBox.CollidesWith(b))
                {
                    colliding = true;
                    collides.Add(b);
                }
            }
            foreach (Enemy e in enemies)
            {
                if (CollisionBox.CollidesWith(e.CollisionBox) && CollisionBox.X != e.CollisionBox.X && CollisionBox.Y != e.CollisionBox.Y)
                {
                    colliding = true;
                    collides.Add(e.CollisionBox);
                }
            }
            foreach (Object o in objects)
            {
                if (CollisionBox.CollidesWith(o.CollisionBox) && CollisionBox.X != o.CollisionBox.X && CollisionBox.Y != o.CollisionBox.Y && !o.CollisionBox.IsButton)
                {
                    colliding = true;
                    collides.Add(o.CollisionBox);
                }
                foreach (BoundingRectangle b in o.OtherBoxes)
                {
                    if (CollisionBox.CollidesWith(b) && b.Activated)
                    {
                        colliding = true;
                        collides.Add(b);
                    }
                }
            }
            if (moveable)
            {
                if (CollisionBox.CollidesWith(playerCollisionBox))
                {
                    collides.Add(playerCollisionBox);
                }
            }

            //order them from closest to furthest
            //trust me ordering them helps a lot
            bool ordered = false;
            while (!ordered && collides.Count > 0)
            {
                ordered = true;
                for (int i = 0; i < collides.Count - 1; i++)
                {
                    float distance1 = (float)Math.Abs(Math.Sqrt(Math.Pow((collides[i].X - CollisionBox.X), 2) + Math.Pow((collides[i].Y - CollisionBox.Y), 2)));
                    float distance2 = (float)Math.Abs(Math.Sqrt(Math.Pow((collides[i + 1].X - CollisionBox.X), 2) + Math.Pow((collides[i + 1].Y - CollisionBox.Y), 2)));
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
            grounded = false;
            foreach (BoundingRectangle b in collides)
            {
                if (CollisionBox.CollidesWith(b))
                {
                    float overlapTop = b.Bottom - CollisionBox.Top;
                    float overlapBottom = b.Top - CollisionBox.Bottom;
                    float overlapLeft = b.Right - CollisionBox.Left;
                    float overlapRight = b.Left - CollisionBox.Right;

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
                    {//if bottom is smallest we gotta make sure we are not grounded to do normal collision with floor

                        Position.Y += overlapBottom;
                        Velocity.Y = 0;
                        grounded = true;
                    }
                    else if (Math.Abs(overlapLeft) < Math.Abs(overlapRight) && //if left is smallest
                             Math.Abs(overlapLeft) < Math.Abs(overlapTop) &&
                             Math.Abs(overlapLeft) < Math.Abs(overlapBottom))
                    {
                        Position.X += overlapLeft;
                        if (Velocity.X < 0)
                        {
                            Velocity.X = 0;
                        }
                        collidingLeft = true;
                    }
                    else//if right is the smallest
                    {
                        Position.X += overlapRight;
                        if (Velocity.X > 0)
                        {
                            Velocity.X = 0;
                        }
                        collidingRight = true;
                    }

                    CollisionBox.X = Position.X;
                    CollisionBox.Y = Position.Y;
                }
            }
            #endregion

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle source;
            source = new Rectangle(0, 0, texturewidth, textureheight);

            //finally, draw what the source rectangle has turned out to be
            spriteBatch.Draw(texture, Position, source, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
    }
}
