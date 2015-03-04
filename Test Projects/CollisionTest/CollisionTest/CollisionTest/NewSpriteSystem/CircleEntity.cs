using System;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest.NewSpriteSystem
{
    class CircleEntity : IEntity
    {
        protected Body SpriteBody;
        internal AnimatedTexture2D SpriteTexture;
        internal float Fps = 1/24f;
        internal float Timer = 0;
        private bool drawBody = true;

        public CircleEntity(World world, Texture2D spriteSheet, float radius, float density, Vector2 position)
        {
            SpriteTexture = new AnimatedTexture2D(spriteSheet);
            SpriteBody = BodyFactory.CreateCircle(world, radius, density, position);
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (drawBody)
                spriteBatch.Draw(SpriteManager.BoundingCircle, ConvertUnits.ToDisplayUnits(SpriteBody.Position), null, Color.White, SpriteBody.Rotation, Vector2.Zero, .02f * SpriteBody.FixtureList[0].Shape.Radius, SpriteEffects.None, 0f);

            spriteBatch.Draw(SpriteTexture.Sheet, ConvertUnits.ToDisplayUnits(SpriteBody.Position), SpriteTexture.CurrentFrame, Color.White, SpriteBody.Rotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (!(Timer > Fps*1000)) return;
            SpriteTexture.Frame++;
            Timer = 0;
        }
    }
}
