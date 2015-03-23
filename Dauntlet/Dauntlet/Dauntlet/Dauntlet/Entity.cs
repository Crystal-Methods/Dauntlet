﻿using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public abstract class Entity
    {
        public float Speed;
        public float Radius;

        protected Vector2 SpriteOrigin;
        protected Texture2D SpriteTexture;
        protected Body CollisionBody;

        public Vector2 SimPosition { get { return CollisionBody.Position; } }
        public Vector2 DisplayPosition { get { return ConvertUnits.ToDisplayUnits(CollisionBody.Position); } }
        public Body GetBody
        {
            get { return CollisionBody; }
            set { CollisionBody = value; }
        }
    }
}
