using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest.SpriteSystem
{
    /// <summary>
    /// An Entity that has an animated texture, the ability to move, or both.
    /// </summary>
    public class AnimatedEntity : Entity
    {
        // ReSharper disable once InconsistentNaming
        protected AnimatedTexture2D SpriteTexture;
        protected bool IsAnimating; // Whether this sprite should animate its texture next Draw cycle

        public AnimatedEntity(Game game, Vector2 position, Vector2 velocity, Texture2D texture, float boundHeight, float boundWidth, float mass)
            : base(game, position, boundHeight, boundWidth)
        {
            _velocity = velocity;
            _mass = mass;
            SpriteTexture = new AnimatedTexture2D(texture);
        }

        // Gets the position at where to draw the sprite relative to the bounding box
        protected override Vector2 GetSpriteDrawPos()
        {
            return new Vector2((Bounds.Center.X - SpriteTexture.Width / 2f), (Bounds.Center.Y - SpriteTexture.Height + Bounds.Height / 4f));
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (ShowBounds) DrawBorder(spriteBatch); // Only draw bounding box if ShowBounds is true
            SpriteTexture.DrawFrame(spriteBatch, GetSpriteDrawPos());
            if (IsAnimating) SpriteTexture.Frame++;
        }

        public void Move()
        {
            // Add velocity to position to move sprite
            _bounds.Position = Vector2.Add(_bounds.Position, Velocity);
        }
    }
}
