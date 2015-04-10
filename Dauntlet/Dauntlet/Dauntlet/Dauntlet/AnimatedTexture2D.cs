using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public struct AnimatedTexture2D
    {
        private readonly Texture2D _sheet;
        private readonly Dictionary<string, AnimationCycle> _animations;
        private AnimationCycle _currentAnimation;
        private string _currentAnimationName;
        internal float Timer;

        // =============================================================

        public Texture2D Sheet { get { return _sheet; } }
        public bool Flipped { get { return _currentAnimation.Flipped; } }
        public float Height { get { return _currentAnimation.FrameHeight; } }
        public float Width { get { return _currentAnimation.FrameWidth; } }
        public int Frame
        {
            get { return _currentAnimation.CurrentFrame; }
            set
            {
                _currentAnimation.CurrentFrame = (_currentAnimation.IsOneTime &&
                    _currentAnimation.CurrentFrame + 1 == _currentAnimation.FrameCount) ?
                    _currentAnimation.CurrentFrame : value % _currentAnimation.FrameCount;
            }
        }
        public Rectangle CurrentFrame
        {
            get
            {
                return new Rectangle(
                        _currentAnimation.StartPosX + (_currentAnimation.FrameWidth * _currentAnimation.CurrentFrame),
                        _currentAnimation.StartPosY, _currentAnimation.FrameWidth, _currentAnimation.FrameHeight);
            }
        }

        // =============================================================

        public AnimatedTexture2D(Texture2D sheet)
        {
            _sheet = sheet;
            _currentAnimationName = "default";
            _currentAnimation = new AnimationCycle
            {
                CurrentFrame = 0,
                FrameCount = 1,
                FrameHeight = _sheet.Height,
                FrameWidth = _sheet.Width,
                StartPosX = 0,
                StartPosY = 0,
                FramesPerSecond = 1/24f,
                Flipped = false
            };
            _animations = new Dictionary<string, AnimationCycle> { { _currentAnimationName, _currentAnimation } };
            Timer = 0;
        }

        public bool StepAnimation(GameTime gameTime)
        {
            if (_currentAnimation.IsOneTime && _currentAnimation.FrameCount == _currentAnimation.CurrentFrame + 1)
                return true;
            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (!(Timer > _currentAnimation.FramesPerSecond * 1000)) return false;
            Frame++;
            Timer = 0;
            return false;
        }

        public void AddAnimation(string name, int startPosX, int startPosY, int frameWidth, int frameHeight, int frameCount, float fps, bool flipped, bool isOneTime)
        {
            var newAni = new AnimationCycle
            {
                StartPosX = startPosX,
                StartPosY = startPosY,
                FrameWidth = frameWidth,
                FrameHeight = frameHeight,
                FrameCount = frameCount,
                CurrentFrame = 0,
                FramesPerSecond = fps,
                Flipped = flipped,
                IsOneTime = isOneTime
            };
            _animations.Add(name, newAni);
        }

        public void SetAnimation(string name)
        {
            if (name.Equals(_currentAnimationName)) return;
            _currentAnimationName = name;
            _currentAnimation = _animations[name];
            _currentAnimation.CurrentFrame = 0;
            Timer = 0;
        }

        public enum AniCycleState
        {
            Loop,
            PlayOnce,
            Stopped,

        }

        // private struct for storing animation cyles
        private struct AnimationCycle
        {
            public int StartPosX { get; set; } // X position on the sheet where the loop begins
            public int StartPosY { get; set; } // Y position on the sheet where the loop begins
            public int FrameWidth { get; set; } // Width of each animation frame
            public int FrameHeight { get; set; } // Height of each animation frame
            public int FrameCount { get; set; } // Total number of frames in the animation
            public int CurrentFrame { get; set; } // The current frame this animation displays
            public float FramesPerSecond { get; set; } // The FPS at which to animate this cycle
            public bool Flipped { get; set; } // If this sprite is to be flipped horizontally on render
            public bool IsOneTime { get; set; } // If this is a one-time animation
        }

    }
}
