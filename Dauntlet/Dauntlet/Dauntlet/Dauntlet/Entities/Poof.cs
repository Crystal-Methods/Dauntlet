using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dauntlet.GameScreens;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class Poof : Entity
    {
        public Poof(Texture2D texture, Entity e)
        {
            SpriteTexture = new AnimatedTexture2D(texture);
            //CollisionBody = new Body();
        }


        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }
    }
}
