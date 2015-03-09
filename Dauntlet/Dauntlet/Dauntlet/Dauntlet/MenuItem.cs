using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    class MenuItem
    {
        public bool Selected;
        public String Text { get; set; }
        public Vector2 Position { get; set; }

        public MenuItem(string text)
        {
            Text = text;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
        }

    }
}
