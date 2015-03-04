using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private World _world;
        
        private Vector2 _screenCenter;

        private PlayerEntity _player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _world = new World(Vector2.Zero);
        }

        protected override void LoadContent()
        {
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            TileEngine.LoadContent(Content);

            var playerSprite = Content.Load<Texture2D>("Circle");

            ConvertUnits.SetDisplayUnitToSimUnitRatio(50f);

            _player = new PlayerEntity(_world, _screenCenter, playerSprite);
            
        }

        protected override void Update(GameTime gameTime)
        {
            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            _player.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            TileEngine.DrawRoom(_spriteBatch, gameTime);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);
            _player.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
