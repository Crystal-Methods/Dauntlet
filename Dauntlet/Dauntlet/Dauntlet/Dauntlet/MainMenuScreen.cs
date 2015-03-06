using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dauntlet
{
    public class MainMenuScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private Texture2D _bgTex;
        private Texture2D _fistTex;


        public MainMenuScreen(Dauntlet game) : base(game)
        {
        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(MainGame.Services, "Content");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _bgTex = _content.Load<Texture2D>("Textures/Dauntlet");
            _fistTex = _content.Load<Texture2D>("Textures/Fist");

            SoundManager.PlaySong("MainTheme");
            SoundManager.VolumeChange(0.5f);
        }

        public override void UnloadContent()
        {
            _content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            HandleKeyboard();
            HandleGamePad();
        }

        private void HandleGamePad()
        {
            GamePadState padState = GamePad.GetState(0, GamePadDeadZone.Circular);

            if (padState.IsButtonDown(Buttons.Start))
            {
                MainGame.ChangeScreens(Screen.GameplayScreen);
                SoundManager.PlaySong("NoCombat");
            }
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Space))
            {
                MainGame.ChangeScreens(Screen.GameplayScreen);
                SoundManager.PlaySong("NoCombat");
            }
        }

        public override void Draw(GameTime gametime)
        {
            float drawScale = (float) MainGame.Graphics.Viewport.Width/_bgTex.Width;

            _spriteBatch.Begin();
            _spriteBatch.Draw(_bgTex, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 1f);

            // Pulsate the size of the selected menu entry.
            double time = gametime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 12) + 1;

            float scale = 1 + pulsate * 0.15f;

            // Draw text, centered on the middle of each line.
            var origin = new Vector2(_fistTex.Width/2f, _fistTex.Height/2f);
            var position = new Vector2(331, 573)*drawScale;

            _spriteBatch.Draw(_fistTex, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 1f);
            _spriteBatch.End();
        }
    }
}
