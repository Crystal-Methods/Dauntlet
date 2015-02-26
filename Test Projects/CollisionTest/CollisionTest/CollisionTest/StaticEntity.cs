using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest
{
    public class StaticEntity : Entity
    {
        public Texture2D SpriteTexture { get; set; } // Texture for the entity

        public StaticEntity(Game game, Vector2 position, Texture2D texture, int boundWidth,
            int boundHeight)
            : base(game, position, boundWidth, boundHeight)
        {
            SpriteTexture = texture;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (ShowBounds) DrawBorder(spriteBatch); // Only draw bounding box if ShowBounds is true

            spriteBatch.Draw(SpriteTexture, GetSpriteDrawPos(), Color.White);
        }

        protected override Vector2 GetSpriteDrawPos()
        {
            return new Vector2((Center.X - SpriteTexture.Width / 2f), (Center.Y - SpriteTexture.Height + Bounds.Height / 4f));
        }
    }
}
