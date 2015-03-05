using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        public World World { get; set; }
        public static bool DebugCollision { get; set; }
        public Vector2 DisplayRoomCenter { get { return new Vector2(TileEngine.CurrentRoom.PixelWidth/2f, TileEngine.CurrentRoom.PixelHeight/2f);} }
        public Vector2 SimRoomCenter { get { return ConvertUnits.ToSimUnits(DisplayRoomCenter); } }

        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;

        public PlayerEntity Player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            DebugCollision = false;
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
        }

        protected override void LoadContent()
        {
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            TileEngine.LoadContent(this, Content);
            World = TileEngine.CurrentRoom.World;

            var playerSprite = Content.Load<Texture2D>("Circle");

            ConvertUnits.SetDisplayUnitToSimUnitRatio(32f);

            Player = new PlayerEntity(World, DisplayRoomCenter, playerSprite);

        }

        protected override void Update(GameTime gameTime)
        {
            // if (_defaultRoom.Width + 64 < _graphics.GraphicsDevice.Viewport.Width)  // For rooms larger than screen
            _cameraPosition.X = (_graphics.GraphicsDevice.Viewport.Width - TileEngine.CurrentRoom.PixelWidth) / 2f;
            // if (_defaultRoom.Width + 64 < _graphics.GraphicsDevice.Viewport.Width)  // For rooms larger than screen
            _cameraPosition.Y = (_graphics.GraphicsDevice.Viewport.Height - TileEngine.CurrentRoom.PixelHeight) / 2f;
            _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));


            //We update the world
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            Player.Update(gameTime);
            //float a = ConvertUnits.ToSimUnits(TileEngine.TileSize);

            //List<Vector2> posList = (from b in _world.BodyList where b.IsStatic select ConvertUnits.ToDisplayUnits(b.Position)).ToList();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _view);
            TileEngine.DrawRoom(_spriteBatch, gameTime);
            _spriteBatch.End();

            if (DebugCollision)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _view);
                var rect = new Texture2D(_graphics.GraphicsDevice, 32, 32);
                var data = new Color[32*32];
                for (int i = 0; i < data.Length; i++) data[i] = new Color(1, 0, 0, 0.1f);
                rect.SetData(data);
                foreach (var body in World.BodyList)
                {
                    if (body.IsStatic)
                        _spriteBatch.Draw(rect,
                            ConvertUnits.ToDisplayUnits(body.Position) -
                            new Vector2(TileEngine.TileSize/2f, TileEngine.TileSize/2f),
                            body.FixtureList[0].CollisionCategories != Category.Cat10 ? Color.Red : Color.Blue);
                }
                _spriteBatch.End();
            }

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            Player.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
