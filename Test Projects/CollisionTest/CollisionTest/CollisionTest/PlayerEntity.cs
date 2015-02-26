using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CollisionTest
{
    public class PlayerEntity : AnimatedEntity
    {
        private const float MaxSpeed = 4f; // Speed the sprite moves when controlled
        private string _previousAnimation;

        public PlayerEntity(Game game, Vector2 position, Vector2 velocity, Texture2D texture, int boundWidth,
            int boundHeight)
            : base(game, position, velocity, texture, boundWidth, boundHeight)
        {
            SpriteTexture.AddAnimation("WalkUp", 0, 0*41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkUpRight", 0, 1*41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkRight", 0, 2*41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkDownRight", 0, 3*41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkDown", 0, 4 * 41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkDownLeft", 0, 5 * 41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkLeft", 0, 6 * 41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkUpLeft", 0, 7 * 41, 42, 41, 8);
        }

        private string GetAnimation(Vector2 velocity)
        {
            if (velocity == Vector2.Zero)
                return null;
            if (velocity.X > 0.001)
            {
                if (velocity.Y > 0.001) return "WalkDownRight";
                if (velocity.Y < -0.001) return "WalkUpRight";
                return "WalkRight";
            }
            if (velocity.X < -0.001)
            {
                if (velocity.Y > 0.001) return "WalkDownLeft";
                if (velocity.Y < -0.001) return "WalkUpLeft";
                return "WalkLeft";
            }
            return velocity.Y > 0.001 ? "WalkDown" : "WalkUp";

        }

        public override void Update(GameTime gameTime)
        {
            if (_previousAnimation == null)
                _previousAnimation = "WalkRight";
            IsAnimating = false;
            // Reset velocity to zero
            _velocity.X = 0;
            _velocity.Y = 0;

            // Query keyboard keys, ajust velocity accordingly
            var keys = Keyboard.GetState();
            if (!(keys.IsKeyUp(Keys.Up) && keys.IsKeyUp(Keys.Down) && keys.IsKeyUp(Keys.Right) && keys.IsKeyUp(Keys.Left)))
            {
                IsAnimating = true;
                if (keys.IsKeyDown(Keys.Up))
                    _velocity.Y = -1;

                if (keys.IsKeyDown(Keys.Down))
                    _velocity.Y = 1;

                if (keys.IsKeyDown(Keys.Right))
                    _velocity.X = 1;

                if (keys.IsKeyDown(Keys.Left))
                    _velocity.X = -1;
            }

            if (_velocity != Vector2.Zero)
            {
                _velocity = MaxSpeed * Vector2.Normalize(_velocity);
            }

            var ani = GetAnimation(_velocity);
            SpriteTexture.SetAnimation(ani ?? _previousAnimation);
            _previousAnimation = ani ?? _previousAnimation;

            // Add velocity to position to move sprite
            Position = Vector2.Add(Position, Velocity);
        }

    }
}
