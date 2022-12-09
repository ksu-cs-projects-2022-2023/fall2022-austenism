using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Austen_sYetUntitledPlatformer.Collisions;

namespace Austen_sYetUntitledPlatformer
{
    public interface IDoor
    {
        public BoundingRectangle DoorCollisionBox { get; set; }
        public bool Open { get; set; }

        public void DrawDoor(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
