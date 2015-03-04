using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest.NewSpriteSystem
{
    public interface IEntity
    {
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
