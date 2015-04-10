using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dauntlet.GameScreens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class Poof : Entity
    {
        private readonly Vector2 _position;

        public Poof(Vector2 displayPosition)
        {
            _position = displayPosition;
            SpriteTexture = new AnimatedTexture2D(SpriteFactory.GetTexture("Splode"));
            SpriteTexture.AddAnimation("Asplode", 0, 0, 32, 32, 6, 1/24f, false, true);
            SpriteTexture.SetAnimation("Asplode");
        }


        public override void Update(GameTime gameTime)
        {
            if (SpriteTexture.StepAnimation(gameTime))
                TileEngine.CurrentRoom.RemoveQueue.Add(this);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float layerDepth = ConvertUnits.ToSimUnits(_position.Y)/100f;
            spriteBatch.Draw(SpriteTexture.Sheet, _position, SpriteTexture.CurrentFrame, Color.White,
                0f, new Vector2(16, 16), 1f, SpriteEffects.None, layerDepth);
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }

        public static void SummonPoof(Vector2 displayPosition)
        {
            var p = new Poof(displayPosition);
            TileEngine.CurrentRoom.AddQueue.Add(p);
        }
    }
}
