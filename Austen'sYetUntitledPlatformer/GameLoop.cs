using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using SharpDX.XAudio2;
using Austen_sYetUntitledPlatformer.Collisions;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Austen_sYetUntitledPlatformer
{
    public class GameLoop : Game
    {
        const int GAME_WIDTH = 1280;
        const int GAME_HEIGHT = 736;
        const int TILE_SIZE = 32;

        TiledMap _tiledMap;
        TiledMapRenderer _tiledMapRenderer;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private PlayerController player;
        private Vector2 PlayerStartingPosition = new Vector2(2 * TILE_SIZE, 20 * TILE_SIZE);
        private List<Enemy> enemies;

        private int[,] screenLayout;
        private List<Collisions.BoundingRectangle> levelCollision;

        private SoundEffect YouDied;

        public GameLoop()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = GAME_WIDTH;
            _graphics.PreferredBackBufferHeight = GAME_HEIGHT;

            SetScreenLayout();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            player = new PlayerController(PlayerStartingPosition);
            
            enemies = new List<Enemy>();
            //so heres all the enemies in the level and where they will start at
            enemies.Add(new NotGoomba(new Vector2(1152, 670)));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _tiledMap = Content.Load<TiledMap>("DungeonMap");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            YouDied = Content.Load<SoundEffect>("hitHurt");
            

            player.LoadContent(Content);
            foreach(Enemy enemy in enemies)
            {
                enemy.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _tiledMapRenderer.Update(gameTime);

            //update the player, and if it returns false, that means the player hit an enemy and is dead
            if(!player.Update(gameTime, levelCollision, enemies))
            {
                YouDied.Play();
                player.Position = PlayerStartingPosition;
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime, levelCollision, enemies);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);

            Vector2 offset = new Vector2(100, 0);
            offset -= player.Position;
            Matrix transform = Matrix.CreateTranslation(offset.X, offset.Y, 0);

            _tiledMapRenderer.Draw();
            _spriteBatch.Begin();

            player.Draw(gameTime, _spriteBatch);
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();


            base.Draw(gameTime);
        }

        /// <summary>
        /// Set layout array for collision, will also make an array of BoundingBoxes for the current screen
        /// </summary>
        private void SetScreenLayout()
        {
            screenLayout = new int[GAME_HEIGHT / TILE_SIZE, GAME_WIDTH / TILE_SIZE]
            {   { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,1,1,1,1,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,1,1,1,1,0,0,0,0,1},
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1} };
            //lets make the array of bounding boxes
            levelCollision = new List<Collisions.BoundingRectangle>();
            for (int i = 0; i < GAME_HEIGHT / TILE_SIZE; i++)
            {
                for (int j = 0; j < GAME_WIDTH / TILE_SIZE; j++)
                {//when theres something more than zero in the array, it needs collision
                    if (screenLayout[i,j] == 1)
                        levelCollision.Add(new Collisions.BoundingRectangle(j * TILE_SIZE, i * TILE_SIZE, TILE_SIZE, TILE_SIZE, false));
                }
            }
        }
    }
}