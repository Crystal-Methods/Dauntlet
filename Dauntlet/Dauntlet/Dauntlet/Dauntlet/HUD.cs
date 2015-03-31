using Dauntlet.GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public static class HUD
    {
        private static Texture2D _lifebar;
        private static AnimatedTexture2D _flame;
        private static int Health { get { return GameplayScreen.Player.Health; } }

        public static void Init()
        {
            _lifebar = SpriteFactory.GetTexture("Lifebar");
            _flame = new AnimatedTexture2D(SpriteFactory.GetTexture("DauntletFire"));
            _flame.AddAnimation("Flicker1", 0, 0, 256, 256, 7, 1/10f, false);
            _flame.SetAnimation("Flicker1");
        }

        public static void Update(GameTime gameTime)
        {
            _flame.StepAnimation(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_lifebar, new Vector2(30, 40), Color.White);
            spriteBatch.Draw(_flame.Sheet, new Vector2(83, 151), _flame.CurrentFrame, Color.White, 0f, new Vector2(132, 194), 1f, SpriteEffects.None, 0f);
        }
    }
}
