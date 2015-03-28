using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class Guapo : EnemyEntity
    {

        private const float GuapoSpeed = 30f; // Speed of the player; CHANGES DEPENDING ON RADIUS!!
        private const float GuapoRadius = 14f; // Radius of player's bounding circle
        private const float GuapoFloatHeight = 15f;

        // ---------------------------------

        public Guapo(World world, Vector2 roomCenter, Texture2D spriteTexture) : base(world, roomCenter, spriteTexture, GuapoSpeed, GuapoRadius)
        {
            OffGroundHeight = GuapoFloatHeight;

            SpriteTexture.AddAnimation("Fly", 0, 0, 32, 32, 5, 1/24f, false);
            SpriteTexture.SetAnimation("Fly");
        }

        // Get player position and other data like this:
        public void Foo()
        {
            Vector2 playerPosition = Player.SimPosition;
            // Do something...
        }
    }
}
