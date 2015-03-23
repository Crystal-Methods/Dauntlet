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

        protected IList<MenuItem> MenuEntries
        {
            get { return _menuEntries; }
        }

        protected MenuScreen(Dauntlet game, string menuTitle) : base(game)
        {
            _menuTitle = menuTitle;
        }

        public void HandleInput(InputState input)
        {
            // Move to the previous menu entry?
            if (input.IsMenuUp())
            {
                _selectedEntry--;

                if (_selectedEntry < 0)
                    _selectedEntry = _menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.IsMenuDown())
            {
                _selectedEntry++;

                if (_selectedEntry >= _menuEntries.Count)
                    _selectedEntry = 0;
            }

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.

            if (input.IsMenuSelect())
            {
                OnSelectEntry(_selectedEntry);
            }
            else if (input.IsMenuCancel())
            {
                OnCancel(this, EventArgs.Empty);
            }
        }

        protected virtual void OnSelectEntry(int entryIndex)
        {
            _menuEntries[entryIndex].OnSelectEntry();
        }

        protected virtual void OnCancel(object sender, EventArgs eventArgs)
        {
            MainGame.ToGameplayScreen();
        }
        
        protected virtual void UpdateMenuItemLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            //float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            var position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            foreach (MenuItem menuItem in _menuEntries)
            {
                // each entry is to be centered horizontally
                position.X = MainGame.GraphicsDevice.Viewport.Width / 2 - menuItem.GetWidth(this) / 2;

                //if (ScreenState == ScreenState.TransitionOn)
                //    position.X -= transitionOffset * 256;
                //else
                //    position.X += transitionOffset * 512;

                // set the entry's position
                menuItem.Position = position;

                // move down for the next entry the size of this entry
                position.Y += menuItem.GetHeight(this);
            }
        }

        public override void Update(GameTime gameTime)
        {
            //base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            HandleInput(MainGame.Input);
            // Update each nested MenuItem object.
            for (int i = 0; i < _menuEntries.Count; i++)
            {
                bool isSelected = i == _selectedEntry;

                _menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuItemLocations();

            GraphicsDevice graphics = MainGame.GraphicsDevice;
            SpriteBatch spriteBatch = SpriteBatch;
            SpriteFont font = MainGame.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < _menuEntries.Count; i++)
            {
                MenuItem menuItem = _menuEntries[i];
                menuItem.Draw(this, i == _selectedEntry, gameTime);
            }

            // Draw the menu title centered on the screen
            var titlePosition = new Vector2(graphics.Viewport.Width / 2f, 80);
            Vector2 titleOrigin = font.MeasureString(_menuTitle) / 2;
            var titleColor = new Color(192, 192, 192);
            const float titleScale = 1.25f;

            //titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, _menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }

    }
}
