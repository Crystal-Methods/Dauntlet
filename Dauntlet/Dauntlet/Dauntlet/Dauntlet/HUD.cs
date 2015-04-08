using System;
using Dauntlet.GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public static class HUD
    {
        private static Texture2D _lifebar;
        private static AnimatedTexture2D _flame;
        private static AnimatedTexture2D _healthStock;
        private static int Health { get { return GameplayScreen.Player.Health; } }
        private static Vector2 _hsa; // Displacement between health stocks on the healthbar

        public static void Init()
        {
            _lifebar = SpriteFactory.GetTexture("Lifebar");
            _flame = new AnimatedTexture2D(SpriteFactory.GetTexture("DauntletFire"));
            _flame.AddAnimation("Flicker1", 0, 0, 256, 256, 7, 1/10f, false);
            _flame.SetAnimation("Flicker1");
            _healthStock = new AnimatedTexture2D(SpriteFactory.GetTexture("HealthStock"));

            _hsa = new Vector2(-(float) Math.Cos(22.38), (float) Math.Sin(22.38));
            _hsa.Normalize();
            _hsa *= 20;
        }

        public static void Update(GameTime gameTime)
        {
            _flame.StepAnimation(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_lifebar, new Vector2(30, 40), Color.White);
            spriteBatch.Draw(_flame.Sheet, new Vector2(83, 151), _flame.CurrentFrame, Color.White, 0f, new Vector2(132, 194), 1f, SpriteEffects.None, 0f);
            for (int i = 0; i < GameplayScreen.Player.Health; i++)
            {
                spriteBatch.Draw(_healthStock.Sheet, new Vector2(140, 102) + (_hsa * i), _healthStock.CurrentFrame, Color.White, 0f, new Vector2(10, 10), 1f, SpriteEffects.None, 1f);
            }
        }
    }
}
