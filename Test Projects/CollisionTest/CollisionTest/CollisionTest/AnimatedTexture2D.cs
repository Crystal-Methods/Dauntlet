using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest
{
    public class AnimatedTexture2D
    {
        private readonly Texture2D _sheet;
        private readonly Dictionary<string, AnimationCycle> _animations = new Dictionary<string, AnimationCycle>();
        private AnimationCycle _currentAnimation;
        private string _currentAnimationName;

        public float Height
        {
            get { return _currentAnimation.FrameHeight; }
        }

        public float Width
        {
            get { return _currentAnimation.FrameWidth; }
        }

        public int Frame
        {
            get { return _currentAnimation.CurrentFrame; }
            set { _currentAnimation.CurrentFrame = value % _currentAnimation.FrameCount; }
        }

        public AnimatedTexture2D(Texture2D sheet)
        {
            _sheet = sheet;
        }

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

        public void SetAnimation(string name)
        {
            if (name.Equals(_currentAnimationName)) return;
            _currentAnimationName = name;
            _currentAnimation = _animations[name];
            _currentAnimation.CurrentFrame = 0;
        }

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

        private class AnimationCycle
        {
            public int StartPosX { get; set; }
            public int StartPosY { get; set; }
            public int FrameWidth { get; set; }
            public int FrameHeight { get; set; }
            public int FrameCount { get; set; }
            public int CurrentFrame { get; set; }
        }

    }
}
