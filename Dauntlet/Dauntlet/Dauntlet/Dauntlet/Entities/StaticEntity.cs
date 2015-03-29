using Dauntlet.GameScreens;
using FarseerPhysics;
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

        public StaticEntity(World world, Vector2 position, float radius, Texture2D spriteTexture)
        {
            IsCircle = true;
            Speed = 0f;
            Radius = radius;
            SpriteTexture = new AnimatedTexture2D(spriteTexture);

             // Create body
            CollisionBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), 0.7f, ConvertUnits.ToSimUnits(position));
            CollisionBody.BodyType = BodyType.Static;
            CollisionBody.FixedRotation = true;
            CollisionBody.Restitution = 0f;
            CollisionBody.Friction = 0.5f;
        }

        public StaticEntity(World world, Vector2 position, Vector2 bounds, Texture2D spriteTexture)
        {
            IsRectangle = true;
            Speed = 0f;
            Height = bounds.Y;
            Width = bounds.X;
            SpriteTexture = new AnimatedTexture2D(spriteTexture);

            // Create body
            CollisionBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(bounds.X), ConvertUnits.ToSimUnits(bounds.Y),
                1f, ConvertUnits.ToSimUnits(position));
            CollisionBody.BodyType = BodyType.Static;
            CollisionBody.FixedRotation = true;
            CollisionBody.Restitution = 0f;
            CollisionBody.Friction = 0.5f;
        }

        public void SetAnimation(int startPosX, int startPosY, int frameWidth, int frameHeight, int frameCount, float fps, bool flipped)
        {
            SpriteTexture.AddAnimation("Animation", startPosX, startPosY, frameWidth, frameHeight, frameCount, fps, flipped);
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
            if (GameplayScreen.DebugCollision)
            {
                if (IsCircle)
                    spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                        CenterOrigin(DebugCircleTexture), 2*Radius/50f, SpriteEffects.None, LayerDepth + 1/100f);
                else if (IsRectangle)
                {
                    spriteBatch.Draw(SpriteFactory.GetRectangleTexture((int)Height, (int)Width, new Color(1, 0, 0, 0.1f)), DisplayPosition,
                        null, Color.White, 0f,
                        new Vector2(Width/2f, Height/2f), 1f, SpriteEffects.None, LayerDepth + 1/100f);
                }
            }
            spriteBatch.Draw(SpriteTexture.Sheet, SpritePosition(), SpriteTexture.CurrentFrame, Color.White, 0f,
                SpriteOrigin, 1f, SpriteTexture.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, LayerDepth);
        }
    }
}
