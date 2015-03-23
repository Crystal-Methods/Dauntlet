using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.GameScreens
{
    public abstract class GameScreen
    {
        public Dauntlet MainGame;
        protected GraphicsDevice GraphicsDevice { get { return MainGame.Graphics; } }
        public SpriteBatch SpriteBatch { get; set; }
        protected bool isLoaded;
        public bool IsLoaded { get { return isLoaded; } }
        abstract public Screen ScreenType { get; }
        
        protected GameScreen(Dauntlet game)
        {
            MainGame = game;
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public abstract void LoadContent();
        public abstract void UnloadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gametime);

    }
}
