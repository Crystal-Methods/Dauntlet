using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace Dauntlet.TileEngine
{
    /// <summary>
    /// A collidable body that, when touched, will send the player to a corresponding Teleport elsewhere in the world
    /// </summary>
    public struct Teleport
    {
        public int Height;
        public int Width;
        public Vector2 Position;
        public char Id;
        public Body CollisionBody;

        /// <summary>
        /// Creates a new Teleport entity
        /// </summary>
        /// <param name="id">ID of the teleport.  Corresponds to a teleport with the same ID in another room.</param>
        /// <param name="height">height of the teleport entity, in display units</param>
        /// <param name="width">width of the teleport entity, in display units</param>
        /// <param name="position">position of the teleport entity, in display units</param>
        /// <param name="collisionBody">the collision body for the teleport entity</param>
        public Teleport(char id, int height, int width, Vector2 position, Body collisionBody)
        {
            Id = id;
            Height = height;
            Width = width;
            Position = position;
            CollisionBody = collisionBody;
        }
    }
}