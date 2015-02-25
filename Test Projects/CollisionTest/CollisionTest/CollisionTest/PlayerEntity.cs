using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CollisionTest
{
    public class PlayerEntity : Entity
    {
        private const float MaxSpeed = 7f; // Speed the sprite moves when controlled

        public PlayerEntity(Game game, Vector2 position, Vector2 velocity, Texture2D texture, string textureName, int boundWidth,
            int boundHeight)
            : base(game, position, velocity, texture, textureName, boundWidth, boundHeight)
        { }

        public override void Update(GameTime gameTime)
        {
            // Reset velocity to zero
            _velocity.X = 0;
            _velocity.Y = 0;

            // Query keyboard keys, ajust velocity accordingly
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                _velocity.Y = -MaxSpeed;

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                _velocity.Y = MaxSpeed;

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                _velocity.X = MaxSpeed;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                _velocity.X = -MaxSpeed;

            if (_velocity != Vector2.Zero)
            {
                //_velocity.Normalize();
            }

            // Add velocity to position to move sprite
            Position = Vector2.Add(Position, Velocity);
        }

    }
}
