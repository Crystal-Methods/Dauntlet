using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest.TileSystem
{
    /// <summary>
    /// Handles the tile system
    /// </summary>
    public static class TileEngine
    {

        public static Texture2D TileSet; // Tile texture
        public static Dictionary<string, Room> Rooms; // Holds all the various rooms
        public static List<string> RoomList; // Holds the names of all the rooms in Rooms
        public static string CurrentRoomName = "testroom"; // The current room being drawn

        public static Room CurrentRoom { get { return Rooms[CurrentRoomName]; } }

        // Creates all the Room objects from any number of .csv files in the Rooms directory
        public static void LoadContent(ContentManager content)
        {
            TileSet = content.Load<Texture2D>(@"Textures/tileset");
            Rooms = new Dictionary<string, Room>();

            foreach (string file in Directory.EnumerateFiles("Rooms", "*.csv"))
            {
                string[] rows = File.ReadAllLines(file);
                int height = rows.Length;
                int width = (rows[0].Length + 1)/2;
                var map = new int[height][];

                for (int i = 0; i < rows.Length; i++)
                {
                    string[] columns = rows[i].Split(',');
                    var intCells = new int[width];
                    for (int j = 0; j < columns.Length; j++)
                        intCells[j] = Int32.Parse(columns[j]);
                    map[i] = intCells;
                }
                Rooms.Add(Path.GetFileNameWithoutExtension(file), new Room(map, height, width));
            }
        }

        // Draws the room
        public static void DrawRoom(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < CurrentRoom.Map.Length; i++)
                for (int j = 0; j < CurrentRoom.Map[i].Length; j++)
                {
                    var position = new Vector2(j*32, i*32);
                    var sourcerect = new Rectangle(CurrentRoom.Map[i][j] * 32, 0, 32, 32);
                    spriteBatch.Draw(TileSet, position, sourcerect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
        }


    }
}
