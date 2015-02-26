using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest
{
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        public static Texture2D Pixel; // This is for the collision box boundaries
        private readonly SpriteFactory _spriteFactory; // This is responsible for creating sprites
        private readonly List<Entity> _entityList = new List<Entity>(); // This is the list of all entities in the game
        private MapRoom _defaultRoom;
        private PlayerEntity _player;
        


        public Game1()
        {
            _spriteFactory = new SpriteFactory(this);
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            TargetElapsedTime = TimeSpan.FromSeconds(1/24.0);
        }

        // Adds an entity to the list
        public void AddEntity(Entity entity)
        {
            _entityList.Add(entity);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _defaultRoom = new MapRoom
            {
                Height = _graphics.GraphicsDevice.Viewport.Height,
                Width = _graphics.GraphicsDevice.Viewport.Width
            };
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Pixel = Content.Load<Texture2D>(@"Textures/pixel");
            _spriteFactory.LoadContent(Content);

            // Creates three enemies
            EnemyEntity enemy1 = _spriteFactory.CreateEnemy(new Vector2(300, 250), new Vector2(0, 0), @"DryBones");
            AddEntity(enemy1);
            EnemyEntity enemy2 = _spriteFactory.CreateEnemy(new Vector2(375, 300), new Vector2(0, 0), @"DryBones");
            AddEntity(enemy2);
            EnemyEntity enemy3 = _spriteFactory.CreateEnemy(new Vector2(560, 60), new Vector2(0, 0), @"DryBones");
            AddEntity(enemy3);

            // Creates the player
            PlayerEntity playerEntity = _spriteFactory.CreatePlayer(new Vector2(100, 100), new Vector2(0, 0), @"MarioWalk");
            _player = playerEntity;
            AddEntity(playerEntity);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Call each entity's override Update
            foreach (var t in _entityList)
                t.Update(gameTime);

            // Perform collision detection on all pairs of entities
            // The double for-loop eliminates duplicate checks
            for (var i = 0; i < _entityList.Count; i++)
                for (var j = _entityList.Count - 1; j > i; j--)
                {
                    Entity a = _entityList[i];
                    Entity b = _entityList[j];
                    if (CollisionManager.DetectCollision(a, b))
                    {
                        // If a collision is found, call each entity's handler
                        a.HandleCollision(b);
                        b.HandleCollision(a);
                    }
                }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.CornflowerBlue); // Draw the fugly background

            // Handle room scrolling
            int roomOffsetX = ((int) _player.Center.X/_defaultRoom.Width) * _defaultRoom.Width;
            int roomOffsetY = ((int)_player.Center.Y / _defaultRoom.Height) * _defaultRoom.Height;
            Matrix translateMatrix = Matrix.CreateTranslation(-roomOffsetX, -roomOffsetY, 0);


            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, translateMatrix);

            // Draw each entity by calling its override
            foreach (var t in _entityList)
                t.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();
        }

        // Small tuple class to handle screen scrolling, might replace
        private class MapRoom
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }
    }
}
