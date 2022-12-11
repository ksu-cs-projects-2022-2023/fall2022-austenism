using Austen_sYetUntitledPlatformer.GameLoops;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Austen_sYetUntitledPlatformer
{
    public class TutorialLevelLoop : LevelLoop
    {
        TiledMap _tiledMap;
        TiledMapRenderer _tiledMapRenderer;

        private SpriteBatch _spriteBatch;

        private PlayerController player;
        private Vector2 PlayerStartingPosition = new Vector2(2 * TILE_SIZE, 20 * TILE_SIZE);
        private List<Enemy> enemies;
        private List<Object> objects;

        private int[,] screenLayout;
        private List<Collisions.BoundingRectangle> levelCollision;

        private SoundEffect YouDied;
        private SoundEffect BackgroundMusic;
        SoundEffectInstance MusicInstance;

        public TutorialLevelLoop(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Content = content;
            GraphicsDevice = graphicsDevice;

            SetScreenLayout();
        }

        public override void Initialize()
        {
            player = new PlayerController(PlayerStartingPosition);

            enemies = new List<Enemy>();
            //so heres all the enemies in the level and where they will start at
            enemies.Add(new NotGoomba(new Vector2(1152, 670)));

            objects = new List<Object>();

            ExitDoor = new Door(new Vector2(22 * TILE_SIZE, 1 * TILE_SIZE), Content);
            ExitDoor.Open = true;

        }
        public override void LoadContent()
        {
            _tiledMap = Content.Load<TiledMap>("TutorialLevel");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            YouDied = Content.Load<SoundEffect>("hitHurt");
            BackgroundMusic = Content.Load<SoundEffect>("DungeonTheme");
            MusicInstance = BackgroundMusic.CreateInstance();
            MusicInstance.IsLooped = true;


            player.LoadContent(Content);
            foreach (Enemy enemy in enemies)
            {
                enemy.LoadContent(Content);
            }
            foreach (Object o in objects)
            {
                o.LoadContent(Content);
            }
        }
        public override int Update(GameTime gameTime)
        {
            if (MusicInstance.State != SoundState.Playing)
            {
                MusicInstance.Play();
            }

            _tiledMapRenderer.Update(gameTime);

            //update the player, and if it returns false, that means the player hit an enemy and is dead
            if (!player.Update(gameTime, levelCollision, enemies, objects))
            {
                YouDied.Play();
                player.Position = PlayerStartingPosition;
                foreach (Object o in objects)
                {
                    o.Position = o.StartingPosition;
                }
            }
            foreach (Object o in objects)
            {
                o.Update(gameTime, levelCollision, enemies, player.PlayerCollisionBox, objects);
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime, levelCollision, enemies, objects);
            }

            //check to see of the level is complete
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                MusicInstance.Stop();
                return 2;
            }
            else if (player.PressingW && ExitDoor.Open && player.PlayerCollisionBox.CollidesWith(ExitDoor.DoorCollisionBox))
            {
                MusicInstance.Stop();
                return 1;
            }
            else return 0;
        }
        public override void Draw(GameTime gameTime)
        {
            _tiledMapRenderer.Draw();

            _spriteBatch.Begin();
            //this door shall be drawn before all else (other than tilemap)
            ExitDoor.DrawDoor(gameTime, _spriteBatch);

            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(gameTime, _spriteBatch);
            }
            foreach (Object o in objects)
            {
                o.Draw(gameTime, _spriteBatch);
            }


            player.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
        }
        /// <summary>
        /// Set layout array for collision, will also make an array of BoundingBoxes for the current screen
        /// </summary>
        private void SetScreenLayout()
        {
            screenLayout = new int[GAME_HEIGHT / TILE_SIZE, GAME_WIDTH / TILE_SIZE]
            {   { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,1,1,0,1,1,1,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,0,0,0,0,1,1,0,1,1,1,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 } };
            //lets make the array of bounding boxes
            levelCollision = new List<Collisions.BoundingRectangle>();
            for (int i = 0; i < GAME_HEIGHT / TILE_SIZE; i++)
            {
                for (int j = 0; j < GAME_WIDTH / TILE_SIZE; j++)
                {//when theres something more than zero in the array, it needs collision
                    if (screenLayout[i, j] == 1)
                        levelCollision.Add(new Collisions.BoundingRectangle(j * TILE_SIZE, i * TILE_SIZE, TILE_SIZE, TILE_SIZE));
                }
            }
        }
        public override void Reset()
        {
            Initialize();
            LoadContent();
        }
    }
}
