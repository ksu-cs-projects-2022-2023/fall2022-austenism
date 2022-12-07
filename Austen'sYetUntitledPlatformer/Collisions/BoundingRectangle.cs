using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Text;


namespace Austen_sYetUntitledPlatformer.Collisions
{
    public class BoundingRectangle
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public bool IsObject;
        public bool IsButton;
        public bool Activated; //when activated, collision is true, so dont change it unless you want the collision to go away
        public Vector2 Center => new Vector2(X + (Width/2), Y + (Height/2));

        public float Left => X;
        public float Right => X + Width;
        public float Top => Y;
        public float Bottom => Y + Height;

        public BoundingRectangle(float x, float y, float height, float width)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            IsObject = false;
            IsButton = false;
            Activated = true;
        }

        public BoundingRectangle(Vector2 position, float height, float width)
        {
            X = position.X;
            Y = position.Y;
            Height = height;
            Width = width;
            IsObject = false;
            IsButton = false;
            Activated = true;
        }

        public bool CollidesWith(BoundingRectangle other)
        {
            return (CollisionHelper.CollidesX(this, other) && CollisionHelper.CollidesY(this, other));
        }
    }
}
