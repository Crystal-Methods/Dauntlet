using System;
using Dauntlet.GameScreens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class ExpOrb : Entity
    {

        public static Random Rand = new Random();
        protected PlayerEntity Player { get { return GameplayScreen.Player; } }

        public ExpOrb(World world, Vector2 position)
        {
            OffGroundHeight = 15f;
            Speed = 1f;
            Radius = 10f;
            SpriteTexture = new AnimatedTexture2D(SpriteFactory.GetTexture("ExpOrb"));
            SpriteTexture.AddAnimation("Pulsate", 0, 0, 32, 32, 16, 1/12f, false, false);
            SpriteTexture.SetAnimation("Pulsate");

            Vector2 circlePosition = position;

            // Create player body
            CollisionBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), 0.7f, circlePosition);
            CollisionBody.BodyType = BodyType.Dynamic;
            CollisionBody.FixedRotation = true;
            CollisionBody.Restitution = 0.3f;
            CollisionBody.Friction = 0.5f;
            CollisionBody.LinearDamping = 5f;
            CollisionBody.AngularDamping = 100f;
            CollisionBody.CollisionCategories = Category.Cat24;
            foreach (Fixture f in CollisionBody.FixtureList)
                f.CollidesWith = Category.All & ~Category.Cat24;
            CollisionBody.UserData = this;
        }

        public ExpOrb(World world, Vector2 position, Vector2 linearVelocity) : this(world, position)
        {
            CollisionBody.LinearVelocity = linearVelocity;
        }

        public void DrawToPlayer()
        {
            float distanceFromPlayer = Vector2.Distance(Player.SimPosition, SimPosition);
            float speed = distanceFromPlayer < 3 ? -(float) Math.Log10(distanceFromPlayer/3)/20f : 0;
            speed = Math.Min(speed, 4);
            Vector2 direction = Vector2.Normalize(Player.SimPosition - SimPosition);
            CollisionBody.ApplyLinearImpulse(direction * speed);
        }

        public override void Update(GameTime gameTime)
        {
            SpriteTexture.StepAnimation(gameTime);
            DrawToPlayer();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (GameplayScreen.DebugCollision)
                spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                    CenterOrigin(DebugCircleTexture), 2 * Radius / 50f, SpriteEffects.None, LayerDepth - 1 / 10000f);
            spriteBatch.Draw(SpriteTexture.Sheet, new Vector2(DisplayPosition.X, DisplayPosition.Y - OffGroundHeight), SpriteTexture.CurrentFrame,
                Color.White, 0f, new Vector2(16, 16), 0.5f, SpriteEffects.None, LayerDepth);
        }

        public override void Die()
        {
            Dead = true;
            CollisionBody.Dispose();
            TileEngine.CurrentRoom.RemoveQueue.Add(this);
        }

        public static void SpawnExp(int count, Vector2 origin)
        {
            World w = TileEngine.CurrentRoom.World;
            const float violence = 3f; // Speed at which it explodes from the origin
            for (int i = 0; i < count; i++)
            {
                var explodeDirection = new Vector2();
                explodeDirection.X += MathHelper.Lerp(-.25f, .25f, (float)Rand.NextDouble());
                explodeDirection.Y += MathHelper.Lerp(-.25f, .25f, (float)Rand.NextDouble());
                explodeDirection.Normalize();
                var ex = new ExpOrb(w, origin, explodeDirection * violence);
                TileEngine.CurrentRoom.AddQueue.Add(ex);
            }
        }
    }
}
