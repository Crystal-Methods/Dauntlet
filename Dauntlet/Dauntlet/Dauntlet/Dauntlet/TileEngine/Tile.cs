using Microsoft.Xna.Framework;

namespace Dauntlet.TileEngine
{
    /// <summary>
    /// Represents a single tile of the map
    /// </summary>
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

        /// <summary>
        /// Creates a new Tile entity
        /// </summary>
        /// <param name="spriteId">ID of the sprite to render to this tile</param>
        /// <param name="position">position of the tile in sim units</param>
        /// <param name="isWall">true if this </param>
        /// <param name="isCappableWall"></param>
        /// <param name="isVoid"></param>
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
}