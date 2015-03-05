using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public static class TileEngine
    {
        public static Texture2D TileSet; // Tile texture
        public static Dictionary<string, Room> Rooms; // Holds all the various rooms
        public static List<string> RoomList; // Holds the names of all the rooms in Rooms
        public static string CurrentRoomName = "testroom"; // The current room being drawn
        internal const int TileSize = 32;
        internal static Game1 Game;

        public static Room CurrentRoom { get { return Rooms[CurrentRoomName]; } }

        // Creates all the Room objects from any number of .csv files in the Rooms directory
        public static void LoadContent(Game1 game, ContentManager content)
        {
            Game = game;
            TileSet = content.Load<Texture2D>(@"Textures/tileset");
            Rooms = new Dictionary<string, Room>();

            foreach (string file in Directory.EnumerateFiles("Rooms", "*.csv"))
            {
                string[] rows = File.ReadAllLines(file);
                int height = rows.Length -1;
                int width = (rows[1].Length + 1) / 2;
                var map = new int[height][];

                string[] adjacentRooms = rows[0].Split(',');

                for (int i = 1; i < rows.Length; i++)
                {
                    string[] columns = rows[i].Split(',');
                    var intCells = new int[width];
                    for (int j = 0; j < columns.Length; j++)
                        intCells[j] = Int32.Parse(columns[j]);
                        
                    map[i-1] = intCells;
                }

                if (file != null) Rooms.Add(Path.GetFileNameWithoutExtension(file), new Room(map, adjacentRooms, height, width));
            }

            if (CurrentRoomName == null)
                CurrentRoomName = "testroom";
        }

        // Draws the room
        public static void DrawRoom(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < CurrentRoom.Map.Length; i++)
                for (int j = 0; j < CurrentRoom.Map[i].Length; j++)
                {
                    var position = new Vector2(j * 32, i * 32);
                    var sourcerect = new Rectangle(CurrentRoom.Map[i][j] * 32, 0, 32, 32);
                    spriteBatch.Draw(TileSet, position, sourcerect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
        }

        public static void HandleRooms()
        {
            if (Game._player.SimPosition.Y <= 0f)
            {
                CurrentRoomName = CurrentRoom.AdjRoomN;
                Game._player.ChangeRoom(CurrentRoom.World, true, CurrentRoom.MetricHeight - ConvertUnits.ToSimUnits(TileSize / 2f));
            }
            else if (Game._player.SimPosition.Y >= CurrentRoom.MetricHeight)
            {
                CurrentRoomName = CurrentRoom.AdjRoomS;
                Game._player.ChangeRoom(CurrentRoom.World, true, ConvertUnits.ToSimUnits(TileSize/2f));
            }
            else if (Game._player.SimPosition.X <= 0f)
            {
                CurrentRoomName = CurrentRoom.AdjRoomW;
                Game._player.ChangeRoom(CurrentRoom.World, false, CurrentRoom.MetricWidth - ConvertUnits.ToSimUnits(TileSize / 2f));
            }
            else if (Game._player.SimPosition.X >= CurrentRoom.Width)
            {
                CurrentRoomName = CurrentRoom.AdjRoomE;
                Game._player.ChangeRoom(CurrentRoom.World, false, ConvertUnits.ToSimUnits(TileSize / 2f));
            }
            Game.World = CurrentRoom.World;
        }
    }

    public struct Room
    {
        public World World;
        public readonly int Height;
        public readonly int Width;
        private readonly int[][] _map;
        private readonly string[] adjacentRooms;
        //private List<Body> walls;

        public int PixelHeight { get { return Height * TileEngine.TileSize; } }
        public int PixelWidth { get { return Width*TileEngine.TileSize; } }
        public float MetricHeight { get { return ConvertUnits.ToSimUnits(PixelHeight); } }
        public float MetricWidth { get { return ConvertUnits.ToSimUnits(PixelWidth); } }
        public string AdjRoomN { get { return adjacentRooms[0]; } }
        public string AdjRoomS { get { return adjacentRooms[1]; } }
        public string AdjRoomW { get { return adjacentRooms[2]; } }
        public string AdjRoomE { get { return adjacentRooms[3]; } }
        
        //public List<Body> Walls { get { return walls; } } 

        public int[][] Map
        {
            get { return _map; }
        }

        public Room(int[][] map, string[] adjRooms, int height, int width)
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(32f);
            World = new World(Vector2.Zero);
            _map = map;
            adjacentRooms = adjRooms;
            Height = height;
            Width = width;
            //float a = ConvertUnits.ToSimUnits(TileEngine.TileSize);
            for (int i = 0; i < _map.Length; i++)
                for (int j = 0; j < _map[i].Length; j++)
                    if (_map[i][j] == 1)
                    {
                        Body newBody = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(TileEngine.TileSize), ConvertUnits.ToSimUnits(TileEngine.TileSize), 1f,
                            new Vector2(ConvertUnits.ToSimUnits(j * TileEngine.TileSize + TileEngine.TileSize / 2f), ConvertUnits.ToSimUnits(i * TileEngine.TileSize + TileEngine.TileSize / 2f)));
                        newBody.BodyType = BodyType.Static;
                    }
        }
    }
}
