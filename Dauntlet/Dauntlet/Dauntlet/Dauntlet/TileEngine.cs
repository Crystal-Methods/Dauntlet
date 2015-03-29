using System;
using System.Collections.Generic;
using System.IO;
using Dauntlet.Entities;
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
        internal const int SizeOfOneTile = 64; // Size of 1 tile in pixels

        // -----------------------------

        internal static GameplayScreen Game;
        public static Texture2D TileSet;
        public static Dictionary<string, Room> Rooms;
        public static List<string> RoomList;
        public static string CurrentRoomName = "Fountain2";

        public static Room CurrentRoom { get { return Rooms[CurrentRoomName]; } }
        public static int TileSize { get { return SizeOfOneTile; } }

        // -----------------------------

        public static void LoadContent(GameplayScreen game, ContentManager content)
        {
            Game = game;
            TileSet = content.Load<Texture2D>(@"Textures/tilesheet2");
            Rooms = new Dictionary<string, Room>();

            // Parse Room files
            ParseRooms();

            if (CurrentRoomName == null)
                CurrentRoomName = "testroom";
        }

        private static void ParseRooms()
        {
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
                        int y = Int32.Parse(tileId[0]);
                        int x = Int32.Parse(tileId[1]);
                        tiles[j] = new Tile(new[] {x, y}, new Vector2(j, i - 1), y == 0);
                    }
                    map[i] = tiles;
                }

                var theRoom = new Room(map, null, mapHeight, mapWidth);
                if (file != null) Rooms.Add(Path.GetFileNameWithoutExtension(file), theRoom);

                string[] staticEntities = fileLines[fileLines.Length - 2].Split(',');
                foreach (var s in staticEntities)
                {
                    string[] info = s.Split(';');
                    var type = (ObjectTypes) Enum.Parse(typeof (ObjectTypes), info[0]);
                    var se = SpriteFactory.CreateStaticEntity(theRoom.World,
                        new Vector2((float) Convert.ToDouble(info[1]), (float) Convert.ToDouble(info[2])), type);
                    theRoom.Entities.Add(se);
                }

                string[] enemies = fileLines[fileLines.Length - 1].Split(',');
                foreach (var s in enemies)
                {
                    string[] info = s.Split(';');
                    var type = (EnemyTypes) Enum.Parse(typeof (EnemyTypes), info[0]);
                    var se = SpriteFactory.CreateEnemy(theRoom.World,
                        new Vector2((float) Convert.ToDouble(info[1]), (float) Convert.ToDouble(info[2])), type);
                    theRoom.Entities.Add(se);
                }
            }
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
            var redRect = SpriteFactory.GetRectangleTexture(TileSize, TileSize, new Color(1, 0, 0, 0.1f));
            var blueRect = SpriteFactory.GetRectangleTexture(TileSize, TileSize, new Color(0, 0, 1, 0.1f));

            foreach (var body in CurrentRoom.World.BodyList)
                switch (body.FixtureList[0].CollisionCategories)
                {
                    case Category.Cat2:
                        spriteBatch.Draw(redRect, ConvertUnits.ToDisplayUnits(body.Position) -
                                                  new Vector2(TileSize / 2f, TileSize / 2f), Color.White);
                        break;
                    case Category.Cat10:
                        spriteBatch.Draw(blueRect, ConvertUnits.ToDisplayUnits(body.Position) -
                                                   new Vector2(TileSize / 2f, TileSize / 2f), Color.White);
                        break;
                }
        }

        public static void HandleTeleport(Body collidedTpBody)
        {
            Tile from = CurrentRoom.Map[(int)ConvertUnits.ToDisplayUnits(collidedTpBody.Position.Y) / TileSize][(int)ConvertUnits.ToDisplayUnits(collidedTpBody.Position.X) / TileSize];
            CurrentRoomName = from.TeleportTo;
            Game.World = CurrentRoom.World;
            Tile to = CurrentRoom.GetTpDestination(Char.ToLower(from.TeleportId));
            Vector2 newPos = to.Position;
            newPos.X *= TileSize;
            newPos.X += TileSize/2f;
            newPos.Y *= TileSize;
            newPos.Y += TileSize/2f;
            GameplayScreen.Player.ChangeRoom(CurrentRoom.World, ConvertUnits.ToSimUnits(newPos));
        }
    }

    // Represents a map tile
    public struct Tile
    {
        public int[] SpriteId;
        public char TeleportId;
        public string TeleportTo;
        public bool IsTeleport;
        public bool IsWall;
        public Vector2 Position;

        //public Tile(int spriteId, Vector2 position, char tpId, string tpTo)
        //{
        //    SpriteId = spriteId;
        //    TeleportId = tpId;
        //    TeleportTo = tpTo;
        //    IsWall = spriteId == 1;
        //    IsTeleport = true;
        //    Position = position;
        //}

        public Tile(int[] spriteId, Vector2 position, bool isWall)
        {
            SpriteId = spriteId;
            TeleportId = '\0';
            TeleportTo = null;
            IsTeleport = false;
            Position = position;
            IsWall = isWall;
        }
    }

    // Represents a room
    public struct Room
    {
        public World World;
        public List<Entity> Entities; 
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
            Entities = new List<Entity>();
            _map = map;
            _tpFroms = tpFroms;
            Height = height;
            Width = width;
            
            for (int i = 0; i < _map.Length; i++)
                for (int j = 0; j < _map[i].Length; j++)
                {
                    if (_map[i][j].IsWall)
                    {
                        Body newBody = BodyFactory.CreateRectangle(World, 1f, 1f, 1, new Vector2(j + 0.5f, i + 0.5f));
                        newBody.BodyType = BodyType.Static;
                        newBody.CollisionCategories = Category.Cat2;
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
