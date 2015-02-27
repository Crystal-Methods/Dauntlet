using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest
{
    /// <summary>
    /// Handles an animated texture from a spritesheet
    /// </summary>
    public class AnimatedTexture2D
    {

        private readonly Texture2D _sheet; // the spritesheet
        private readonly Dictionary<string, AnimationCycle> _animations = new Dictionary<string, AnimationCycle>(); // List of animation cycles on the sheet
        private AnimationCycle _currentAnimation; // Current animation playing
        private string _currentAnimationName; // Name of current animation playing
        private float frameInterval;

        // Height of the current animation frame
        public float Height
        {
            get { return _currentAnimation.FrameHeight; }
        }

        // Width of the current animation frame
        public float Width
        {
            get { return _currentAnimation.FrameWidth; }
        }

        // The number of the current frame
        public int Frame
        {
            get { return _currentAnimation.CurrentFrame; }
            set { _currentAnimation.CurrentFrame = value % _currentAnimation.FrameCount; }
        }

        public AnimatedTexture2D(Texture2D sheet)
        {
            _sheet = sheet;
            frameInterval = ((float)1/24)*1000; //default 24 FPS
        }

        // Add an animation cycle
        public void AddAnimation(string name, int startPosX, int startPosY, int frameWidth, int frameHeight, int frameCount)
        {
            var newAni = new AnimationCycle
            {
                StartPosX = startPosX,
                StartPosY = startPosY,
                FrameWidth = frameWidth,
                FrameHeight = frameHeight,
                FrameCount = frameCount,
                CurrentFrame = 0
            };
            _animations.Add(name, newAni);
        }

        // Set the current animation to play
        public void SetAnimation(string name)
        {
            if (name.Equals(_currentAnimationName)) return;
            _currentAnimationName = name;
            _currentAnimation = _animations[name];
            _currentAnimation.CurrentFrame = 0;
        }

        // Draw the correct frame of the animation
        private const float Rotation = 0f;
        private readonly Vector2 _origin = new Vector2(0, 0);
        private const float Scale = 1f;
        private const float Depth = 0f;
        public void DrawFrame(SpriteBatch batch, Vector2 position)
        {
            var sourcerect =
                new Rectangle(_currentAnimation.StartPosX + (_currentAnimation.FrameWidth * _currentAnimation.CurrentFrame),
                    _currentAnimation.StartPosY, _currentAnimation.FrameWidth, _currentAnimation.FrameHeight);
            batch.Draw(_sheet, position, sourcerect, Color.White, Rotation, _origin, Scale, SpriteEffects.None, Depth);
        }

        // private class for storing animation cyles
        private class AnimationCycle
        {
            public int StartPosX { get; set; } // X position on the sheet where the loop begins
            public int StartPosY { get; set; } // Y position on the sheet where the loop begins
            public int FrameWidth { get; set; } // Width of each animation frame
            public int FrameHeight { get; set; } // Height of each animation frame
            public int FrameCount { get; set; } // Total number of frames in the animation
            public int CurrentFrame { get; set; } // The current frame this animation displays
        }

    }
}
