using Dauntlet.GameScreens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public abstract class EnemyEntity : Entity
    {
        // Reference to the player
        protected PlayerEntity Player { get { return GameplayScreen.Player; } }

        /// <summary>
        /// Creates a new Enemy entity
        /// </summary>
        /// <param name="world">Farseer world in which to put the Enemy</param>
        /// <param name="position">initial position of the Enemy, in sim units</param>
        /// <param name="spriteTexture">texture for the Enemy</param>
        /// <param name="speed">top speed of the Enemy</param>
        /// <param name="radius">radius of the Enemy's collision body, in sim units</param>
        protected EnemyEntity(World world, Vector2 position, AnimatedTexture2D spriteTexture, float speed, float radius)
        {
            Speed = speed;
            Radius = ConvertUnits.ToSimUnits(radius);
            SpriteTexture = spriteTexture;

            // Create player body
            CollisionBody = BodyFactory.CreateCircle(world, Radius, 0.7f, position);
            CollisionBody.InitBody(BodyType.Dynamic, Category.Cat10, Category.All, true, 0.3f, 0.5f, 5f, 100f);
            CollisionBody.UserData = this;
        }
        
        public override void Update(GameTime gameTime)
        {
            if (HitPoints <= 0 && !Dying && !Dead) Die();

            if (Hurt) {
                HurtTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (HurtTimer > 300) {
                    Hurt = false;
                    CollisionBody.FixtureList[0].CollisionCategories = Category.Cat10;
                }
            }

            UpdateAi(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteTexture.StepAnimation(gameTime);

            // Draw shadow
            spriteBatch.Draw(Shadow, DisplayPosition, null, Color.White, 0f,
                ShadowOrigin, 0.8f, SpriteEffects.None, LayerDepth - 2/10000f);

            // Draw debug
            if (GameplayScreen.DebugCollision)
                spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                    CenterOrigin(DebugCircleTexture), 2 * DisplayRadius / 50f, SpriteEffects.None, LayerDepth - 1/10000f);

            // Draw enemy
            spriteBatch.Draw(SpriteTexture.Sheet, SpritePosition(), SpriteTexture.CurrentFrame,
                Hurt ? new Color(1, 0, 0, 1f) : Color.White, 0f, SpriteOrigin, 1f, SpriteEffects.None, LayerDepth);
        }

        public override void Die()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Updates the entity using its AI
        /// </summary>
        /// <param name="gameTime">The game's GameTime object</param>
        protected abstract void UpdateAi(GameTime gameTime);


    }
}
