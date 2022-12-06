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
        string texturepath = "PlayerSpriteSheet";
        //these are the size of the texture file not whats on screen
        private int texturewidth = 64;
        private int textureheight = 96;
        private int width;
        private int height;

        private double animationTimer = 0;
        private short animationFrame = 0;
        private short animationFramerate = 4;
        private bool facingRight = true;
        private bool wallSliding = false;

        private bool colliding = false;
        private bool collidingLeft = false;
        private bool collidingRight = false;
        private List<BoundingRectangle> collides = new List<BoundingRectangle>();
        private Vector2 previousPosition;

        private KeyboardState previousState;
        private KeyboardState currentState;
        
        private bool grounded = false;
        float jumpTimer = 0;



        public static float Scale
        {
            get;
            private set;
        } = (float)0.5;

        //PHYSICS PROPERTIES OF MAN
        public Vector2 Position;
        public BoundingRectangle PlayerCollisionBox;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        const float maxVerticalVelocity = 800;
        const float maxHorizontalVelocity = 300;
        const float accelerationDueToGravity = 1000;
        const float frictionFactor = 16;
        const float movementSpeed = 8;
        const float maxJumpTime = 0.3f;

        public PlayerController(Vector2 position)
        {
            Position = position;
            width = (int)(texturewidth * Scale);
            height = (int)(textureheight * Scale);
            PlayerCollisionBox = new BoundingRectangle(Position.X, Position.Y, height, width, false);
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            //origin = new Vector2(width / 2, height);
            currentState = Keyboard.GetState();
        }
        public void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>(texturepath);
        }
        public bool Update(GameTime gameTime, List<Collisions.BoundingRectangle> levelCollision, List<Enemy> enemies)
        {
            //before anything lets reset all the sprite checker deals so we know what sprite to use when we get to Draw()
            wallSliding = false;

            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //how about physics heh
            Acceleration.Y = 0;
            if (Velocity.Y < maxVerticalVelocity) Acceleration.Y = accelerationDueToGravity;

            Velocity += Acceleration * t;
            //real quick lets check and make sure that velocity isnt higher than it should be.
            if(Math.Abs(Velocity.Y) > maxVerticalVelocity)
            {
                if (Velocity.Y > 0)
                    Velocity.Y = maxVerticalVelocity;
                else if (Velocity.Y < 0)
                    Velocity.Y = -maxVerticalVelocity;
            }
            if(Math.Abs(Velocity.X) > maxHorizontalVelocity)
            {
                if (Velocity.X > 0)
                    Velocity.X = maxHorizontalVelocity;
                else if (Velocity.X < 0)
                    Velocity.X = -maxHorizontalVelocity;
            }
            float tempor = Position.X;
            Position += Velocity * t;
            //this one teleports you from bottom to top if you fall off
            if (Position.Y > 832)
                Position.Y = -50;
            //this one disallows you from walking on top of the map when you get teleported
            if (Position.Y < -33 || Position.Y > 736)
                Position.X = tempor;
            PlayerCollisionBox.X = Position.X;
            PlayerCollisionBox.Y = Position.Y;
            colliding = false;
            collidingLeft = false;
            collidingRight = false;

            //lets check for collision with enemies real quick
            foreach(Enemy e in enemies)
            {
                if (PlayerCollisionBox.CollidesWith(e.CollisionBox))
                    return false;
            }

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
            //trust me ordering them helps a lot
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
                        if (Velocity.X < 0)
                        {
                            Velocity.X = 0;
                        }
                        collidingLeft = true;
                    }
                    else//if right is the smallest
                    {
                        Position.X += overlapRight;
                        if(Velocity.X > 0)
                        {
                            Velocity.X = 0;
                        }
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
                facingRight = true;
                if (Velocity.X < maxHorizontalVelocity)
                {
                    if(Velocity.X < 0)
                        speedDelta += movementSpeed * 2;
                    else
                        speedDelta += movementSpeed;
                }
                if (collidingRight)
                {//if holding d while hugging a wall, slide down the wall
                    if (Velocity.Y > maxVerticalVelocity / 4)
                        Velocity.Y = maxVerticalVelocity / 4;
                    wallSliding = true;
                }
            }
            if (currentState.IsKeyDown(Keys.A))
            {//go left
                facingRight = false;
                if (Velocity.X > -maxHorizontalVelocity)
                {
                    if(Velocity.X > 0)
                        speedDelta += -movementSpeed * 2;
                    else
                        speedDelta += -movementSpeed;
                }
                if (collidingLeft)
                {//if holding a while hugging a wall, slide down the wall
                    if (Velocity.Y > maxVerticalVelocity / 4)
                        Velocity.Y = maxVerticalVelocity / 4;
                    wallSliding = true;
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
                    jumpTimer = 0;
                    float extraHeight = 65 * Math.Abs(Velocity.X) / maxHorizontalVelocity;
                    if (Velocity.X < (-0.1 * maxHorizontalVelocity) || Math.Abs(Velocity.X) > (0.1 * maxHorizontalVelocity))
                        extraHeight += 25;

                    Velocity += new Vector2(0, (-150 - extraHeight));
                }
                //here be wall jumping mechanics
                if (currentState.IsKeyDown(Keys.A) && !grounded && collidingLeft)
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
            if (currentState.IsKeyDown(Keys.Space) && previousState.IsKeyDown(Keys.Space) && jumpTimer < maxJumpTime)
            {//if you are holding down space for a longer jump
                jumpTimer += t;
                Velocity.Y -= 5.4f * (1/(3 * jumpTimer));
            }
            if (!currentState.IsKeyDown(Keys.Space) && previousState.IsKeyDown(Keys.Space))
            {//you have let go of the jump button after jumping so your jump has ended no more jump
                jumpTimer = maxJumpTime;
                Velocity.Y += 30;
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
            return true;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //update animation timer
            //animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            //lets get the animation source rectangle
            

            //if (colliding) //THIS IS FOR DEBUGGING COLLISION
            //{
            //    spriteBatch.Draw(texture, Position, source, Color.Red, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            //    foreach (BoundingRectangle b in collides)
            //        spriteBatch.Draw(texture, new Vector2(b.X, b.Y), source, Color.Red, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            //}
            //else
            SpriteEffects flipped = SpriteEffects.None;
            if (!facingRight)
            {
                flipped = SpriteEffects.FlipHorizontally;
            }


            //for the texture lets default to the base standing, but this can be changed by the state of the player
            Rectangle source;
            source = new Rectangle(0, 0, texturewidth, textureheight);
            
            //walking/running case
            if(Velocity.X != 0 && grounded)
            {//we'll only do the walking/running if grounded so heres that
                //figure the framerate real quick. it needs to be a linear function of the speed between 4ish and 16
                animationFramerate = 3;
                animationFramerate += (short)(8 * (Math.Abs(Velocity.X) / maxHorizontalVelocity));

                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if(animationTimer > 1 / (double)animationFramerate)
                {
                    animationFrame += 1;
                    if (animationFrame == 4)
                        animationFrame = 0;
                    animationTimer = 0;
                }
                source = new Rectangle(0 + (animationFrame * texturewidth), 0, texturewidth, textureheight);
            }

            if (!grounded && Math.Abs(Velocity.X) > 0)
            {
                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (animationTimer > 1 / (double)animationFramerate)
                {
                    animationFramerate = 8;
                    animationFrame += 1;
                    if (animationFrame == 4)
                        animationFrame = 0;
                    animationTimer = 0;
                }
                source = new Rectangle(0 + (animationFrame * texturewidth), 0, texturewidth, textureheight);
            }

            //wall sliding case
            if (wallSliding && !grounded)//this only has one frame of animation so we can just do that one frame here
                source = new Rectangle(0, 97, texturewidth, textureheight);

            //finally, draw what the source rectangle has turned out to be
            spriteBatch.Draw(texture, Position, source, Color.White, 0, Vector2.Zero, Scale, flipped, 0);
        }


    }
}
