using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest.SpriteSystem
{
    public class EnemyEntity : StaticEntity
    {
        private bool _intersected; // Whether or not this sprite is intersecting the player

        public EnemyEntity(Game game, Vector2 position, Texture2D texture, float boundHeight, float boundWidth)
            : base(game, position,texture, boundHeight, boundWidth)
        { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _intersected = false; // Set this to false every Update cycle
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // If the player is intersecting, draw a red box under the bounding box
            if (_intersected)
            {
                var rect = new Texture2D(Game.GraphicsDevice, (int)Bounds.Width, (int)Bounds.Height);
                var data = new Color[(int)Bounds.Width * (int)Bounds.Height];
                for (int i = 0; i < data.Length; i++) data[i] = Color.Red;
                rect.SetData(data);
                spriteBatch.Draw(rect, _bounds.Position, Color.White);
            }
            base.Draw(spriteBatch, gameTime);
        }

        public override void HandleCollision(Entity collider)
        {
            if (CollidedEntities.Contains(collider))
                return;
            CollidedEntities.Add(collider);
            // If a collision with the player is found, toggle _intersected
            if (collider.GetType() == typeof (PlayerEntity))
                _intersected = true;
        }
    }
}
