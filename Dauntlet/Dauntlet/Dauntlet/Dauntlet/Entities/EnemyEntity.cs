using Dauntlet.GameScreens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class EnemyEntity : Entity
    {
        protected int HitPoints;
        //private int attack;
        //private int speed;

        protected PlayerEntity Player { get { return GameplayScreen.Player; } }

        public EnemyEntity(World world, Vector2 position, Texture2D spriteTexture, float speed, float radius)
        {
            Speed = speed;
            Radius = radius;
            SpriteTexture = new AnimatedTexture2D(spriteTexture);

            Vector2 circlePosition = ConvertUnits.ToSimUnits(position);

            // Create player body
            CollisionBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), 0.7f, circlePosition);
            CollisionBody.BodyType = BodyType.Dynamic;
            CollisionBody.FixedRotation = true;
            CollisionBody.Restitution = 0.3f;
            CollisionBody.Friction = 0.5f;
            CollisionBody.LinearDamping = 5f;
            CollisionBody.AngularDamping = 100f;
            CollisionBody.CollisionCategories = Category.Cat5;
            CollisionBody.UserData = this;

            //CollisionBody.OnCollision += OnCollision;
        }
        
        public override void Update(GameTime gameTime)
        {

            if (HitPoints <= 0 && !Dying && !Dead)
                Die();
            if (Hurt)
            {
                HurtTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (HurtTimer > 300)
                {
                    Hurt = false;
                    CollisionBody.FixtureList[0].CollisionCategories = Category.Cat5;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteTexture.StepAnimation(gameTime);
            spriteBatch.Draw(Shadow, DisplayPosition, null, Color.White, 0f,
                ShadowOrigin, 0.8f, SpriteEffects.None, LayerDepth - 2/10000f);
            if (GameplayScreen.DebugCollision)
                spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                    CenterOrigin(DebugCircleTexture), 2 * Radius / 50f, SpriteEffects.None, LayerDepth - 1/10000f);
            spriteBatch.Draw(SpriteTexture.Sheet, SpritePosition(), SpriteTexture.CurrentFrame, Hurt ? new Color(1, 0, 0, 1f) : Color.White, 0f, SpriteOrigin, 1f, SpriteEffects.None, LayerDepth);
        }

        public override void Die()
        {
            throw new System.NotImplementedException();
        }

        public virtual void InflictDamage(int damage)
        {
            Hurt = true;
            CollisionBody.FixtureList[0].CollisionCategories = Category.Cat4;
            HurtTimer = 0f;
            HitPoints -= damage;
        }


    }
}
