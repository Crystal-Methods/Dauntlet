using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public abstract class GameScreen
    {
        protected Dauntlet MainGame;
        protected GraphicsDevice GraphicsDevice { get { return MainGame.Graphics; } }
        
        protected GameScreen(Dauntlet game)
        {
            MainGame = game;
        }

        public abstract void LoadContent();
        public abstract void UnloadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gametime);

    }
}
