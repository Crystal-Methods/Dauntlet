using Dauntlet.GameScreens;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Dauntlet.Entities
{
    public class Zombie : EnemyEntity
    {
        private const float ZombieSpeed = 30f; // Speed of the player; CHANGES DEPENDING ON RADIUS!!
        private const float ZombieRadius = 14f; // Radius of player's bounding circle
        private const float ZombieFloatHeight = 14f; //How far the base of the zombie is from the shadow

        // ---------------------------------

        public Zombie(World world, Vector2 position, Texture2D spriteTexture) : base(world, position, spriteTexture, ZombieSpeed, ZombieRadius)
        {
            OffGroundHeight = ZombieFloatHeight;
            HitPoints = 4;
            SpriteTexture.AddAnimation("Walk", 0, 0, 64, 64, 8, 1/4f, false);
            SpriteTexture.SetAnimation("Walk");
            
        }

        // Get player position and other data like this:
        public void Foo()
        {
            Vector2 playerPosition = Player.SimPosition;
            // Do something...
        }

        public override void Die()
        {
            Dying = true;
            //SoundManager.Play("ZombieDeath");
        }

        public override void InflictDamage(int damage)
        {
            base.InflictDamage(damage);
            //SoundManager.Play("ZombieHurt");
        }

        public override void Update(GameTime gameTime)
        {
            if (Dying)
            {
                DeathTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (DeathTimer > 500)
                {
                    Dying = false;
                    Dead = true;
                    CollisionBody.Dispose();
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteTexture.StepAnimation(gameTime);
            spriteBatch.Draw(Shadow, DisplayPosition, null, Color.White, 0f,
                ShadowOrigin, 0.8f, SpriteEffects.None, LayerDepth - 2 / 10000f);
            if (GameplayScreen.DebugCollision)
                spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                    CenterOrigin(DebugCircleTexture), 2 * Radius / 50f, SpriteEffects.None, LayerDepth - 1 / 10000f);
            spriteBatch.Draw(SpriteTexture.Sheet, SpritePosition(), SpriteTexture.CurrentFrame, Hurt ? new Color(1, 0, 0, 1f) : Color.White, 0f, SpriteOrigin, 1f, SpriteEffects.None, LayerDepth);
        }
    }
}
