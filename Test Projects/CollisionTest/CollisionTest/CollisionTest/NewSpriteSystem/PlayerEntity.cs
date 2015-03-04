using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CollisionTest.NewSpriteSystem
{
    internal class PlayerEntity : CircleEntity
    {
        public PlayerEntity(World world, Texture2D spriteSheet, float radius, float density, Vector2 position)
            : base(world, spriteSheet, radius, density, position)
        {
            //SpriteBody.BodyType = BodyType.Dynamic;
            SpriteBody.LinearDamping = 0.5f;

            SpriteBody.Restitution = 0.3f;
            SpriteBody.Friction = 0.5f;

            // Add the various animations the sprite can have
            SpriteTexture.AddAnimation("WalkUp", 0, 0 * 41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkUpRight", 0, 1 * 41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkRight", 0, 2 * 41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkDownRight", 0, 3 * 41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkDown", 0, 4 * 41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkDownLeft", 0, 5 * 41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkLeft", 0, 6 * 41, 42, 41, 8);
            SpriteTexture.AddAnimation("WalkUpLeft", 0, 7 * 41, 42, 41, 8);

            SpriteTexture.SetAnimation("WalkRight");
        }

        public Body GetSpriteBody { get { return SpriteBody; } }

        public void HandleKeyboard()
        {
            Vector2 impulse = Vector2.Zero;
            var keys = Keyboard.GetState();
            if (!(keys.IsKeyUp(Keys.Up) && keys.IsKeyUp(Keys.Down) && keys.IsKeyUp(Keys.Right) && keys.IsKeyUp(Keys.Left)))
            {
                if (keys.IsKeyDown(Keys.Up))
                    impulse.Y -= 100;

                if (keys.IsKeyDown(Keys.Down))
                    impulse.Y += 100;

                if (keys.IsKeyDown(Keys.Right))
                    impulse.X += 100;

                if (keys.IsKeyDown(Keys.Left))
                    impulse.X -= 100;
                SpriteBody.ApplyForce(impulse);
            }

            if (keys.IsKeyDown(Keys.A))
                SpriteBody.Position = new Vector2(5, 5);
            else
            {
               // SpriteBody.LinearVelocity = Vector2.Zero;
            }

           
        }
    }
}
