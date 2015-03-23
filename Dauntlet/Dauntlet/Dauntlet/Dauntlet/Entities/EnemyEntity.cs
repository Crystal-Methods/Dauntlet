using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public class EnemyEntity : Entity
    {
        private Vector2 enemyPosition;
        private Texture2D enemySpriteSheet;
        //stats
        private int hitPoints;
        private int attack;
        private int speed;
        //animation
        private Vector2 spriteSheetDimensions;
        private Vector2 spriteSize;
        private int numberOfFrames;
        private int currentFrame;
        private Rectangle sourceRect;
        private int timeBetweenFrames;

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

        //public EnemyEntity(World world, Vector2 enemyPosition, Texture2D enemySpriteSheet, int numberOfFrames)
        //{
        //    currentFrame = 1;
        //    this.numberOfFrames = numberOfFrames;
        //    this.enemyPosition = enemyPosition;
        //    this.enemySpriteSheet = enemySpriteSheet;
        //    spriteSheetDimensions = new Vector2(enemySpriteSheet.Bounds.Width, enemySpriteSheet.Bounds.Height);
        //    spriteSize = new Vector2(32, 32);
        //    sourceRect = new Rectangle(0, 0, (int)spriteSize.X, (int)spriteSize.Y);
        //    timeBetweenFrames = 4;
        //}

        public void Update(GameTime gameTime)
        {
            //if (timeBetweenFrames == 0)
            //{
            //    if (currentFrame == numberOfFrames)
            //    {
            //        currentFrame = 1;
            //        sourceRect.X = 0;
            //    }
            //    else
            //    {
            //        sourceRect.X += 32;
            //        currentFrame++;
            //    }
            //    timeBetweenFrames = 4;
            //}
            //timeBetweenFrames -= 1;
            SpriteTexture.StepAnimation(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(enemySpriteSheet, enemyPosition, sourceRect, Color.White);
            spriteBatch.Draw(Shadow, DisplayPosition, null, Color.White, 0f,
                ShadowOrigin, 1f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(SpriteTexture.Sheet, SpritePosition, SpriteTexture.CurrentFrame, Color.White, 0f, SpriteOrigin, 1f, SpriteEffects.None, 0f);
        }


    }
}
