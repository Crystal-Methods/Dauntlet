using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest
{
    /// <summary>
    /// An Entity that has an animated texture, the ability to move, or both.
    /// </summary>
    public class AnimatedEntity : Entity
    {
        // ReSharper disable once InconsistentNaming
        protected Vector2 _velocity; // Where the entity is going
        protected AnimatedTexture2D SpriteTexture;
        protected bool IsAnimating; // Whether this sprite should animate its texture next Draw cycle

        // Property version of _velocity
        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public AnimatedEntity(Game game, Vector2 position, Vector2 velocity, Texture2D texture,
            int boundWidth,
            int boundHeight)
            : base(game, position, boundWidth, boundHeight)
        {
            _velocity = velocity;
            SpriteTexture = new AnimatedTexture2D(texture);
        }

        // Gets the position at where to draw the sprite relative to the bounding box
        protected override Vector2 GetSpriteDrawPos()
        {
            return new Vector2((Center.X - SpriteTexture.Width / 2f), (Center.Y - SpriteTexture.Height + Bounds.Height / 4f));
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (ShowBounds) DrawBorder(spriteBatch); // Only draw bounding box if ShowBounds is true
            SpriteTexture.DrawFrame(spriteBatch, GetSpriteDrawPos());
            if (IsAnimating) SpriteTexture.Frame++;
        }
    }
}
