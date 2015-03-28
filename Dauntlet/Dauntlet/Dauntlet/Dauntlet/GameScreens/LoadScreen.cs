using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.GameScreens
{
    public class LoadScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Screen _screenToLoad;
        public override Screen ScreenType { get { return Screen.LoadingScreen; } }

        public LoadScreen(Dauntlet game) : base(game)
        {
        }

        public void BeginLoadingScreen(Screen screenToLoad)
        {
            _screenToLoad = screenToLoad;
            MainGame.GetScreen(_screenToLoad).LoadContent();
        }

        public override void LoadContent()
        {
            IsScreenLoaded = true;
            if (_content == null)
                _content = new ContentManager(MainGame.Services, "Content");

            _font = _content.Load<SpriteFont>("loadingFont");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void UnloadContent()
        {
            // This screen cannot be unloaded
        }

        public override void Update(GameTime gameTime)
        {
            if (MainGame.GetScreen(_screenToLoad).IsLoaded)
                MainGame.ChangeScreen(_screenToLoad);
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
