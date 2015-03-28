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

             // Create player body
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
            Radius = 0f;
            SpriteTexture = new AnimatedTexture2D(spriteTexture);

            // Create player body
            //CollisionBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), 0.7f, ConvertUnits.ToSimUnits(position));
            CollisionBody = BodyFactory.CreateRectangle(world, bounds.X, bounds.Y, 1f, ConvertUnits.ToSimUnits(position));
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
                        CenterOrigin(DebugCircleTexture), 2*Radius/50f, SpriteEffects.None, LayerDepth - 1/10000f);
            }
            spriteBatch.Draw(SpriteTexture.Sheet, SpritePosition(), SpriteTexture.CurrentFrame, Color.White, 0f,
                SpriteOrigin, 1f, SpriteTexture.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, LayerDepth);
        }
    }
}
