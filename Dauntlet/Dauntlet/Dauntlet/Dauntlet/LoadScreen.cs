using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public class LoadScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        public LoadScreen(Dauntlet game) : base(game)
        {
        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(MainGame.Services, "Content");

            _font = _content.Load<SpriteFont>("loadingFont");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if (GameplayScreen.Initialized)
                MainGame.ToGameplayScreen();
        }

        public override void Draw(GameTime gametime)
        {
            const string message = "Loading...";

            // Center the text in the viewport.
            Viewport viewport = MainGame.Graphics.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = _font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            //Color color = Color.White * TransitionAlpha;

            // Draw the text.
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, message, textPosition, Color.White);
            _spriteBatch.End();
        }
    }
}
