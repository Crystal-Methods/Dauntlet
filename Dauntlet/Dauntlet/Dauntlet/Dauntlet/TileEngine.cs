using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;
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
        public static string CurrentRoomName = "MausoleumRoom";

        public static Room CurrentRoom { get { return Rooms[CurrentRoomName]; } }
        public static int TileSize { get { return SizeOfOneTile; } }

        // -----------------------------

        public static void LoadContent(GameplayScreen game, ContentManager content)
        {
            Game = game;
            TileSet = content.Load<Texture2D>(@"Textures/Tiles/tilesheet2");
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
                        tiles[j] = new Tile(new[] { x, y }, new Vector2(j, i - 1), (!(x == 0 && y == 0) && y < 2), y == 1, (x == 0 && y == 0));
                    }
                    map[i] = tiles;
                }

                var theRoom = new Room(map, mapHeight, mapWidth);
                if (file != null) Rooms.Add(Path.GetFileNameWithoutExtension(file), theRoom);

                string[] teleports = fileLines[fileLines.Length - 3].Split(',');
                foreach (var s in teleports)
                {
                    if (s.Equals("")) continue;
                    string[] info = s.Split(';');
                    char id = info[0].ToCharArray()[0];
                    var w = Convert.ToInt32(info[1]);
                    var h = Convert.ToInt32(info[2]);
                    var pos = ConvertUnits.ToSimUnits(new Vector2((float)Convert.ToDouble(info[3]), (float)Convert.ToDouble(info[4])));
                    var tpToRoom = info[5];
                    var stuff = new List<object> { id, CurrentRoomName, w, h, tpToRoom };
                    var tp = new Teleport
                    {
                        Id = id,
                        Width = w,
                        Height = h,
                        Position = pos,
                        CollisionBody = BodyFactory.CreateRectangle(theRoom.World, ConvertUnits.ToSimUnits(w), ConvertUnits.ToSimUnits(h), 1f,
                            new Vector2(pos.X + ConvertUnits.ToSimUnits(w / 2f), pos.Y + ConvertUnits.ToSimUnits(h / 2f)), stuff)
                    };
                    tp.CollisionBody.CollisionCategories = Category.Cat10;
                    theRoom.Teleports.Add(tp.Id, tp);
                }

                string[] staticEntities = fileLines[fileLines.Length - 2].Split(',');
                foreach (var s in staticEntities)
                {
                    if (s.Equals("")) continue;
                    string[] info = s.Split(';');
                    var type = (ObjectTypes)Enum.Parse(typeof(ObjectTypes), info[0]);
                    var se = SpriteFactory.CreateStaticEntity(theRoom.World,
                        new Vector2((float)Convert.ToDouble(info[1]), (float)Convert.ToDouble(info[2])), type);
                    theRoom.Entities.Add(se);
                }

                string[] enemies = fileLines[fileLines.Length - 1].Split(',');
                foreach (var s in enemies)
                {
                    string[] info = s.Split(';');
                    var type = (EnemyTypes)Enum.Parse(typeof(EnemyTypes), info[0]);
                    var se = SpriteFactory.CreateEnemy(theRoom.World,
                        new Vector2((float)Convert.ToDouble(info[1]), (float)Convert.ToDouble(info[2])), type);
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
                    spriteBatch.Draw(TileSet, position, sourcerect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
        }

        public static void DrawWallCaps(SpriteBatch spriteBatch)
        {
            Texture2D capTexture = SpriteFactory.GetRectangleTexture(TileSize / 2, TileSize / 2, new Color(139, 188, 204));

            for (int i = 0; i < CurrentRoom.WallCaps.Length; i++)
                for (int j = 0; j < CurrentRoom.WallCaps[i].Length; j++)
                {
                    var position = new Vector2(j * TileSize, i * TileSize - TileSize);
                    int capType = CurrentRoom.WallCaps[i][j];
                    if (capType == 0) continue;
                    if (capType == 4 || capType == 6 || capType == 8)
                        spriteBatch.Draw(capTexture, position, null, Color.White, 0f, Vector2.Zero, 1f,
                            SpriteEffects.None, ConvertUnits.ToSimUnits(position.Y + TileSize) / 100f);
                    if (capType == 5 || capType == 7 || capType == 8)
                        spriteBatch.Draw(capTexture, position + new Vector2(TileSize / 2f, 0), null, Color.White, 0f,
                            Vector2.Zero, 1f, SpriteEffects.None,
                            ConvertUnits.ToSimUnits(position.Y + TileSize) / 100f);
                    if (capType != 2 && capType != 5)
                        spriteBatch.Draw(capTexture, position + new Vector2(0, TileSize / 2f), null, Color.White, 0f,
                            Vector2.Zero, 1f, SpriteEffects.None,
                            ConvertUnits.ToSimUnits(position.Y + TileSize) / 100f);
                    if (capType != 1 && capType != 4)
                        spriteBatch.Draw(capTexture, position + new Vector2(TileSize / 2f, TileSize / 2f), null,
                            Color.White,
                            0f, Vector2.Zero, 1f, SpriteEffects.None,
                            ConvertUnits.ToSimUnits(position.Y + TileSize) / 100f);
                }
        }

        public static void DrawDebug(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            var redRect = SpriteFactory.GetRectangleTexture(TileSize, TileSize, new Color(1, 0, 0, 0.1f));

            foreach (var body in CurrentRoom.World.BodyList)
                switch (body.FixtureList[0].CollisionCategories)
                {
                    case Category.Cat2:
                        spriteBatch.Draw(redRect, ConvertUnits.ToDisplayUnits(body.Position) -
                                                  new Vector2(TileSize / 2f, TileSize / 2f), Color.White);
                        break;
                    case Category.Cat10:
                        var w = (int)((List<Object>) body.UserData)[2];
                        var h = (int)((List<Object>)body.UserData)[3];
                        var blueRect = SpriteFactory.GetRectangleTexture(h, w, new Color(0, 0, 1, 0.1f));
                        spriteBatch.Draw(blueRect, ConvertUnits.ToDisplayUnits(body.Position) -
                                                   new Vector2(w / 2f, h / 2f), Color.White);
                        break;
                }
        }

        public static void HandleTeleport(Body collidedTpBody, char direction)
        {
            //Tile from = CurrentRoom.Map[(int)ConvertUnits.ToDisplayUnits(collidedTpBody.Position.Y) / TileSize][(int)ConvertUnits.ToDisplayUnits(collidedTpBody.Position.X) / TileSize];
            //CurrentRoomName = from.TeleportTo;
            //Game.World = CurrentRoom.World;
            //Tile to = CurrentRoom.GetTpDestination(Char.ToLower(from.TeleportId));
            //Vector2 newPos = to.Position;
            //newPos.X *= TileSize;
            //newPos.X += TileSize/2f;
            //newPos.Y *= TileSize;
            //newPos.Y += TileSize/2f;
            //GameplayScreen.Player.ChangeRoom(CurrentRoom.World, ConvertUnits.ToSimUnits(newPos));

            Teleport tpFrom = CurrentRoom.Teleports[(char)((List<Object>)collidedTpBody.UserData)[0]];
            CurrentRoomName = (String)((List<Object>)tpFrom.CollisionBody.UserData)[4];
            Game.World = CurrentRoom.World;
            Teleport tpTo = CurrentRoom.Teleports[tpFrom.Id];
            Vector2 pos = GameplayScreen.Player.SimPosition;
            float relativeX = pos.X - (float)Math.Truncate(pos.X);
            float relativeY = pos.Y - (float)Math.Truncate(pos.Y);
            if (direction == 'W')
            {
                pos.X = tpTo.Position.X + ConvertUnits.ToSimUnits(tpTo.Width + GameplayScreen.Player.Radius + 1);
                pos.Y = tpTo.Position.Y + relativeY;
            }
            else if (direction == 'E')
            {
                pos.X = tpTo.Position.X - ConvertUnits.ToSimUnits(GameplayScreen.Player.Radius + 1);
                pos.Y = tpTo.Position.Y + relativeY;
            }
            else if (direction == 'S')
            {
                pos.Y = tpTo.Position.Y + ConvertUnits.ToSimUnits(tpTo.Height + GameplayScreen.Player.Radius + 1);
                pos.X = tpTo.Position.X + relativeX;
            }
            else
            {
                pos.Y = tpTo.Position.Y - ConvertUnits.ToSimUnits(GameplayScreen.Player.Radius + 1);
                pos.X = tpTo.Position.X + relativeX;
            }
            GameplayScreen.Player.ChangeRoom(CurrentRoom.World, pos);
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
        public bool IsCappableWall;
        public bool IsVoid;
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

        public Tile(int[] spriteId, Vector2 position, bool isWall, bool isCappableWall, bool isVoid)
        {
            SpriteId = spriteId;
            TeleportId = '\0';
            TeleportTo = null;
            IsTeleport = false;
            Position = position;
            IsWall = isWall;
            IsVoid = isVoid;
            IsCappableWall = isCappableWall;
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
        private readonly int[][] _wallCaps;
        public Dictionary<char, Teleport> Teleports;

        public int PixelHeight { get { return Height * TileEngine.TileSize; } }
        public int PixelWidth { get { return Width * TileEngine.TileSize; } }
        public float MetricHeight { get { return ConvertUnits.ToSimUnits(PixelHeight); } }
        public float MetricWidth { get { return ConvertUnits.ToSimUnits(PixelWidth); } }
        public Tile[][] Map { get { return _map; } }
        public int[][] WallCaps { get { return _wallCaps; } }

        public Room(Tile[][] map, int height, int width)
        {
            ConvertUnits.SetDisplayUnitToSimUnitRatio(TileEngine.TileSize);
            World = new World(Vector2.Zero);
            Entities = new List<Entity>();
            Teleports = new Dictionary<char, Teleport>();

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
                        newBody.CollisionCategories = Category.Cat2;
                    }

                    int v = GetCapValue(j, i);
                    if (v > 3) _map[i][j].SpriteId = new[] { 0, 0 };
                    temp[j] = v;

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
                _wallCaps[i + 1] = temp;
            }
            _wallCaps[0] = new int[_wallCaps[1].Length];
            for (int i = 0; i < _wallCaps[0].Length; i++)
                _wallCaps[0][i] = GetTopRowCapValue(i);
        }

        //public Teleport GetTpDestination(char key) { return _tpFroms[key]; }

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

    public struct Teleport
    {
        public int Height;
        public int Width;
        public Vector2 Position;
        public char Id;
        public Body CollisionBody;
    }

}
