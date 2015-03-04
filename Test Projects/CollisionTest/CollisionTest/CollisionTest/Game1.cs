using System;
using System.Collections.Generic;
using System.Linq;
using CollisionTest.NewSpriteSystem;
//using CollisionTest.Physics;
using CollisionTest.TileSystem;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest
{
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        public static Texture2D Pixel; // This is for the collision box boundaries
        //private readonly SpriteFactory _spriteFactory; // This is responsible for creating sprites
        private readonly List<IEntity> _entityList = new List<IEntity>(); // This is the list of all entities in the game
        //private MapRoom _defaultRoom;
        private PlayerEntity _player;
        private SpriteFont _gameFont;

        private World _world;

        // Camera controls?
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;
        private Vector2 SimScreenCenter { get { return ConvertUnits.ToSimUnits(_screenCenter); } }

        public Game1()
        {
            //_spriteFactory = new SpriteFactory(this);
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //TargetElapsedTime = TimeSpan.FromSeconds(1/24.0);
            _world = new World(Vector2.Zero);
        }

         //Adds an entity to the list
        public void AddEntity(IEntity entity)
        {
            _entityList.Add(entity);
        }

        protected override void LoadContent()
        {
            // Initialize camera controls
            _view = Matrix.Identity;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(32f);  // 1 meter = 32 pixels

            TileEngine.LoadContent(Content);
            //GridCheck.Init(TileEngine.Rooms[TileEngine.CurrentRoomName]);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteManager.Init(Content);

            EnemyEntity enemy1 = SpriteManager.CreateEnemy(_world, @"DryBones", new Vector2(10, 8));
            AddEntity(enemy1);

            _player = SpriteManager.CreatePlayer(_world, @"MarioWalk", new Vector2(3, 3));
            AddEntity(_player);

            _gameFont = Content.Load<SpriteFont>("gamefont");

            //Pixel = Content.Load<Texture2D>(@"Textures/pixel");
            //_spriteFactory.LoadContent(Content);

            // Creates three enemies
            
            //EnemyEntity enemy2 = _spriteFactory.CreateEnemy(new Vector2(375, 300), new Vector2(0, 0), @"DryBones");
            //AddEntity(enemy2);
            //EnemyEntity enemy3 = _spriteFactory.CreateEnemy(new Vector2(560, 60), new Vector2(0, 0), @"DryBones");
            //AddEntity(enemy3);

            // Creates the player
            //PlayerEntity playerEntity = _spriteFactory.CreatePlayer(new Vector2(100, 100), new Vector2(0, 0), @"MarioWalk");
            //_player = playerEntity;
            //AddEntity(playerEntity);
        }

        protected override void Update(GameTime gameTime)
        {
            // if (_defaultRoom.Width + 64 < _graphics.GraphicsDevice.Viewport.Width)  // For rooms larger than screen
                _cameraPosition.X = (_graphics.GraphicsDevice.Viewport.Width - TileEngine.CurrentRoom.PixelWidth)/2f;
            // if (_defaultRoom.Width + 64 < _graphics.GraphicsDevice.Viewport.Width)  // For rooms larger than screen
                _cameraPosition.Y = (_graphics.GraphicsDevice.Viewport.Height - TileEngine.CurrentRoom.PixelHeight) / 2f;
            _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));

            _player.HandleKeyboard();

            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);

            // Call each entity's override Update
            //foreach (var t in _entityList)
            //    t.Update(gameTime);

            //GridCheck.UpdateCells(_entityList);
            //foreach (var e1 in _entityList)
            //{
            //    foreach (var e2 in GridCheck.GetNearby(e1).Distinct())
            //    {
            //        if (CollisionManager.DetectCollision(e1, e2))
            //        {
            //            CollisionManager.ResolveCollision(e1, e2);
            //            e1.HandleCollision(e2);
            //            e2.HandleCollision(e1);
            //        }
            //    }
            //}

            //foreach (AnimatedEntity ae in _entityList.OfType<AnimatedEntity>())
            //    ae.Move();

            //for (var i = 0; i < _entityList.Count; i++)
            //    for (var j = _entityList.Count - 1; j > i; j--)
            //    {
            //        Entity a = _entityList[i];
            //        Entity b = _entityList[j];
            //        if (CollisionManager.DetectCollision(a, b))
            //        {
            //            // If a collision is found, call each entity's handler
            //            a.HandleCollision(b);
            //            b.HandleCollision(a);
            //        }
            //    }
        }

        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.CornflowerBlue); // Draw the fugly background

            // Handle room scrolling
            //int roomOffsetX = ((int) _player.Bounds.Center.X/_defaultRoom.Width) * _defaultRoom.Width;
            //int roomOffsetY = ((int)_player.Bounds.Center.Y / _defaultRoom.Height) * _defaultRoom.Height;
            //Matrix translateMatrix = Matrix.CreateTranslation(-roomOffsetX, -roomOffsetY, 0);


            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);
            //Draw the room
            //TileEngine.DrawRoom(_spriteBatch, gameTime);

            // Draw each entity by calling its override
            foreach (var t in _entityList)
                t.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_gameFont, String.Format("Position: {0}, {1}", _player.GetSpriteBody.Position.X, _player.GetSpriteBody.Position.Y), new Vector2(20, 50), Color.Green, 0.0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0.0f);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        // Small tuple class to handle screen scrolling, might replace
        //private class MapRoom
        //{
        //    public int Height { get; set; }
        //    public int Width { get; set; }
        //}
    }
}
