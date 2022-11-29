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
    internal abstract class Enemy
    {
        public BoundingRectangle CollisionBox;

        public abstract void LoadContent(ContentManager Content);
        public abstract void Update(GameTime gameTime, List<Collisions.BoundingRectangle> levelCollision);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
