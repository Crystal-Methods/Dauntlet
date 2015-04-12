using Dauntlet.GameScreens;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class StaticEntity : Entity
    {
        public bool IsCircle;
        public bool IsRectangle;

        /// <summary>
        /// Creates a circular Static entity
        /// </summary>
        /// <param name="world">the Farseer World in which to put this entity</param>
        /// <param name="position">initial position of this entity, in sim units</param>
        /// <param name="radius">radius of the collision body, in sim units</param>
        /// <param name="spriteTexture">this entity's texture</param>
        public StaticEntity(World world, Vector2 position, float radius, AnimatedTexture2D spriteTexture)
        {
            IsCircle = true;
            Speed = 0f;
            Radius = radius;
            Mass = 1;
            SpriteTexture = spriteTexture;

             // Create body
            CollisionBody = BodyFactory.CreateCircle(world, Radius, this.Density(), position);
            CollisionBody.InitBody(BodyType.Static, Category.Cat1, Category.All, true, 0f, 0.5f, 0f, 0f);
        }

        /// <summary>
        /// Creates a rectangular Static entity
        /// </summary>
        /// <param name="world">the Farseer World in which to put this entity</param>
        /// <param name="position">initial position of this entity, in sim units</param>
        /// <param name="bounds">a set of floats that represent the collision body's width (x) and height (y), in sim units</param>
        /// <param name="spriteTexture">this entity's texture</param>
        public StaticEntity(World world, Vector2 position, Vector2 bounds, AnimatedTexture2D spriteTexture)
        {
            IsRectangle = true;
            Speed = 0f;
            Height = bounds.Y;
            Width = bounds.X;
            Mass = 1;
            SpriteTexture = spriteTexture;

            // Create body
            CollisionBody = BodyFactory.CreateRectangle(world, Width, Height, this.Density(), position);
            CollisionBody.InitBody(BodyType.Static, Category.Cat1, Category.All, true, 0f, 0.5f, 0f, 0f);
        }

        public void SetAnimation(int startPosX, int startPosY, int frameWidth, int frameHeight, int frameCount, float fps, bool flipped, bool isOneTime)
        {
            SpriteTexture.AddAnimation("Animation", startPosX, startPosY, frameWidth, frameHeight, frameCount, fps, flipped, isOneTime);
            SpriteTexture.SetAnimation("Animation");
        }

        public override void Die()
        {
            throw new System.NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            SpriteTexture.StepAnimation(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw debug
            if (GameplayScreen.DebugCollision)
            {
                if (IsCircle)
                    spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                        CenterOrigin(DebugCircleTexture), 2*DisplayRadius/50f, SpriteEffects.None, 1f);
                else if (IsRectangle)
                {
                    spriteBatch.Draw(SpriteFactory.GetRectangleTexture((int)DisplayHeight,
                        (int)DisplayWidth, new Color(1, 0, 0, 0.1f)), DisplayPosition, null, Color.White, 0f,
                        new Vector2(DisplayWidth/2f, DisplayHeight/2f), 1f, SpriteEffects.None, 1f);
                }
            }

            // Draw entity
            spriteBatch.Draw(SpriteTexture.Sheet, SpritePosition(), SpriteTexture.CurrentFrame, Color.White, 0f,
                SpriteOrigin, 1f, SpriteTexture.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, LayerDepth);
        }
    }
}
