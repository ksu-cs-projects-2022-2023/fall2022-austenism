using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austen_sYetUntitledPlatformer.SpriteHandlers
{
    internal class Sprite
    {
        Texture2D texture;
        string texturepath;

        private Vector2 origin;
        private double animationTimer;
        private short animationFrame = 0;

        public Sprite(string texturePath)
        {
            texturepath = texturePath;
        }
        public void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>(texturepath);
        }
        public void Draw(GameTime gameTime)
        {

        }
    }
}
