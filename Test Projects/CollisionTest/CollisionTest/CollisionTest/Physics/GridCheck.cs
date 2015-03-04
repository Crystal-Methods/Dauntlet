using System;
using System.Collections.Generic;
using CollisionTest.TileSystem;
using Microsoft.Xna.Framework;

namespace CollisionTest.Physics
{
    public static class GridCheck
    {
        public static Dictionary<int, List<Entity>> Cells;
        public static Room CurrentRoom { get; set; }

        public static void Init(Room currentRoom)
        {
            CurrentRoom = currentRoom;
            Cells = new Dictionary<int, List<Entity>>(CurrentRoom.Height * CurrentRoom.Width);
        }

        public static void UpdateCells(List<Entity> entities)
        {
            ClearCells();
            foreach (var e in entities)
                RegisterObject(e);
        }

        internal static void ClearCells()
        {
            Cells.Clear();
            for (int i = 0; i < CurrentRoom.Height * CurrentRoom.Width; i++)
            {
                Cells.Add(i, new List<Entity>());
            }
        }

        internal static void RegisterObject(Entity e)
        {
            List<int> cellIds = GetIdForObj(e);
            foreach (var item in cellIds)
            {
                Cells[item].Add(e);
            }
        }

        internal static List<int> GetIdForObj(Entity e)
        {
            int leftTile = (int)e.FutureBounds.Left / 32;      // bounds = AABB of your entity
            int topTile = (int)e.FutureBounds.Top / 32;
            int rightTile = (int)Math.Ceiling(e.FutureBounds.Right / 32) - 1;
            int bottomTile = (int)Math.Ceiling((e.FutureBounds.Bottom / 32)) - 1;
            var cellIDs = new List<int>();

            if (e.FutureBounds.Left < 0 || e.FutureBounds.Top < 0 || rightTile >= CurrentRoom.Width || bottomTile >= CurrentRoom.Height)
                throw new OutOfRoomException("Entity is outside of room boundaries!!");

            for (int y = topTile; y <= bottomTile; ++y)
                for (int x = leftTile; x <= rightTile; ++x)
                    cellIDs.Add((y * CurrentRoom.Width) + x);

            return cellIDs;
        }

        internal static List<Entity> GetNearby(Entity e)
        {
            var objects = new List<Entity>();
            List<int> bucketIds = GetIdForObj(e);
            foreach (var item in bucketIds)
            {
                List<Entity> colliders = Cells[item];
                colliders.Remove(e);
                objects.AddRange(Cells[item]);
            }
            return objects;
        }

    }
}
