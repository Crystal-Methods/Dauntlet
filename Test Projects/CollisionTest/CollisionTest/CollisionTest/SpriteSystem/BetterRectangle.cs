using Microsoft.Xna.Framework;

namespace CollisionTest.SpriteSystem
{
    public struct AABB
    {
        private Vector2 _min;
        private Vector2 _max;

        public float Left
        {
            get { return _min.X; }
            set { _min.X = value; }
        }

        public float Right
        {
            get { return _max.X; }
            set { _max.X = value; }
        }

        public float Top
        {
            get { return _min.Y; }
            set { _min.Y = value; }
        }

        public float Bottom
        {
            get { return _max.Y; }
            set { _max.Y = value; }
        }

        public float Height
        {
            get { return _max.Y - _min.Y; }
        }

        public float Width
        {
            get { return _max.X - _min.X; }
        }

        public Vector2 Center
        {
            get { return new Vector2((_max.X - _min.X)/2 + _min.X, (_max.Y - _min.Y)/2 + _min.Y);}
        }

        public Vector2 Position
        {
            get { return _min; }
            set
            {
                _max += (value - _min);
                _min = value;
            }
        }

        public AABB(float x, float y, float height, float width)
        {
            _min = new Vector2(x, y);
            _max = new Vector2(x + width, y + height);
        }
    }


}
