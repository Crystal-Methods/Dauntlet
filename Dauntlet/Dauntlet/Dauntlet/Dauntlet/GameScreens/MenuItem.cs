using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.GameScreens
{
    public class MenuItem
    {
        public String Text { get; set; }
        public Vector2 Position { get; set; }

        // ==================

        public MenuItem(string text) { Text = text; }

        public event EventHandler Selected;
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }

        public void Draw(MenuScreen ms, bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;

            float scale = 1;
            // Pulsate the size of the selected menu entry.
            if (isSelected)
            {
                double time = gameTime.TotalGameTime.TotalSeconds;

                float pulsate = (float) Math.Sin(time*12) + 1;

                scale += pulsate*0.15f;
            }

            // Draw text, centered on the middle of each line.
            Dauntlet mainGame = ms.MainGame;
            SpriteBatch spriteBatch = ms.SpriteBatch;
            SpriteFont font = mainGame.Font;
            
            var origin = font.MeasureString(Text)/2f;

            spriteBatch.DrawString(font, Text, Position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        public void Update(MenuScreen ms, bool isSelected, GameTime gameTime) { }

        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.MainGame.Font.LineSpacing;
        }

        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.MainGame.Font.MeasureString(Text).X;
        }

    }
}
