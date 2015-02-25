using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        public static Texture2D Pixel; // This is for the collision box boundaries
        private readonly SpriteFactory _spriteFactory; // This is responsible for creating sprites
        private List<Entity> entityList = new List<Entity>(); // This is the list of all entities in the game

        public Game1()
        {
            _spriteFactory = new SpriteFactory(this);
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        // Adds an entity to the list
        public void AddEntity(Entity entity)
        {
            entityList.Add(entity);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
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
            PlayerEntity player = _spriteFactory.CreatePlayer(new Vector2(100, 100), new Vector2(0, 0), @"Boo");
            AddEntity(player);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Call each entity's override Update
            foreach (var t in entityList)
                t.Update(gameTime);

            // Perform collision detection on all pairs of entities
            // The double for-loop eliminates duplicate checks
            for (var i = 0; i < entityList.Count; i++)
                for (var j = entityList.Count - 1; j > i; j--)
                {
                    Entity a = entityList[i];
                    Entity b = entityList[j];
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

            _spriteBatch.Begin();

            // Draw each entity by calling its override
            foreach (var t in entityList)
                t.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();
        }
    }
}
