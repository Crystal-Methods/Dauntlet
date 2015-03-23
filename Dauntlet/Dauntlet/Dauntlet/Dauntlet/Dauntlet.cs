using System.Collections.Generic;
using Dauntlet.GameScreens;
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

        private readonly GraphicsDeviceManager _graphics;
        private Screen _currentScreenType;
        private Screen _previousScreenType;
        private GameScreen CurrentScreen { get { return _screens[_currentScreenType]; } }
        private GameScreen PreviousScreen { get { return _screens[_previousScreenType]; } }
        //public SpriteBatch SpriteBatch { get; set; }
        public GraphicsDevice Graphics { get { return _graphics.GraphicsDevice; } }
        public SpriteFont Font { get; set; }
        public InputState Input;

        public Dauntlet()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
            _currentScreenType = Screen.TitleScreen;
            Input = new InputState();
            //SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void LoadContent()
        {
            Font = Content.Load<SpriteFont>("loadingFont");

            SoundManager.LoadContent(Content);
            _screens.Add(Screen.GameplayScreen, new GameplayScreen(this));
            _screens.Add(Screen.TitleScreen, new MainMenuScreen(this));
            _screens.Add(Screen.LoadingScreen, new LoadScreen(this));
            _screens.Add(Screen.PauseScreen, new PauseScreen(this));
            
            // The Loading screen is always loaded
            _screens[Screen.LoadingScreen].LoadContent();
            _screens[Screen.PauseScreen].LoadContent();
            
            CurrentScreen.LoadContent();
            
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
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
            _previousScreenType = _currentScreenType;
            CurrentScreen.UnloadContent();
            if (newSceen != Screen.GameplayScreen)
            {
                _currentScreenType = newSceen;
                CurrentScreen.LoadContent();
            }
            else
            {
                _currentScreenType = Screen.LoadingScreen;
                _screens[Screen.GameplayScreen].LoadContent();
            }
        }

        public void ToGameplayScreen()
        {
            _currentScreenType = Screen.GameplayScreen;
        }

        public void OverlayMenu(Screen screenType)
        {
            _previousScreenType = _currentScreenType;
            _currentScreenType = screenType;
        }


    }
}
