using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.GameScreens
{
    public class MainMenuScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private Texture2D _bgTex_sky;
        private Texture2D _bgTex_clouds_1;
        private Texture2D _bgTex_clouds_2;
        private Texture2D _bgTex_house;
        private Texture2D _bgTex_title;
        private Texture2D _bgTex_options;
        private Texture2D _fistTex;
        private Texture2D _black;

        private int _menuSelection;
        private Vector2 cloudPos1;
        private Vector2 cloudPos2;

        private Cue _menuTheme;

        // ==============================================

        public override Screen ScreenType { get { return Screen.TitleScreen;} }

        // ==============================================

        public MainMenuScreen(Dauntlet game) : base(game) { }

        public override void LoadContent()
        {
            if (_content == null) _content = new ContentManager(MainGame.Services, "Content");
            if (_spriteBatch == null) _spriteBatch = new SpriteBatch(GraphicsDevice);

            _bgTex_sky = _content.Load<Texture2D>("Textures/Menu/Menu_Sky");
            _bgTex_clouds_1 = _content.Load<Texture2D>("Textures/Menu/Menu_Clouds");
            _bgTex_clouds_2 = _content.Load<Texture2D>("Textures/Menu/Menu_Clouds");
            _bgTex_house = _content.Load<Texture2D>("Textures/Menu/Menu_House");
            _bgTex_title = _content.Load<Texture2D>("Textures/Menu/Menu_Title");
            _bgTex_options = _content.Load<Texture2D>("Textures/Menu/Menu_Options");

            _fistTex = _content.Load<Texture2D>("Textures/Fist");

            int width = MainGame.Graphics.Viewport.Width;
            int height = MainGame.Graphics.Viewport.Height;
            _black = new Texture2D(MainGame.Graphics, width, height);
            var data = new Color[width * height];
            for (int i = 0; i < data.Length; i++) data[i] = Color.Black;
            _black.SetData(data);

            _menuSelection = 0;

            cloudPos1 = new Vector2(0, 0);
            cloudPos2 = new Vector2(1366, 0);


            //SoundManager.PlaySong("MainTheme");
            //SoundManager.VolumeChange(0.0f);
            //songVolume = 0.0f;

            _menuTheme = Dauntlet.SoundBank.GetCue("DauntletMainTheme");
            _menuTheme.Play();
            IsScreenLoaded = true;
        }

        public override void UnloadContent()
        {
            IsScreenLoaded = false;
            _content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            if (MainGame.Input.IsMenuSelect())
            {
                if(_menuSelection == 0)
                {
                    MainGame.ChangeScreen(Screen.GameplayScreen);
                    _menuTheme.Stop(AudioStopOptions.Immediate);
                    //SoundManager.PlaySong("NoCombat");
                }
                else if(_menuSelection == 1)
                {

                }
                else if(_menuSelection == 2)
                {
                    MainGame.Exit();
                }
                //Additional options to be added here.
            }
            if (MainGame.Input.IsQuitGame())
                MainGame.Exit();
            if(MainGame.Input.IsMenuDown())
            {
                if(_menuSelection < 2)
                {
                    _menuSelection++;
                    //SoundManager.Play("MenuBlip");
                    Dauntlet.SoundBank.PlayCue("MenuBlip");
                }
            }
            if (MainGame.Input.IsMenuUp())
            {
                if (_menuSelection > 0)
                {
                    _menuSelection--;
                    //SoundManager.Play("MenuBlip");
                    Dauntlet.SoundBank.PlayCue("MenuBlip");
                }
            }

            cloudPos1.X -= 0.20f;
            cloudPos2.X -= 0.20f;
            if(cloudPos1.X == -1366)
            {
                cloudPos1.X = 1336;
            }
            if (cloudPos2.X == -1366)
            {
                cloudPos2.X = 1366;
            }
        }

        public override void Draw(GameTime gametime)
        {
            float drawScale = (float) MainGame.Graphics.Viewport.Height/_bgTex_sky.Height;
            var translate = new Vector2((MainGame.Graphics.Viewport.Width - _bgTex_sky.Width*drawScale)/2f, 0);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_black, Vector2.Zero, Color.White);
            _spriteBatch.Draw(_bgTex_sky, translate, null, Color.White, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 1f);
            _spriteBatch.Draw(_bgTex_clouds_1, cloudPos1, null, Color.White, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 1f);
            _spriteBatch.Draw(_bgTex_clouds_2, cloudPos2, null, Color.White, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 1f);
            _spriteBatch.Draw(_bgTex_house, translate, null, Color.White, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 1f);
            _spriteBatch.Draw(_bgTex_title, translate, null, Color.White, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 1f);
            _spriteBatch.Draw(_bgTex_options, translate, null, Color.White, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 1f);


            // Pulsate the size of the selected menu entry.
            double time = gametime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 12) + 1;

            float scale = 1 + pulsate * 0.15f;

            // Draw fist.
            var origin = new Vector2(_fistTex.Width / 2f, _fistTex.Height / 2f);
            var position = new Vector2(450, 560) * drawScale;
            if (_menuSelection == 0)
            {
                position = new Vector2(450, 560) * drawScale;
            }
            else if(_menuSelection == 1)
            {
                position = new Vector2(450, 625) * drawScale;
            }
            else if(_menuSelection == 2)
            {
                position = new Vector2(450, 690) * drawScale;
            }

            _spriteBatch.Draw(_fistTex, position + translate, null, Color.White, 0f, origin, scale, SpriteEffects.None, 1f);
            _spriteBatch.End();
        }
    }
}
