using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Dauntlet.GameScreens
{
    public class SplashScreen : GameScreen
    {
        VideoPlayer _videoPlayer;
        Video _video;
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private Vector2 Origin { get { return new Vector2(_video.Width/2f, _video.Height/2f);} }
        private Vector2 ScreenCenter { get { return new Vector2(GraphicsDevice.Viewport.Height/2f, GraphicsDevice.Viewport.Width/2f);} }

        public SplashScreen(Dauntlet game) : base(game)
        {
        }

        public override Screen ScreenType { get { return Screen.SplashScreen; } }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(MainGame.Services, "Content");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _video = _content.Load<Video>("yourvideoname");
            _videoPlayer = new VideoPlayer();
            _videoPlayer.Play(_video);
            IsScreenLoaded = true;
        }

        public override void UnloadContent()
        {
            IsScreenLoaded = false;
            _content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            if (_videoPlayer.State == MediaState.Stopped)
                MainGame.ChangeScreen(Screen.TitleScreen);
        }

        public override void Draw(GameTime gametime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (_videoPlayer.State != MediaState.Stopped)
            {
                Texture2D texture = _videoPlayer.GetTexture();
                if (texture != null)
                {
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(texture, ScreenCenter, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);
                    _spriteBatch.End();
                }
            }
        }
    }
}
