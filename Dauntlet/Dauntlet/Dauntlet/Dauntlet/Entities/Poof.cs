using System;
using FarseerPhysics;
using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class Poof : Entity
    {
        // As we do not make use of the Body, we must define our own position variable
        private readonly Vector2 _position; // position in display units

        /// <summary>
        /// Creates a new Poof entity
        /// </summary>
        /// <param name="origin">origin of the poof, in display units</param>
        /// <param name="offGroundHeight">the off-ground height of the explosion</param>
        public Poof(Vector2 origin, float offGroundHeight)
        {
            _position = origin;
            OffGroundHeight = offGroundHeight;
            SpriteTexture = new AnimatedTexture2D(SpriteFactory.GetTexture("Splode"));
            SpriteTexture.AddAnimation("Asplode", 0, 0, 32, 32, 6, 1/24f, false, true);
            SpriteTexture.SetAnimation("Asplode");
        }

        /// <summary>
        /// Summons a puff of smoke at the specified position
        /// </summary>
        /// <param name="bodyPosition">the position of the body that summoned the poof</param>
        /// <param name="offGroundHeight">the off-ground height of the body</param>
        public static void SummonPoof(Vector2 bodyPosition, float offGroundHeight)
        {
            var pos = new Vector2(bodyPosition.X, bodyPosition.Y - offGroundHeight);
            TileEngine.TileEngine.CurrentRoom.AddQueue.Add(new Poof(pos, offGroundHeight));
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (SpriteTexture.StepAnimation(gameTime))
                TileEngine.TileEngine.CurrentRoom.RemoveQueue.Add(this);

            float layerDepth = ConvertUnits.ToSimUnits(_position.Y + OffGroundHeight)/100f;
            spriteBatch.Draw(SpriteTexture.Sheet, _position, SpriteTexture.CurrentFrame, Color.White,
                0f, new Vector2(16, 16), 2f, SpriteEffects.None, layerDepth);
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }

    }
}
