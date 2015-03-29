//Cystal Methods
//Section 2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimatedSprites
{
    class BouncingSprite : AutomatedSprite
    {
        // Sprite is automated. Direction is same as speed
        public override Vector2 direction
        {
            get { return speed; }
        }

        public BouncingSprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, string collisionCueName)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed,collisionCueName)
        {
        }

        public BouncingSprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            int millisecondsPerFrame, string collisionCueName)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame,collisionCueName)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Move sprite based on direction
            position += direction;


            //Check if it went off screen and reverses the direction if it goes off the screen
            if (position.X < 0)
            {
                position.X = 0;
                speed.X = speed.X * -1;
            }

            if (position.Y < 0)
            {
                position.Y = 0;
                speed.Y = speed.Y * -1;
            }
                
            if (position.X > clientBounds.Width - frameSize.X)
            {
                position.X = clientBounds.Width - frameSize.X;
                speed.X = speed.X * -1;
                //speed.Y = speed.Y * -1;
            }

            if (position.Y > clientBounds.Height - frameSize.Y)
            {
                position.Y = clientBounds.Height - frameSize.Y;
                //speed.X = speed.X * -1;
                speed.Y = speed.Y * -1;
            }
                

            base.Update(gameTime, clientBounds);
        }

    }
}
