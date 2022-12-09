using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austen_sYetUntitledPlatformer.GameLoops
{
    public abstract class LevelLoop
    {
        public const int TILE_SIZE = 32;
        public const int GAME_WIDTH = 1280;
        public const int GAME_HEIGHT = 736;


        public ContentManager Content;
        public GraphicsDevice GraphicsDevice;

        //ExitDoor can be an Object, as long as that object has implemented the IDoor Interface
        public IDoor ExitDoor;

        public abstract void Initialize();
        public abstract void LoadContent();
        public abstract int Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);

        public virtual void Reset()
        {
            //do nothing by default
        }
    }
}
