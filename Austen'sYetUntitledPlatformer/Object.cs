using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Austen_sYetUntitledPlatformer.Collisions;
using Microsoft.Xna.Framework.Graphics;

namespace Austen_sYetUntitledPlatformer
{
    internal abstract class Object
    {
        public BoundingRectangle CollisionBox;
        public List<BoundingRectangle> OtherBoxes;
        public bool moveable { get; set; } = false;
        public Vector2 StartingPosition { get; set; }

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        public abstract void LoadContent(ContentManager Content);
        public abstract void Update(GameTime gameTime, List<Collisions.BoundingRectangle> levelCollision, List<Enemy> enemies, BoundingRectangle playerCollisionBox, List<Object> objects);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public virtual void Push(GameTime gameTime, float XAxis)
        {
            throw new NotImplementedException();
        }
    }
}
