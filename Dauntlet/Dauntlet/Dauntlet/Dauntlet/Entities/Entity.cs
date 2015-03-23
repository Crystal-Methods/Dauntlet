using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public abstract class Entity
    {
        public float Speed;
        public float Radius;

        protected float OffGroundHeight { get; set; }
        protected Vector2 ShadowOrigin { get { return new Vector2(Shadow.Width / 2f, Shadow.Height / 2f); } }
        protected Vector2 SpriteOrigin { get { return new Vector2(SpriteTexture.Width / 2f, SpriteTexture.Height); } }
        protected AnimatedTexture2D SpriteTexture;
        protected Body CollisionBody;
        protected Vector2 SpritePosition { get { return new Vector2(DisplayPosition.X, DisplayPosition.Y - OffGroundHeight);} }

        public Vector2 SimPosition { get { return CollisionBody.Position; } }
        public Vector2 DisplayPosition { get { return ConvertUnits.ToDisplayUnits(CollisionBody.Position); } }
        public Body GetBody
        {
            get { return CollisionBody; }
            set { CollisionBody = value; }
        }

        public static Texture2D DebugCircleTexture;
        public static Texture2D Shadow;
    }
}
