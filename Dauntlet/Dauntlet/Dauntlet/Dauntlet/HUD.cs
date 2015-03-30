using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dauntlet.GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public static class HUD
    {
        private static Texture2D _lifebar;
        private static int Health { get { return GameplayScreen.Player.Health; } }

        public static void Init()
        {
            _lifebar = SpriteFactory.GetTexture("Lifebar");
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_lifebar, new Vector2(10, 10), Color.White);
        }
    }
}
