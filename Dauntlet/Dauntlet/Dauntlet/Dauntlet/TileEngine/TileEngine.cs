using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dauntlet.GameScreens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.TileEngine
{

    /// <summary>
    /// Stores a cardinal direction
    /// </summary>
    public enum Dir { N, S, E, W }

    public static class TileEngine
    {
        private const int    SizeOfOneTile = 64;              // Size of 1 tile in pixels
        private const string FirstRoomName = "MausoleumRoom"; // Name of the initial room in which the player spawns

        // -----------------------------

        private static GameplayScreen           _game;            // Reference to the game screen
        private static Texture2D                _tileSet;         // The texture holding the main map tile set
        private static Dictionary<string, Room> _rooms;           // List of all rooms in the game
        private static string                   _currentRoomName; // Name of the currently loaded room

        ///<sumary>
        ///The current room's name
        /// </sumary>
        public static String CurrentRoomName { get { return _currentRoomName; } }


        /// <summary>
        /// The current room
        /// </summary>
        public static Room CurrentRoom { get { return _rooms[_currentRoomName]; } }
        /// <summary>
        /// The size of one tile in pixels
        /// </summary>
        public static int TileSize { get { return SizeOfOneTile; } }

        // -----------------------------

        /// <summary>
        /// Loads all the room files into the tile engine
        /// </summary>
        /// <param name="game">Gameplay screen</param>
        /// <param name="content">the game's ContentManager object</param>
        public static void LoadContent(GameplayScreen game, ContentManager content)
        {
            _game = game;
            _tileSet = content.Load<Texture2D>(@"Textures/Tiles/tilesheet2");
            _rooms = new Dictionary<string, Room>();
            ParseRooms();
            _currentRoomName = FirstRoomName;
        }

        /// <summary>
        /// Parse the files into complete room objects
        /// </summary>
        private static void ParseRooms()
        {
            foreach (string file in Directory.EnumerateFiles("Rooms", "*.csv"))
            {
                // Read file into lines
                string[] fileLines = File.ReadAllLines(file);
                int mapHeight = fileLines.Length - 3;
                int mapWidth = fileLines[0].Split(',').Length;
                var map = new Tile[mapHeight][];

                // Populate the map with tiles
                for (int i = 0; i < mapHeight; i++) {
                    string[] columns = fileLines[i].Split(',');
                    var tiles = new Tile[mapWidth];
                    for (int j = 0; j < columns.Length; j++) {
                        string[] tileId = columns[j].Split(';');
                        int y = Int32.Parse(tileId[0]);
                        int x = Int32.Parse(tileId[1]);
                        tiles[j] = new Tile(new[] { x, y }, new Vector2(j, i - 1), (!(x == 0 && y == 0) && y < 2), y == 1, (x == 0 && y == 0));
                    }
                    map[i] = tiles;
                }

                // Create a new room out of the map and add it to the list
                var theRoom = new Room(map, mapHeight, mapWidth);
                if (file != null) _rooms.Add(Path.GetFileNameWithoutExtension(file), theRoom);

                // Parse and add teleports
                string[] teleports = fileLines[fileLines.Length - 3].Split(',');
                foreach (var tp in from s in teleports where !s.Equals("") select s.Split(';') into info
                                   let id = info[0].ToCharArray()[0]
                                   let w = Convert.ToInt32(info[1]) let h = Convert.ToInt32(info[2])
                                   let pos = Units.SimVector((float) Convert.ToDouble(info[3]), (float) Convert.ToDouble(info[4]))
                                   let tpToRoom = info[5]
                                   let stuff = new List<object> { id, _currentRoomName, w, h, tpToRoom }
                                   let b = BodyFactory.CreateRectangle(theRoom.World, w.Sim(), h.Sim(), 1f,
                                        new Vector2(pos.X + w.Sim()/2f, pos.Y + h.Sim()/2f), stuff)
                                   select new Teleport(id, h, w, pos, b) {CollisionBody = {CollisionCategories = Category.Cat29}})
                    theRoom.Teleports.Add(tp.Id, tp);

                // Parse and add static room objects
                string[] staticEntities = fileLines[fileLines.Length - 2].Split(',');
                foreach (var se in from s in staticEntities where !s.Equals("") select s.Split(';') into info
                                   let type = (ObjectTypes)Enum.Parse(typeof(ObjectTypes), info[0])
                                   select SpriteFactory.CreateStaticEntity(theRoom.World,
                                        new Vector2((float)Convert.ToDouble(info[1]), (float)Convert.ToDouble(info[2])), type))
                    theRoom.Entities.Add(se);

                // Parse and add enemies
                string[] enemies = fileLines[fileLines.Length - 1].Split(',');
                foreach (var se in from s in enemies select s.Split(';') into info
                                   let type = (EnemyTypes)Enum.Parse(typeof(EnemyTypes), info[0])
                                   select SpriteFactory.CreateEnemy(theRoom.World,
                                        new Vector2((float)Convert.ToDouble(info[1]), (float)Convert.ToDouble(info[2])), type))
                    theRoom.Entities.Add(se);
            }
        }

        /// <summary>
        /// Draws the current room map to the screen
        /// </summary>
        /// <param name="spriteBatch">the game's SpriteBatch object</param>
        /// <param name="gameTime">the game's GameTime object</param>
        public static void DrawRoom(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < CurrentRoom.Map.Length; i++)
                for (int j = 0; j < CurrentRoom.Map[i].Length; j++)
                {
                    Tile t = CurrentRoom.Map[i][j];
                    var position = new Vector2(j * TileSize, i * TileSize);
                    var sourcerect = new Rectangle(t.SpriteId[0] * TileSize,
                        t.SpriteId[1] * TileSize, TileSize, TileSize);
                    if (t.IsWall || t.IsCappableWall)
                        spriteBatch.Draw(_tileSet, position, sourcerect, Color.White, 0f, Vector2.Zero, 1f,
                            SpriteEffects.None, ConvertUnits.ToSimUnits(position.Y + TileSize) / 100f);
                    else
                        spriteBatch.Draw(_tileSet, position, sourcerect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
        }

        /// <summary>
        /// Draws the tops of wall tiles in a map.  The shape drawn varies dynamically with the surrounding wall structure.
        /// </summary>
        /// <param name="spriteBatch">the game's SpriteBatch object</param>
        public static void DrawWallCaps(SpriteBatch spriteBatch)
        {
            Texture2D capTexture = SpriteFactory.GetRectangleTexture(TileSize / 2, TileSize / 2, new Color(139, 188, 204));

            for (int i = 0; i < CurrentRoom.WallCaps.Length; i++)
                for (int j = 0; j < CurrentRoom.WallCaps[i].Length; j++)
                {
                    var position = new Vector2(j * TileSize, (i - 1) * TileSize);
                    int capType = CurrentRoom.WallCaps[i][j];
                    if (capType == 0) continue;
                    if (capType == 4 || capType == 6 || capType == 8)
                        spriteBatch.Draw(capTexture, position, null, Color.White, 0f,
                            Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    if (capType == 5 || capType == 7 || capType == 8)
                        spriteBatch.Draw(capTexture, position + new Vector2(TileSize / 2f, 0), null,
                            Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    if (capType != 2 && capType != 5)
                        spriteBatch.Draw(capTexture, position + new Vector2(0, TileSize / 2f), null,
                            Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    if (capType != 1 && capType != 4)
                        spriteBatch.Draw(capTexture, position + new Vector2(TileSize / 2f, TileSize / 2f), null,
                            Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
        }

        /// <summary>
        /// Draws colored debug highlights on all map tiles
        /// </summary>
        /// <param name="spriteBatch">the game's SpriteBatch object</param>
        /// <param name="graphics">the game's GraphicsDevice object</param>
        public static void DrawDebug(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            var redRect = SpriteFactory.GetRectangleTexture(TileSize, TileSize, new Color(1, 0, 0, 0.1f));

            foreach (var body in CurrentRoom.World.BodyList)
                switch (body.FixtureList[0].CollisionCategories)
                {
                    case Category.Cat30: // Wall
                        spriteBatch.Draw(redRect, ConvertUnits.ToDisplayUnits(body.Position) - new Vector2(TileSize / 2f, TileSize / 2f),
                            null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                        break;

                    case Category.Cat29: // Teleport
                        var w = (int)((List<Object>) body.UserData)[2];
                        var h = (int)((List<Object>)body.UserData)[3];
                        var blueRect = SpriteFactory.GetRectangleTexture(h, w, new Color(0, 0, 1, 0.1f));
                        spriteBatch.Draw(blueRect, ConvertUnits.ToDisplayUnits(body.Position) - new Vector2(w / 2f, h / 2f),
                            null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                        break;
                }
        }

        /// <summary>
        /// On teleport, switches the active room and finds the location in the room where the player enters
        /// </summary>
        /// <param name="tpId">ID of the activated teleport</param>
        /// <param name="direction">direction from which the player collided with the teleport</param>
        public static void HandleTeleport(char tpId, Dir direction)
        {
            Teleport tpFrom = CurrentRoom.Teleports[tpId];
            _currentRoomName = (String)((List<Object>)tpFrom.CollisionBody.UserData)[4];
            _game.World = CurrentRoom.World;
            Teleport tpTo = CurrentRoom.Teleports[tpFrom.Id];
            Vector2 pos = GameplayScreen.Player.Position;
            float relativeX = pos.X - tpFrom.Position.X;
            float relativeY = pos.Y - tpFrom.Position.Y;
            float radius = GameplayScreen.Player.DisplayRadius;

            switch (direction)
            {
                case Dir.N:
                    pos.X = tpTo.Position.X + relativeX;
                    pos.Y = tpTo.Position.Y + ConvertUnits.ToSimUnits(tpTo.Height + radius + 1);
                    break;
                case Dir.S:
                    pos.X = tpTo.Position.X + relativeX;
                    pos.Y = tpTo.Position.Y - ConvertUnits.ToSimUnits(radius + 1);
                    break;
                case Dir.W:
                    pos.X = tpTo.Position.X + ConvertUnits.ToSimUnits(tpTo.Width + radius + 1);
                    pos.Y = tpTo.Position.Y + relativeY;
                    break;
                case Dir.E:
                    pos.X = tpTo.Position.X - ConvertUnits.ToSimUnits(radius + 1);
                    pos.Y = tpTo.Position.Y + relativeY;
                    break;
                default:
                    throw new ArgumentException("Direction does not exist!");
            }

            GameplayScreen.Player.ChangeRoom(CurrentRoom.World, pos);
        }
    }
}
