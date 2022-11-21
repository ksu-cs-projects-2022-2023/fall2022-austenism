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
        private bool collidingLeft = false;
        private bool collidingRight = false;
        private List<BoundingRectangle> collides = new List<BoundingRectangle>();
        private Vector2 previousPosition;

        private KeyboardState previousState;
        private KeyboardState currentState;
        private bool grounded = false;



        public static float Scale
        {
            get;
            private set;
        } = (float)0.4;

        //PHYSICS PROPERTIES OF MAN
        public Vector2 Position;
        public BoundingRectangle PlayerCollisionBox;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        const float maxVerticalVelocity = 800;
        const float maxHorizontalVelocity = 400;
        const float accelerationDueToGravity = 1000;
        const float frictionFactor = 16;
        const float movementSpeed = 8;

        public PlayerController(Vector2 position)
        {
            Position = position;
            width = (int)(texturewidth * Scale);
            height = (int)(textureheight * Scale);
            PlayerCollisionBox = new BoundingRectangle(Position.X, Position.Y, height, width);
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            //origin = new Vector2(width / 2, height);
            currentState = Keyboard.GetState();
        }
        public void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("newsprite_filler");
        }
        public void Update(GameTime gameTime, List<Collisions.BoundingRectangle> levelCollision)
        {
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //how about physics heh
            if (Velocity.Y < maxVerticalVelocity) Acceleration.Y = accelerationDueToGravity;

            Velocity += Acceleration * t;

            Position += Velocity * t;
            PlayerCollisionBox.X = Position.X;
            PlayerCollisionBox.Y = Position.Y;
            colliding = false;
            collidingLeft = false;
            collidingRight = false;

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
            grounded = false;
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
                        Velocity.X = 0;
                        collidingLeft = true;
                    }
                    else//if right is the smallest
                    {
                        Position.X += overlapRight;
                        Velocity.X = 0;
                        collidingRight = true;
                    }




                    PlayerCollisionBox.X = Position.X;
                    PlayerCollisionBox.Y = Position.Y;
                }
            }


            //lets take input next
            previousState = currentState;
            currentState = Keyboard.GetState();


            //The big three important ones
            float speedDelta = 0;
            if (currentState.IsKeyDown(Keys.D))
            {//go right
                if (Velocity.X < maxHorizontalVelocity)
                {
                    if(Velocity.X < 0)
                        speedDelta += movementSpeed * 2;
                    else
                        speedDelta += movementSpeed;
                }
            }
            if (currentState.IsKeyDown(Keys.A))
            {//go left
                if (Velocity.X > -maxHorizontalVelocity)
                {
                    if(Velocity.X > 0)
                        speedDelta += -movementSpeed * 2;
                    else
                        speedDelta += -movementSpeed;
                }  
            }

            //if not grounded you dont accelerate as fast
            if (!grounded)
            {
                if (Math.Abs(Velocity.X) < maxHorizontalVelocity / 2)
                    speedDelta *= 0.8f;
                else if ((speedDelta * Velocity.X) < 0)
                    speedDelta *= 0.8f;
                else
                    speedDelta = 0f;
            }
            Velocity.X += speedDelta;

            if (currentState.IsKeyDown(Keys.Space) && !previousState.IsKeyDown(Keys.Space))
            {//JUMPU
                //jump from ground
                if (grounded)
                {
                    float extraHeight = 65 * Math.Abs(Velocity.X) / maxHorizontalVelocity;
                    if (Velocity.X < (-0.1 * maxHorizontalVelocity) || Math.Abs(Velocity.X) > (0.1 * maxHorizontalVelocity))
                        extraHeight += 25;

                    Velocity += new Vector2(0, (-470 - extraHeight));
                }
                //here be wall jumping mechanics
                if(currentState.IsKeyDown(Keys.A) && !grounded && collidingLeft)
                {//wall jump off left wall
                    Velocity.Y = 0;
                    Velocity += new Vector2(400, -400);
                }
                if (currentState.IsKeyDown(Keys.D) && !grounded && collidingRight)
                {//wall jump off right wall
                    Velocity.Y = 0;
                    Velocity += new Vector2(-400, -400);
                }
            }
            //the other two less important ones
            if (currentState.IsKeyDown(Keys.W))
            { //maybe look up?

            }
            if (currentState.IsKeyDown(Keys.S))
            {//squat

            }

            //handle slowdown from friction with floor
            if (!currentState.IsKeyDown(Keys.D) && !currentState.IsKeyDown(Keys.A) && Velocity.X != 0)
            {//slowdown if not pressing either direction
                if (grounded)
                {
                    if (Velocity.X < 0)
                        Velocity.X += frictionFactor;
                    if (Velocity.X > 0)
                        Velocity.X -= frictionFactor;
                    if (Velocity.X < frictionFactor && Velocity.X > -frictionFactor)//its somewhere in the middle so just make it zero
                        Velocity.X = 0;
                }
                else //in the air
                {
                    if (Velocity.X < 0)
                        Velocity.X += frictionFactor/4;
                    if (Velocity.X > 0)
                        Velocity.X -= frictionFactor/4;
                    if (Velocity.X < frictionFactor/4 && Velocity.X > -frictionFactor/4)//its somewhere in the middle so just make it zero
                        Velocity.X = 0;
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
            //else
                spriteBatch.Draw(texture, Position, source, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }


    }
}
