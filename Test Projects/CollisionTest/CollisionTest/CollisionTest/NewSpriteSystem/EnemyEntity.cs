using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest.NewSpriteSystem
{
    internal class EnemyEntity : CircleEntity
    {
        public EnemyEntity(World world, Texture2D spriteSheet, float radius, float density, Vector2 position)
            : base(world, spriteSheet, radius, density, position)
        {
            // Add the various animations the sprite can have
            SpriteTexture.AddAnimation("Stand", 0, 0, spriteSheet.Width, spriteSheet.Height, 1);
        }
    }
}
