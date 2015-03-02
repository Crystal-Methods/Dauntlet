using System;
using System.Runtime.Serialization;

namespace CollisionTest.TileSystem
{
    internal class OutOfRoomException : Exception, ISerializable
    {
        public OutOfRoomException()
        {
            // Add implementation.
        }

        public OutOfRoomException(string message)
        {
            // Add implementation.
        }

        public OutOfRoomException(string message, Exception inner)
        {
            // Add implementation.
        }

        // This constructor is needed for serialization.
        protected OutOfRoomException(SerializationInfo info, StreamingContext context)
        {
            // Add implementation.
        }
    }
}
