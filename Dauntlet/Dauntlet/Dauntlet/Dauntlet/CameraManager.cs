using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public static class CameraManager
    {
        private const int BlackBuffer = 1; // min # of tiles buffer between screen edge and map edge
        private const int ScrollBuffer = 5; // # of tiles from screen edge until screen starts scrolling

        // ----------------------------

        private static int _screenWidth;
        private static int _screenHeight;
        private static Vector2 _cameraPosition;
        private static Vector2 _screenCenter;
        private static Vector2 _screenScrollBuffer;
        private static int Tile { get { return TileEngine.TileSize; } }
        private static int Buffer { get { return Tile*BlackBuffer; } }
        private static Vector2 Room { get { return new Vector2(TileEngine.CurrentRoom.PixelWidth, TileEngine.CurrentRoom.PixelHeight);} }

        // ---------------------------

        public static void Init(GraphicsDevice graphics)
        {
            _screenWidth = graphics.Viewport.Width;
            _screenHeight = graphics.Viewport.Height;
            _cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(graphics.Viewport.Width / 2f, graphics.Viewport.Height / 2f);
            _screenScrollBuffer = new Vector2(ScrollBuffer * Tile, ScrollBuffer * Tile);
        }

        public static Matrix MoveCamera(Vector2 playerPosition)
        {
            if (TileEngine.CurrentRoom.PixelWidth <= _screenWidth &&
                TileEngine.CurrentRoom.PixelHeight <= _screenHeight)
            {
                //Center room on screen
                _cameraPosition.X = (_screenWidth - Room.X) / 2f;
                _cameraPosition.Y = (_screenHeight - Room.Y) / 2f;
            }
            else
            {
                // Get player's displacement relative to the camera
                Vector2 displacement = playerPosition + _cameraPosition;

                if (displacement.X < _screenScrollBuffer.X && _cameraPosition.X < Buffer)
                    _cameraPosition.X = Math.Min(_cameraPosition.X + (_screenScrollBuffer.X - displacement.X), Buffer);
                if (displacement.X > _screenWidth - _screenScrollBuffer.X && _cameraPosition.X > _screenWidth - (Room.X + Buffer))
                    _cameraPosition.X = Math.Max(_cameraPosition.X + (_screenWidth - _screenScrollBuffer.X - displacement.X), _screenWidth - (Room.X + Buffer));
                if (displacement.Y < _screenScrollBuffer.Y && _cameraPosition.Y < Buffer)
                    _cameraPosition.Y = Math.Min(_cameraPosition.Y + (_screenScrollBuffer.Y - displacement.Y), Buffer);
                if (displacement.Y > _screenHeight - _screenScrollBuffer.Y && _cameraPosition.Y > _screenHeight - (Room.Y + Buffer))
                    _cameraPosition.Y = Math.Max(_cameraPosition.Y + (_screenHeight - _screenScrollBuffer.Y - displacement.Y), _screenHeight - (Room.Y + Buffer));
            }

            return Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));
        }

    }
}
