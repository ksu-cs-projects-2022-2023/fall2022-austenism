using Austen_sYetUntitledPlatformer.Collisions;
using Autofac.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austen_sYetUntitledPlatformer
{
    internal class Button : Object
    {
        Texture2D texture;
        string texturepath = "Button";
        //these are the size of the texture file not whats on screen
        private int texturewidth = 32;
        private int textureheight = 20;
        private int width;
        private int height;

        //this is for the floor that disappears when the button is pressed
        Texture2D texture2;
        string texturepath2 = "VanishingFloor";
            //these are the size of the texture file not whats on screen
        private int texturewidth2 = 32;
        private int textureheight2 = 32;

        //whether or not this button is tied a door
        private bool HasDoor = false;
        Texture2D doorTexture;
        string doorTexturePath = "Door";
        //these are the size of the texture file not whats on screen
        private int doortexturewidth = 32;
        private int doortextureheight = 64;
        private int doorwidth;
        private int doorheight;
        public Vector2 DoorPosition;

        public static float Scale
        {
            get;
            private set;
        } = (float)1;

        private bool colliding = false;
        private bool collidingLeft = false;
        private bool collidingRight = false;
        private List<BoundingRectangle> collides = new List<BoundingRectangle>();

        //here are some Button specific things
        public bool pressed = false;
        private double unpressTimer = 0.0;
        private const double unpressWaitTime = 0.3;

        //SOUND EFFECTS*******************
        List<SoundEffect> soundEffects;

        public Button(Vector2 position, List<BoundingRectangle> otherBoxes, bool hasDoor)
        {
            Position = position;
            Position.Y += 0.375f * 32;
            StartingPosition = Position;
            width = (int)(texturewidth * Scale);
            height = (int)(textureheight * Scale);
            CollisionBox = new BoundingRectangle(Position.X, Position.Y, height, width);
            CollisionBox.IsObject = false;
            CollisionBox.IsButton = true;
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;
            moveable = false;
            //lets add all the disappearing floor boxes real quick
            OtherBoxes = otherBoxes;

            HasDoor = hasDoor;

            soundEffects = new List<SoundEffect>();
        }

        public override void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>(texturepath);
            texture2 = Content.Load<Texture2D>(texturepath2);
            doorTexture = Content.Load <Texture2D>(doorTexturePath);

            soundEffects.Add(Content.Load<SoundEffect>("buttonPressed")); //[0]
        }

        public override void Update(GameTime gameTime, List<BoundingRectangle> levelCollision, List<Enemy> enemies, BoundingRectangle playerCollisionBox, List<Object> objects)
        {
            bool wasPressed = pressed;

            unpressTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if(unpressTimer <= 0)
            {
                pressed = false;
                unpressTimer = 0.0;
            }
            if (!pressed)
            {
                foreach (BoundingRectangle b in OtherBoxes)
                {
                    b.Activated = true;
                }
            }

            CollisionBox.X = Position.X;
            CollisionBox.Y = Position.Y;
            collidingLeft = false;
            collidingRight = false;
            colliding = false;
            collides = new List<BoundingRectangle>();

            //lets check for collisions from enemies or the player or other objects
            foreach (Enemy e in enemies)
            {
                if (CollisionBox.CollidesWith(e.CollisionBox))
                {
                    colliding = true;
                    collides.Add(e.CollisionBox);
                }
            }
            foreach (Object o in objects)
            {
                if (CollisionBox.CollidesWith(o.CollisionBox) && CollisionBox.X != o.CollisionBox.X && CollisionBox.Y != o.CollisionBox.Y)
                {
                    colliding = true;
                    collides.Add(o.CollisionBox);
                }
            }
            if (CollisionBox.CollidesWith(playerCollisionBox))
            {
                collides.Add(playerCollisionBox);
            }

            if(collides.Count > 0)
            {//something is pressing the button
                pressed = true;
                unpressTimer = unpressWaitTime;
            }

            //well lets update whatever is tied to the button
            if (pressed)
            {
                foreach(BoundingRectangle b in OtherBoxes)
                {
                    b.Activated = false;
                }
            }

            //lets play the SoundEffect for pressing the button
            if (pressed && !wasPressed)
                soundEffects[0].Play();

            if(!pressed && wasPressed)
                soundEffects[0].Play();
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle source;
            source = new Rectangle(0*texturewidth, 0, texturewidth, textureheight);

            Rectangle source2;
            source2 = new Rectangle(0*texturewidth2, 0, texturewidth2, textureheight2);

            if (pressed)
            {//is pressed so lets use the pressed sprites
                source = new Rectangle(1*texturewidth, 0, texturewidth, textureheight);

                source2 = new Rectangle(1*texturewidth2, 0, texturewidth2, textureheight2);
            }

            //finally, draw what the source rectangle has turned out to be
            spriteBatch.Draw(texture, Position, source, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);

            foreach (BoundingRectangle b in OtherBoxes)
            {
                spriteBatch.Draw(texture2, new Vector2(b.X, b.Y), source2, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            }
        }
        /// <summary>
        /// this is a method that is for when you have a door tied to the button. Draws the door so you can draw the door in different order than the button if you want
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void DrawDoor(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle doorSource;
            doorSource = new Rectangle(0 * doortexturewidth, 0, doortexturewidth, doortextureheight);


            if (pressed)
            {//is pressed so lets use the pressed sprites
                doorSource = new Rectangle(1 * doortexturewidth, 0, doortexturewidth, doortextureheight);
            }

            if (HasDoor)
            {
                spriteBatch.Draw(doorTexture, DoorPosition, doorSource, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
            }
        }
    }
}
