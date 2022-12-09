using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austen_sYetUntitledPlatformer.GameLoops
{
    internal class MainMenuLoop : LevelLoop
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        Texture2D playerTexture;
        private int playertexturewidth = 64;
        private int playertextureheight = 96;

        Texture2D startButtonTexture;
        bool HoveringOverStart = false;

        private int animationFrame = 0;
        private double animationTimer = 0.0;

        Texture2D NotGoombaTexture;
        List<Vector2> NotGoombaLocations;

        public MainMenuLoop(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Content = content;
            GraphicsDevice = graphicsDevice;
        }

        public override void Initialize()
        {
            NotGoombaLocations = new List<Vector2>();
            NotGoombaLocations.Add(new Vector2(1050, -128));
            NotGoombaLocations.Add(new Vector2(1050, 21));
            NotGoombaLocations.Add(new Vector2(1050, 170));
            NotGoombaLocations.Add(new Vector2(1050, 319));
            NotGoombaLocations.Add(new Vector2(1050, 468));
            NotGoombaLocations.Add(new Vector2(1050, 617));
        }
        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Content.Load<SpriteFont>("ComicSans");

            playerTexture = Content.Load<Texture2D>("PlayerSpriteSheet");
            startButtonTexture = Content.Load<Texture2D>("StartButton");
            NotGoombaTexture = Content.Load<Texture2D>("NotGoombaSpriteSheet");
        }
        public override int Update(GameTime gameTime)
        {
            //move the NotGoombas
            for(int i = 0; i < NotGoombaLocations.Count; i++)
            {
                NotGoombaLocations[i] = NotGoombaLocations[i] + new Vector2(0, 1);
                if (NotGoombaLocations[i].Y >= 768)
                    NotGoombaLocations[i] = new Vector2(NotGoombaLocations[i].X, -128);
            }

            HoveringOverStart = false;
            MouseState state = Mouse.GetState();

            if(state.X >= 650 && state.X <= 906 && state.Y >= 350 && state.Y <= 478)
            { //if mouse is hovering over start
                HoveringOverStart = true;
            }

            if (HoveringOverStart && state.LeftButton == ButtonState.Pressed)
                return 1;

            return 0;
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Purple);

            Rectangle source;
            source = new Rectangle(0, 0, playertexturewidth, playertextureheight);

            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > 0.25)
            {
                animationFrame += 1;
                if (animationFrame == 4)
                    animationFrame = 0;
                animationTimer = 0;
            }

            source = new Rectangle(0 + (animationFrame * playertexturewidth), 0, playertexturewidth, playertextureheight);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(_spriteFont, "Austen's Untitled Platformer", new Vector2(65, 90), Color.Orange);

            if(!HoveringOverStart)
                _spriteBatch.Draw(startButtonTexture, new Vector2(650, 350), null, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0);
            else
                _spriteBatch.Draw(startButtonTexture, new Vector2(650, 350), null, Color.Gray, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0);
            
            _spriteBatch.DrawString(_spriteFont, "Start", new Vector2(685, 364), Color.Black);

            _spriteBatch.Draw(playerTexture, new Vector2(150, 350), source, Color.White, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0);

            foreach(Vector2 position in NotGoombaLocations)
            {
                _spriteBatch.Draw(NotGoombaTexture, position, new Rectangle(0, 0, 64, 64), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
            }

            _spriteBatch.End();
        }
    }
}
