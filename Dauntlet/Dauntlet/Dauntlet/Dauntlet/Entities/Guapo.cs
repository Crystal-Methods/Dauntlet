using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class Guapo : EnemyEntity
    {

        private const float speed = 30f; // Speed of the player; CHANGES DEPENDING ON RADIUS!!
        private const float radius = 14f; // Radius of player's bounding circle
        private const float defaultOffGroundHeight = 15f;

        // ---------------------------------

        public Guapo(World world, Vector2 roomCenter, Texture2D spriteTexture) : base(world, roomCenter, spriteTexture, speed, radius)
        {
            OffGroundHeight = defaultOffGroundHeight;

            SpriteTexture.AddAnimation("Fly", 0, 0, 32, 32, 5, 1/24f, false);
            SpriteTexture.SetAnimation("Fly");
        }
    }
}
