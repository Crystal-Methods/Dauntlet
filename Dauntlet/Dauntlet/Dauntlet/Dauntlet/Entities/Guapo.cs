using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Dauntlet.Entities
{
    public class Guapo : EnemyEntity
    {

        private const float GuapoSpeed = 30f; // Speed of the player; CHANGES DEPENDING ON RADIUS!!
        private const float GuapoRadius = 14f; // Radius of player's bounding circle
        private const float GuapoFloatHeight = 15f;

        // ---------------------------------

        public Guapo(World world, Vector2 position, Texture2D spriteTexture) : base(world, position, spriteTexture, GuapoSpeed, GuapoRadius)
        {
            OffGroundHeight = GuapoFloatHeight;
            HitPoints = 3;

            SpriteTexture.AddAnimation("Fly", 0, 0, 32, 32, 5, 1/24f, false);
            SpriteTexture.SetAnimation("Fly");
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
            SoundManager.Play("GuapoDeath");
        }

        public override void InflictDamage(int damage)
        {
            base.InflictDamage(damage);
            SoundManager.Play("GuapoHurt");
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
    }
}
