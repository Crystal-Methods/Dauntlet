using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class Guapo : EnemyEntity
    {
        public Guapo(World world, Vector2 enemyPosition, Texture2D enemySpriteSheet, int numberOfFrames) : base(world, enemyPosition, enemySpriteSheet, numberOfFrames)
        {
        }
    }
}
