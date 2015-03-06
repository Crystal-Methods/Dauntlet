using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        internal const int TileSize = 32; // Size of 1 tile in pixels

        // -----------------------------

        internal static GameplayScreen Game;
        public static Texture2D TileSet;
        public static Dictionary<string, Room> Rooms;
        public static List<string> RoomList;
        public static string CurrentRoomName = "testroom1";

        public static Room CurrentRoom { get { return Rooms[CurrentRoomName]; } }

        // -----------------------------

        public static void LoadContent(GameplayScreen game, ContentManager content)
        {
            Game = game;
            TileSet = content.Load<Texture2D>(@"Textures/tileset");
            Rooms = new Dictionary<string, Room>();

            foreach (string file in Directory.EnumerateFiles("Rooms", "*.csv"))
            {
                #region ReadFiles
                string[] lines = File.ReadAllLines(file);
                int height = lines.Length - 1;
                int width = lines[1].Split(',').Length;
                var map = new Tile[height][];

                var tpTos = new Dictionary<char, string>();
                string[] teleports = lines[0].Split(';');
                foreach (string[] tpIds in teleports.Select(s => s.Split(',')))
                    for (int i = 0; i < tpIds.Length - 1; i++)
                        tpTos.Add(Convert.ToChar(tpIds[i]), tpIds[tpIds.Length - 1]);

                var tpFroms = new Dictionary<char, Tile>();
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] columns = lines[i].Split(',');
                    width = columns.Length;
                    var tiles = new Tile[width];
                    for (int j = 0; j < columns.Length; j++)
                    {
                        Tile t;
                        String s = columns[j];
                        if (Regex.Matches(s, @"[A-Z]").Count > 0)
                        {
                            char tpId = s.ToCharArray(0, 1)[0];
                            string tpTo = tpTos[tpId];
                            int tileId = Int32.Parse(s.Substring(1, 1));
                            var pos = new Vector2(j, i - 1);
                            t = new Tile(tileId, pos, tpId, tpTo);
                        }
                        else if (Regex.Matches(s, @"[a-z]").Count > 0)
                        {
                            char tpId = s.ToCharArray(0, 1)[0];
                            var pos = new Vector2(j, i - 1);
                            t = new Tile(Int32.Parse(s.Substring(1, 1)), pos);
                            tpFroms.Add(tpId, t);
                        }
                        else
                        {
                            var pos = new Vector2(j, i - 1);
                            t = new Tile(Int32.Parse(s), pos);
                        }
                        tiles[j] = t;
                    }

                    map[i - 1] = tiles;
                } 

                if (file != null) Rooms.Add(Path.GetFileNameWithoutExtension(file), new Room(map, tpFroms, height, width));
                #endregion
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
                    var sourcerect = new Rectangle(CurrentRoom.Map[i][j].SpriteId * 32, 0, 32, 32);
                    spriteBatch.Draw(TileSet, position, sourcerect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
        }

        public static void DrawDebug(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            var rectRed = new Texture2D(graphics, 32, 32);
            var rectBlue = new Texture2D(graphics, 32, 32);
            var data = new Color[32 * 32];
            for (int i = 0; i < data.Length; i++) data[i] = new Color(1, 0, 0, 0.1f);
            rectRed.SetData(data);
            for (int i = 0; i < data.Length; i++) data[i] = new Color(0, 0, 1, 0.1f);
            rectBlue.SetData(data);
            foreach (var body in CurrentRoom.World.BodyList.Where(body => body.IsStatic))
                spriteBatch.Draw(body.FixtureList[0].CollisionCategories != Category.Cat10 ? rectRed : rectBlue,
                    ConvertUnits.ToDisplayUnits(body.Position) -
                    new Vector2(TileSize / 2f, TileSize / 2f), Color.White);
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
        public int SpriteId;
        public char TeleportId;
        public string TeleportTo;
        public bool IsWall;
        public bool IsTeleport;
        public Vector2 Position;

        public Tile(int spriteId, Vector2 position, char tpId, string tpTo)
        {
            SpriteId = spriteId;
            TeleportId = tpId;
            TeleportTo = tpTo;
            IsWall = spriteId == 1;
            IsTeleport = true;
            Position = position;
        }

        public Tile(int spriteId, Vector2 position)
        {
            SpriteId = spriteId;
            TeleportId = '\0';
            TeleportTo = null;
            IsWall = spriteId == 1;
            IsTeleport = false;
            Position = position;
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
            ConvertUnits.SetDisplayUnitToSimUnitRatio(32f);
            World = new World(Vector2.Zero);
            _map = map;
            _tpFroms = tpFroms;
            Height = height;
            Width = width;
            
            for (int i = 0; i < _map.Length; i++)
                for (int j = 0; j < _map[i].Length; j++)
                {
                    if (_map[i][j].IsWall)
                    {
                        Body newBody = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(TileEngine.TileSize),
                            ConvertUnits.ToSimUnits(TileEngine.TileSize), 1f,
                            new Vector2(ConvertUnits.ToSimUnits(j*TileEngine.TileSize + TileEngine.TileSize/2f),
                                ConvertUnits.ToSimUnits(i*TileEngine.TileSize + TileEngine.TileSize/2f)));
                        newBody.BodyType = BodyType.Static;
                    }
                    if (_map[i][j].IsTeleport)
                    {
                        Body newBody = BodyFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(TileEngine.TileSize),
                            ConvertUnits.ToSimUnits(TileEngine.TileSize), 1f,
                            new Vector2(ConvertUnits.ToSimUnits(j * TileEngine.TileSize + TileEngine.TileSize / 2f),
                                ConvertUnits.ToSimUnits(i * TileEngine.TileSize + TileEngine.TileSize / 2f)));
                        newBody.BodyType = BodyType.Static;
                        newBody.CollisionCategories = Category.Cat10;
                    }
                }
        }

        public Tile GetTpDestination(char key) { return _tpFroms[key]; }
    }

}
