using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public class GameplayScreen : GameScreen
    {

        private ContentManager _content;
        public PlayerEntity Player;
        SpriteBatch _spriteBatch;
        private static Matrix _view;

        public static bool Initialized { get; set; }
        public World World { get; set; }
        public static bool DebugCollision { get; set; }
        public Vector2 DisplayRoomCenter { get { return new Vector2(TileEngine.CurrentRoom.PixelWidth/2f, TileEngine.CurrentRoom.PixelHeight/2f);} }
        public Vector2 SimRoomCenter { get { return ConvertUnits.ToSimUnits(DisplayRoomCenter); } }

        // ------------------------------------

        public GameplayScreen(Dauntlet game) : base(game)
        {

            DebugCollision = false;
        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(MainGame.Services, "Content");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize things
            CameraManager.Init(GraphicsDevice);
            TileEngine.LoadContent(this, _content);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(TileEngine.TileSize); // 1 meter = 1 tile

            World = TileEngine.CurrentRoom.World;
            Player = new PlayerEntity(World, DisplayRoomCenter, _content.Load<Texture2D>("Circle"));

            Initialized = true;
        }

        public override void UnloadContent()
        {
            Initialized = false;
            _content.Unload();
        }


        public override void Update(GameTime gameTime)
        {
            // Update the world
            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            Player.Update(gameTime);

            // Update camera
            _view = CameraManager.MoveCamera(Player.DisplayPosition);
        }

        public override void Draw(GameTime gameTime)
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

            
        }
    }
}
