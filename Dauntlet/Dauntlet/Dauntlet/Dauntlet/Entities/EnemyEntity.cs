using FarseerPhysics.Dynamics;
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

        public EnemyEntity(World world, Vector2 enemyPosition, Texture2D enemySpriteSheet, int numberOfFrames)
        {
            currentFrame = 1;
            this.numberOfFrames = numberOfFrames;
            this.enemyPosition = enemyPosition;
            this.enemySpriteSheet = enemySpriteSheet;
            spriteSheetDimensions = new Vector2(enemySpriteSheet.Bounds.Width, enemySpriteSheet.Bounds.Height);
            spriteSize = new Vector2(32, 32);
            sourceRect = new Rectangle(0, 0, (int)spriteSize.X, (int)spriteSize.Y);
            timeBetweenFrames = 4;
        }

        public void Update(GameTime gameTime)
        {
            if (timeBetweenFrames == 0)
            {
                if (currentFrame == numberOfFrames)
                {
                    currentFrame = 1;
                    sourceRect.X = 0;
                }
                else
                {
                    sourceRect.X += 32;
                    currentFrame++;
                }
                timeBetweenFrames = 4;
            }
            timeBetweenFrames -= 1;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(enemySpriteSheet, enemyPosition, sourceRect, Color.White);
        }


    }
}
