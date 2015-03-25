using Dauntlet.GameScreens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class EnemyEntity : Entity
    {
        private int hitPoints;
        private int attack;
        private int speed;

        public EnemyEntity(World world, Vector2 roomCenter, Texture2D spriteTexture, float speed, float radius)
        {
            Speed = speed;
            Radius = radius;
            SpriteTexture = new AnimatedTexture2D(spriteTexture);
            
            Vector2 circlePosition = ConvertUnits.ToSimUnits(roomCenter) + new Vector2(0, -1f);

            // Create player body
            CollisionBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), 0.7f, circlePosition);
            CollisionBody.BodyType = BodyType.Dynamic;
            CollisionBody.FixedRotation = true;
            CollisionBody.Restitution = 0.3f;
            CollisionBody.Friction = 0.5f;
            CollisionBody.LinearDamping = 50f;
            CollisionBody.AngularDamping = 100f;

            //CollisionBody.OnCollision += CollisionBodyOnCollision;
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteTexture.StepAnimation(gameTime);
            spriteBatch.Draw(Shadow, DisplayPosition, null, Color.White, 0f,
                ShadowOrigin, 1f, SpriteEffects.None, LayerDepth - 2/10000f);
            if (GameplayScreen.DebugCollision)
                spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                    CenterOrigin(DebugCircleTexture), 2 * Radius / 50f, SpriteEffects.None, LayerDepth - 1/10000f);
            spriteBatch.Draw(SpriteTexture.Sheet, SpritePosition(), SpriteTexture.CurrentFrame, Color.White, 0f, SpriteOrigin, 1f, SpriteEffects.None, LayerDepth);
        }


    }
}
