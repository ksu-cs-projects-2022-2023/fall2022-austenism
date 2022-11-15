using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austen_sYetUntitledPlatformer.Collisions
{
    public static class CollisionHelper
    {
        /// <summary>
        /// detects collision between two BoundingRectangles
        /// </summary>
        /// <param name="a">the first rectangle</param>
        /// <param name="b">the second rectangle</param>
        /// <returns></returns>
        public static bool CollidesX(BoundingRectangle a, BoundingRectangle b)
        {
            if (a.Right > b.Left && a.Left < b.Right) return true;
            else return false;
        }

        public static bool CollidesY(BoundingRectangle a, BoundingRectangle b)
        {
            if (a.Bottom > b.Top && a.Top < b.Bottom) return true;
            else return false;
        }
    }
}
