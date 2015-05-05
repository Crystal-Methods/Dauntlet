using System;
using Dauntlet.GameScreens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class Key : Entity
    {

        private const float TopSpeed       =  4f; // Max movement speed
        private const float KeyRadius      =  8f; // Radius of the collision body, in pixels
        private const float KeyFloatHeight = 15f; // Vertical offset between shadow and sprite (for "floating" effect), in pixels
        private const float KeyMass        =  1f; // Mass of the body
        private const int   BaseHealth     =  1;  // Initial health

        // -----------------------------------------

        public static Random Rand = new Random();

        // Reference to the player
        protected PlayerEntity Player { get { return GameplayScreen.Player; } }

        // -----------------------------------------

        /// <summary>
        /// Creates a new EXP Orb entity
        /// </summary>
        /// <param name="world">the Farseer World in which to put this entity</param>
        /// <param name="position">initial position of this entity, in sim units</param>
        public Key(World world, Vector2 position)
        {
            OffGroundHeight = KeyFloatHeight;
            Speed = TopSpeed;
            Radius = KeyRadius.Sim();
            Mass = KeyMass;
            HitPoints = BaseHealth;

            SpriteTexture = new AnimatedTexture2D(SpriteFactory.GetTexture("Key"));
            SpriteTexture.AddAnimation("Float", 0, 0, 64, 64, 12, 1/12f, false, false);
            SpriteTexture.SetAnimation("Float");

            // Create player body
            CollisionBody = BodyFactory.CreateCircle(world, Radius, 0.7f, position);
            CollisionBody.InitBody(BodyType.Dynamic, Category.Cat24, Category.All & ~Category.Cat24, true, 0.3f, 0.5f, 5f, 100f);
            CollisionBody.UserData = this;
        }

        /// <summary>
        /// Creates a new EXP Orb entity
        /// </summary>
        /// <param name="world">the Farseer World in which to put this entity</param>
        /// <param name="position">initial position of this entity, in sim units</param>
        /// <param name="linearVelocity">initial linear velocity of this entity, in sim units</param>
        public Key(World world, Vector2 position, Vector2 linearVelocity) : this(world, position)
        {
            CollisionBody.LinearVelocity = linearVelocity;
        }

        /// <summary>
        /// Spawn a small explosion of EXP orbs at a given position
        /// </summary>
        /// <param name="count">number of orbs to spawn</param>
        /// <param name="origin">origin of the orb explosion, in sim units</param>
        public static void SpawnKey(int count, Vector2 origin)
        {
            World w = TileEngine.TileEngine.CurrentRoom.World;
            const float violence = 3f; // Speed at which it explodes from the origin
            for (int i = 0; i < count; i++)
            {
                var explodeDirection = new Vector2();
                explodeDirection.X += MathHelper.Lerp(-.25f, .25f, (float)Rand.NextDouble());
                explodeDirection.Y += MathHelper.Lerp(-.25f, .25f, (float)Rand.NextDouble());
                explodeDirection.Normalize();
                var ex = new Key(w, origin, explodeDirection * violence);
                TileEngine.TileEngine.CurrentRoom.AddQueue.Add(ex);
            }
        }

        /// <summary>
        /// Draws this EXP Orb to the player, if in range
        /// </summary>
        private void DrawToPlayer()
        {
            float distanceFromPlayer = Vector2.Distance(Player.Position, Position);
            float speed = distanceFromPlayer < 3 ? -(float) Math.Log10(distanceFromPlayer/3)/20f : 0;
            speed = Math.Min(speed, Speed);
            Vector2 direction = Vector2.Normalize(Player.Position - Position);
            CollisionBody.ApplyLinearImpulse(direction * speed);
        }

        public override void Update(GameTime gameTime)
        {
            SpriteTexture.StepAnimation(gameTime);
            DrawToPlayer();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw debug
            if (GameplayScreen.DebugCollision)
                spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                    CenterOrigin(DebugCircleTexture), 2 * DisplayRadius / 50f, SpriteEffects.None, 1f);

            // Draw orb
            spriteBatch.Draw(SpriteTexture.Sheet, new Vector2(DisplayPosition.X, DisplayPosition.Y - OffGroundHeight), SpriteTexture.CurrentFrame,
                Color.White, 0f, new Vector2(16, 16), 0.5f, SpriteEffects.None, LayerDepth);
        }

        public override void Die()
        {
            Dead = true;
            CollisionBody.Dispose();
            TileEngine.TileEngine.CurrentRoom.RemoveQueue.Add(this);
        }

    }
}
