using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.GameScreens
{
    public abstract class MenuScreen : GameScreen
    {
        readonly List<MenuItem> _menuEntries = new List<MenuItem>();
        int _selectedEntry;
        readonly string _menuTitle;
        protected GameScreen LowerScreen;
        private Texture2D _darkness;
        

        protected IList<MenuItem> MenuEntries
        {
            get { return _menuEntries; }
        }

        protected MenuScreen(Dauntlet game, string menuTitle) : base(game)
        {
            _menuTitle = menuTitle;
        }

        public void OverlayScreen(GameScreen lowerScreen)
        {
            LowerScreen = lowerScreen;
            MainGame.OverlayScreen(Screen.PauseScreen);
        }

        public void HandleInput(InputState input)
        {
            if (input.IsMenuUp())
            {
                _selectedEntry--;

                if (_selectedEntry < 0)
                    _selectedEntry = _menuEntries.Count - 1;
            }

            if (input.IsMenuDown())
            {
                _selectedEntry++;

                if (_selectedEntry >= _menuEntries.Count)
                    _selectedEntry = 0;
            }

            if (input.IsMenuSelect())
                OnSelectEntry(_selectedEntry);

            else if (input.IsMenuCancel())
                OnCancel(this, EventArgs.Empty);
        }

        protected virtual void OnSelectEntry(int entryIndex)
        {
            _menuEntries[entryIndex].OnSelectEntry();
        }

        protected virtual void OnCancel(object sender, EventArgs eventArgs)
        {
            MainGame.OverlayScreen(LowerScreen.ScreenType);
        }
        
        protected virtual void UpdateMenuItemLocations()
        {
            var position = new Vector2(0f, 175f);

            foreach (MenuItem menuItem in _menuEntries)
            {
                position.X = MainGame.GraphicsDevice.Viewport.Width / 2f;
                menuItem.Position = position;
                position.Y += menuItem.GetHeight(this)+10;
            }
        }

        public override void LoadContent()
        {
            IsScreenLoaded = true;
            _darkness = new Texture2D(MainGame.Graphics, MainGame.Graphics.Viewport.Width, MainGame.Graphics.Viewport.Height);
            var data = new Color[MainGame.Graphics.Viewport.Width * MainGame.Graphics.Viewport.Height];
            for (int i = 0; i < data.Length; i++) data[i] = new Color(0, 0, 0, 0.25f);
            _darkness.SetData(data);
        }

        public override void UnloadContent()
        {
            // This screen does not get unloaded
            LowerScreen.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            HandleInput(MainGame.Input);
            
            for (int i = 0; i < _menuEntries.Count; i++)
            {
                bool isSelected = i == _selectedEntry;
                _menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            LowerScreen.Draw(gameTime);
            UpdateMenuItemLocations();

            GraphicsDevice graphics = MainGame.GraphicsDevice;
            SpriteFont font = MainGame.Font;

            SpriteBatch.Begin();
            SpriteBatch.Draw(_darkness, Vector2.Zero, Color.White);

            for (int i = 0; i < _menuEntries.Count; i++)
            {
                MenuItem menuItem = _menuEntries[i];
                menuItem.Draw(this, i == _selectedEntry, gameTime);
            }

            var titlePosition = new Vector2(graphics.Viewport.Width / 2f, 80);
            Vector2 titleOrigin = font.MeasureString(_menuTitle) / 2;
            var titleColor = new Color(192, 192, 192);
            const float titleScale = 1.25f;
            SpriteBatch.DrawString(font, _menuTitle, titlePosition, titleColor, 0,  titleOrigin, titleScale, SpriteEffects.None, 0);

            SpriteBatch.End();
        }

    }
}
