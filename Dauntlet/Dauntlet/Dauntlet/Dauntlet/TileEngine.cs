using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dauntlet.GameScreens;
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
        internal const int tileSize = 64; // Size of 1 tile in pixels

        // -----------------------------

        internal static GameplayScreen Game;
        public static Texture2D TileSet;
        public static Dictionary<string, Room> Rooms;
        public static List<string> RoomList;
        public static string CurrentRoomName = "Fountain";

        public static Room CurrentRoom { get { return Rooms[CurrentRoomName]; } }
        public static int TileSize { get { return tileSize; } }

        // -----------------------------

        public static void LoadContent(GameplayScreen game, ContentManager content)
        {
            Game = game;
            TileSet = content.Load<Texture2D>(@"Textures/tilesheet");
            Rooms = new Dictionary<string, Room>();

            // Parse Collision Data file
            string[] theStringArray = File.ReadAllLines("CollisionData.csv");
            var collisionData = new bool[theStringArray.Length][][];
            for (int i = 0; i < theStringArray.Length; i++)
            {
                string[] temp = theStringArray[i].Split(',');
                var temp2 = new bool[temp.Length][];
                for (int j = 0; j < temp.Length; j++)
                {
                    char[] bits = Convert.ToString(Convert.ToInt32(temp[j], 10), 2).PadLeft(4, '0').ToCharArray();
                    var bools = new bool[4];
                    for (int k = 0; k < 4; k++)
                    {
                        if (bits[k] == '1')
                            bools[k] = true;
                        else
                            bools[k] = false;
                    }
                    temp2[j] = bools;
                }
                    
                collisionData[i] = temp2;
            }

            // Parse Room files
            foreach (string file in Directory.EnumerateFiles("Rooms", "*.csv"))
            {
                string[] fileLines = File.ReadAllLines(file);
                int mapHeight = fileLines.Length - 3;
                int mapWidth = fileLines[0].Split(',').Length;
                var map = new Tile[mapHeight][];

                for (int i = 0; i < mapHeight; i++)
                {
                    string[] columns = fileLines[i].Split(',');
                    var tiles = new Tile[mapWidth];
                    for (int j = 0; j < columns.Length; j++)
                    {
                        string[] tileId = columns[j].Split(';');
                        int Y = Int32.Parse(tileId[0]);
                        int X = Int32.Parse(tileId[1]);
                        tiles[j] = new Tile(new []{X, Y}, new Vector2(j, i - 1), collisionData[Y][X]);
                    }
                    map[i] = tiles;
                }

                if (file != null) Rooms.Add(Path.GetFileNameWithoutExtension(file), new Room(map, null, mapHeight, mapWidth));
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
                    var position = new Vector2(j * TileSize, i * TileSize);
                    var sourcerect = new Rectangle(CurrentRoom.Map[i][j].SpriteId[0] * TileSize, CurrentRoom.Map[i][j].SpriteId[1] * TileSize, TileSize, TileSize);
                    spriteBatch.Draw(TileSet, position, sourcerect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
        }

        public static void DrawDebug(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            var rectRed = new Texture2D(graphics, TileSize/2, TileSize/2);
            var rectBlue = new Texture2D(graphics, TileSize/2, TileSize/2);
            var data = new Color[TileSize/2 * TileSize/2];
            for (int i = 0; i < data.Length; i++) data[i] = new Color(1, 0, 0, 0.1f);
            rectRed.SetData(data);
            for (int i = 0; i < data.Length; i++) data[i] = new Color(0, 0, 1, 0.1f);
            rectBlue.SetData(data);
            foreach (var body in CurrentRoom.World.BodyList.Where(body => body.IsStatic))
                spriteBatch.Draw(body.FixtureList[0].CollisionCategories != Category.Cat10 ? rectRed : rectBlue,
                    ConvertUnits.ToDisplayUnits(body.Position) -
                    new Vector2(TileSize / 4f, TileSize / 4f), Color.White);
        }

        public static void HandleTeleport(Body collidedTpBody)
        {
            Tile from = CurrentRoom.Map[(int)ConvertUnits.ToDisplayUnits(collidedTpBody.Position.Y) / TileSize][(int)ConvertUnits.ToDisplayUnits(collidedTpBody.Position.X) / TileSize];
            CurrentRoomName = from.TeleportTo;
            Game.World = CurrentRoom.World;
            Tile to = CurrentRoom.GetTpDestination(Char.ToLower(from.TeleportId));
            Vector2 newPos = to.Position;
            newPos.X *= TileSize;
            newPos.X += TileSize/2;
            newPos.Y *= TileSize;
            newPos.Y += TileSize/2;
            Game.Player.ChangeRoom(CurrentRoom.World, ConvertUnits.ToSimUnits(newPos));
        }
    }

    // Represents a map tile
    public struct Tile
    {
        public int[] SpriteId;
        public char TeleportId;
        public string TeleportTo;
        public bool IsTeleport;
        public Vector2 Position;
        public bool[] CollisionData;

        //public Tile(int spriteId, Vector2 position, char tpId, string tpTo)
        //{
        //    SpriteId = spriteId;
        //    TeleportId = tpId;
        //    TeleportTo = tpTo;
        //    IsWall = spriteId == 1;
        //    IsTeleport = true;
        //    Position = position;
        //}

        public Tile(int[] spriteId, Vector2 position, bool[] collisionData)
        {
            SpriteId = spriteId;
            TeleportId = '\0';
            TeleportTo = null;
            IsTeleport = false;
            Position = position;
            CollisionData = collisionData;
        }
    }

    // Represents a room
    public struct Room
    {
        public World World;
        public readonly int Height;
        public readonly int Width;
        private readonly Tile[][] _map;
        private readonly Dictionary<char, Tile> _tpFroms; 

        public int PixelHeight { get { return Height * TileEngine.TileSize; } }
        public int PixelWidth { get { return Width*TileEngine.TileSize; } }
        public float MetricHeight { get { return ConvertUnits.ToSimUnits(PixelHeight); } }
        public float MetricWidth { get { return ConvertUnits.ToSimUnits(PixelWidth); } }
        public Tile[][] Map { get { return _map; } }

        public Room(Tile[][] map, Dictionary<char, Tile> tpFroms,  int height, int width)
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(TileEngine.TileSize);
            World = new World(Vector2.Zero);
            _map = map;
            _tpFroms = tpFroms;
            Height = height;
            Width = width;
            
            for (int i = 0; i < _map.Length; i++)
                for (int j = 0; j < _map[i].Length; j++)
                {
                    if (_map[i][j].CollisionData[0])
                    {
                        Body newBody = BodyFactory.CreateRectangle(World, .5f, .5f, 1, new Vector2(j + 0.25f, i + 0.25f));
                        newBody.BodyType = BodyType.Static;
                    }
                    if (_map[i][j].CollisionData[1])
                    {
                        Body newBody = BodyFactory.CreateRectangle(World, .5f, .5f, 1, new Vector2(j + 0.75f, i + 0.25f));
                        newBody.BodyType = BodyType.Static;
                    } 
                    if (_map[i][j].CollisionData[2])
                    {
                        Body newBody = BodyFactory.CreateRectangle(World, .5f, .5f, 1, new Vector2(j + 0.25f, i + 0.75f));
                        newBody.BodyType = BodyType.Static;
                    }
                    if (_map[i][j].CollisionData[3])
                    {
                        Body newBody = BodyFactory.CreateRectangle(World, .5f, .5f, 1, new Vector2(j + 0.75f, i + 0.75f));
                        newBody.BodyType = BodyType.Static;
                    }

                    //if (_map[i][j].IsTeleport)
                    //{
                    //    Body newBody = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(TileEngine.TileSize),
                    //        ConvertUnits.ToSimUnits(TileEngine.TileSize), 1f,
                    //        new Vector2(ConvertUnits.ToSimUnits(j * TileEngine.TileSize + TileEngine.TileSize / 2f),
                    //            ConvertUnits.ToSimUnits(i * TileEngine.TileSize + TileEngine.TileSize / 2f)));
                    //    newBody.BodyType = BodyType.Static;
                    //    newBody.CollisionCategories = Category.Cat10;
                    //}
                }
        }

        public Tile GetTpDestination(char key) { return _tpFroms[key]; }
    }

}
