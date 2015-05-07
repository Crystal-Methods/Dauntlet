using System.Collections.Generic;
using Dauntlet.Entities;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.TileEngine
{
    /// <summary>
    /// Represents a room in the world.  The room is responsible for all of the tiles and entities it contains.
    /// </summary>
    public class Room
    {
        public World World;
        public List<Entity> Entities;
        public List<Entity> AddQueue;
        public List<Entity> RemoveQueue;
        public List<Texture2D> Render;
        public List<Vector2> RenderLocations; 
        public readonly int Height;
        public readonly int Width;
        private readonly Tile[][] _map;
        private readonly int[][] _wallCaps;
        public Dictionary<char, Teleport> Teleports;

        /// <summary>
        /// The height of the room in pixels
        /// </summary>
        public int DisplayHeight { get { return Height * TileEngine.TileSize; } }
        /// <summary>
        /// Width of the room in pixels
        /// </summary>
        public int DisplayWidth { get { return Width * TileEngine.TileSize; } }
        /// <summary>
        /// Height of the room in sim units
        /// </summary>
        public float SimHeight { get { return DisplayHeight.Sim(); } }
        /// <summary>
        /// Width of the room in sim units
        /// </summary>
        public float SimWidth { get { return DisplayWidth.Sim(); } }
        /// <summary>
        /// A two-dimensional array of the room's Tile objects
        /// </summary>
        public Tile[][] Map { get { return _map; } }
        /// <summary>
        /// A two-dimensional array of values that represent the different shapes of top faces of walls
        /// </summary>
        public int[][] WallCaps { get { return _wallCaps; } }

        /// <summary>
        /// Creates a new Room
        /// </summary>
        /// <param name="map">the Tiles that compose the map</param>
        /// <param name="height">the height of the map in full tiles</param>
        /// <param name="width">the width of the map in full tiles</param>
        public Room(Tile[][] map, int height, int width)
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(TileEngine.TileSize);
            World = new World(Vector2.Zero);
            Entities = new List<Entity>();
            AddQueue = new List<Entity>();
            RemoveQueue = new List<Entity>();
            Teleports = new Dictionary<char, Teleport>();
            Render = new List<Texture2D>();
            RenderLocations = new List<Vector2>();

            _map = map;
            Height = height;
            Width = width;
            _wallCaps = new int[_map.Length + 1][];

            for (int i = 0; i < _map.Length; i++)
            {
                var temp = new int[_map[i].Length];
                for (int j = 0; j < _map[i].Length; j++)
                {
                    if (_map[i][j].IsWall)
                    {
                        Body newBody = BodyFactory.CreateRectangle(World, 1f, 1f, 1, new Vector2(j + 0.5f, i + 0.5f));
                        newBody.BodyType = BodyType.Static;
                        newBody.CollisionCategories = Category.Cat30;
                    }

                    int v = GetCapValue(j, i);
                    if (v > 3) _map[i][j].SpriteId = new[] { 0, 0 };
                    temp[j] = v;
                }
                _wallCaps[i + 1] = temp;
            }
            _wallCaps[0] = new int[_wallCaps[1].Length];
            for (int i = 0; i < _wallCaps[0].Length; i++)
                _wallCaps[0][i] = GetTopRowCapValue(i);
        }

        /// <summary>
        /// Checks if a wall cap should be rendered above this tile based on surrounding walls.  If so, figures out which wall cap to render.
        /// </summary>
        /// <param name="x">X-coordinate of the tile to check, in full tiles</param>
        /// <param name="y">Y-coordinate of the tile to check, in full tiles</param>
        /// <returns>the ID of the wall cap type to render, or zero if no wall cap should be rendered</returns>
        private int GetCapValue(int x, int y)
        {
            if (y + 1 >= _map.Length) return 0;
            if (!_map[y + 1][x].IsCappableWall) return 0;
            if (!_map[y][x].IsCappableWall && !_map[y][x].IsVoid) return 3;
            if (_map[y][x].IsVoid)
            {
                if (x + 1 >= _map[0].Length) return 1;
                if (x == 0) return 2;
                if (_map[y + 1][x + 1].IsVoid) return 1;
                if (_map[y + 1][x - 1].IsVoid) return 2;
                return 3;
            }
            if (x + 1 >= _map[0].Length || _map[y][x + 1].IsVoid)
            {
                if ((x + 1 >= _map[0].Length || _map[y + 1][x + 1].IsVoid) && (y + 2 >= _map.Length || _map[y + 2][x].IsCappableWall)) return 4;
                return 6;
            }
            if (x == 0 || _map[y][x - 1].IsVoid)
            {
                if ((x == 0 || _map[y + 1][x + 1].IsVoid) && (y + 2 >= _map.Length || _map[y + 2][x].IsCappableWall)) return 5;
                return 7;
            }
            return 8;
        }

        /// <summary>
        /// Decides which wall cap type should be rendered in the map row at Y = -1.
        /// </summary>
        /// <param name="x">The X-coordinate of the tile to check, in full tiles</param>
        /// <returns>the ID of the wall cap type to render</returns>
        private int GetTopRowCapValue(int x)
        {
            if (!_map[0][x].IsCappableWall) return 0;
            if (x + 1 >= _map[0].Length) return 1;
            if (x == 0) return 2;
            if (_map[0][x + 1].IsVoid) return 1;
            if (_map[0][x - 1].IsVoid) return 2;
            return 3;
        }

    }
}