using Austen_sYetUntitledPlatformer.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Austen_sYetUntitledPlatformer
{
    public class Door : IDoor
    {
        Texture2D doorTexture;
        string doorTexturePath = "Door";
        //these are the size of the texture file not whats on screen
        private int doortexturewidth = 32;
        private int doortextureheight = 64;
        public Vector2 DoorPosition;
        public BoundingRectangle DoorCollisionBox { get; set; }
        public bool Open { get; set; }

        public Door(Vector2 doorposition, ContentManager Content)
        {
            DoorPosition = doorposition;
            doorTexture = Content.Load<Texture2D>(doorTexturePath);

            DoorCollisionBox = new BoundingRectangle(doorposition, doortexturewidth, doortextureheight);
        }

        
       

        public void DrawDoor(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle doorSource;
            doorSource = new Rectangle(0 * doortexturewidth, 0, doortexturewidth, doortextureheight);

            if (Open)
            {//is pressed so lets use the pressed sprites
                doorSource = new Rectangle(1 * doortexturewidth, 0, doortexturewidth, doortextureheight);
            }

                spriteBatch.Draw(doorTexture, DoorPosition, doorSource, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);        }
    }
}
