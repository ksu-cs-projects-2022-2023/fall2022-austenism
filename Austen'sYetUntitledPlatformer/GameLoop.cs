using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using SharpDX.XAudio2;
using Austen_sYetUntitledPlatformer.Collisions;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Text;
using Austen_sYetUntitledPlatformer.GameLoops;

namespace Austen_sYetUntitledPlatformer
{
    public class GameLoop : Game
    {
        const int GAME_WIDTH = 1280;
        const int GAME_HEIGHT = 736;

        private GraphicsDeviceManager _graphics;

        LevelLoop currentLevel;
        short currentLevelIndex;
        MainMenuLoop mainMenu;
        DungeonLevelLoop dungeonLevel;

        List<LevelLoop> levels;

        public GameLoop()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = GAME_WIDTH;
            _graphics.PreferredBackBufferHeight = GAME_HEIGHT;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            mainMenu = new MainMenuLoop(Content, GraphicsDevice);
            mainMenu.Initialize();
            dungeonLevel = new DungeonLevelLoop(Content, GraphicsDevice);
            dungeonLevel.Initialize();

            levels = new List<LevelLoop>();
            levels.Add(mainMenu);
            levels.Add(dungeonLevel);

            currentLevel = mainMenu;
            currentLevelIndex = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            mainMenu.LoadContent();
            dungeonLevel.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            switch (currentLevel.Update(gameTime)) 
            {
                case 0:
                    //do nothing

                    break;
                case 1:
                    //continue to next level
                    if(currentLevelIndex < (levels.Count - 1))
                    {
                        currentLevelIndex += 1;
                        levels[currentLevelIndex].Reset();
                    }
                    else
                    {
                        currentLevelIndex = 0;
                    }
                    break;
                case 2:
                    //exit to menu
                    currentLevelIndex = 0;
                    break;
                default:
                    //something has gone wrong

                    break;
            }

            currentLevel = levels[currentLevelIndex];

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);

            currentLevel.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}