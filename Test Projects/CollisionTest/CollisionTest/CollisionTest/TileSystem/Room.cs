namespace CollisionTest.TileSystem
{
    public class Room
    {
        public readonly int Height;
        public readonly int Width;
        private readonly int[][] _map;

        public int PixelHeight { get { return Height * 32; } }
        public int PixelWidth { get { return Width * 32; } }

        public int[][] Map
        {
            get { return _map; }
        }

        public Room(int[][] map, int height, int width)
        {
            _map = map;
            Height = height;
            Width = width;
        }

    }
}
