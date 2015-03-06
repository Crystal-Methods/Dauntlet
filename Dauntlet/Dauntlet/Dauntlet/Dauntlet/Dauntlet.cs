using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public enum Screen
    {
        GameplayScreen,
        PauseScreen,
        TitleScreen,
        LoadingScreen,
    }

    public class Dauntlet : Game
    {
        readonly Dictionary<Screen, GameScreen> _screens = new Dictionary<Screen, GameScreen>();

        private Screen _currentScreenType;
        private GameScreen CurrentScreen { get { return _screens[_currentScreenType]; } }
        public SpriteBatch SpriteBatch { get; set; }
        public GraphicsDevice Graphics { get { return GraphicsDevice; } }

        public Dauntlet()
        {
            var graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            _currentScreenType = Screen.TitleScreen;
        }

        protected override void LoadContent()
        {
            SoundManager.LoadContent(Content);
            _screens.Add(Screen.GameplayScreen, new GameplayScreen(this));
            _screens.Add(Screen.TitleScreen, new MainMenuScreen(this));
            //foreach (KeyValuePair<Screen, GameScreen> gs in _screens)
            //    gs.Value.LoadContent();
            CurrentScreen.LoadContent();
            
        }

        protected override void Update(GameTime gameTime)
        {
            CurrentScreen.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            CurrentScreen.Draw(gameTime);
            base.Draw(gameTime);
        }

        public void ChangeScreens(Screen newSceen)
        {
            CurrentScreen.UnloadContent();
            _currentScreenType = newSceen;
            CurrentScreen.LoadContent();
        }


    }
}
