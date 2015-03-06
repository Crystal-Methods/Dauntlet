using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public class Game1 : Game
    {
        public PlayerEntity Player;
        SpriteBatch _spriteBatch;
        private static Matrix _view;

        public World World { get; set; }
        public static bool DebugCollision { get; set; }
        public Vector2 DisplayRoomCenter { get { return new Vector2(TileEngine.CurrentRoom.PixelWidth/2f, TileEngine.CurrentRoom.PixelHeight/2f);} }
        public Vector2 SimRoomCenter { get { return ConvertUnits.ToSimUnits(DisplayRoomCenter); } }

        // ------------------------------------

        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            DebugCollision = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize things
            SoundManager.LoadContent(Content);
            CameraManager.Init(GraphicsDevice);
            TileEngine.LoadContent(this, Content);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(TileEngine.TileSize); // 1 meter = 1 tile
            World = TileEngine.CurrentRoom.World;
            Player = new PlayerEntity(World, DisplayRoomCenter, Content.Load<Texture2D>("Circle"));

            //Plays sound
            SoundManager.PlaySong("MainTheme");
            SoundManager.VolumeChange(.5f);
        }

        protected override void Update(GameTime gameTime)
        {
            // Update the world
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            Player.Update(gameTime);

            // Update camera
            _view = CameraManager.MoveCamera(Player.DisplayPosition);
            
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // First pass: Draw room
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _view);
            TileEngine.DrawRoom(_spriteBatch, gameTime);
            _spriteBatch.End();

            // Second pass: Draw room objects
            // TODO

            // Third pass: Draw debug highlights (optional)
            if (DebugCollision)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _view);
                TileEngine.DrawDebug(_spriteBatch, GraphicsDevice);
                _spriteBatch.End();
            }

            // Fourth pass: Draw entities
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            Player.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
