using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest
{
    public abstract class Entity
    {
        protected bool ShowBounds = true; // true if the bounding boxes should be drawn

        protected static Texture2D Pixel = Game1.Pixel; // Used for drawing bounding boxes
        
        protected int BoundWidth; // Width of the bounding box
        protected int BoundHeight; // Height of the bounding box
        protected Game Game; // Reference to the Game instance
        
        public Vector2 Position { get; set; } // Where the top left corner of the entity's bounding box is

        // The bounding box represented as a rectangle
        public Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, BoundWidth, BoundHeight); }
        }

        // The center of the bounding box
        public Vector2 Center
        {
            get { return new Vector2(Position.X + (Bounds.Width / 2f), Position.Y + (Bounds.Height / 2f)); }
        }

        protected Entity(Game game, Vector2 position, int boundWidth, int boundHeight)
        {
            Game = game;
            Position = position;
            BoundWidth = boundWidth;
            BoundHeight = boundHeight;
        }

        /// Draws the bounding box
        /// By Sean Colombo, from http://bluelinegamestudios.com/blog
        protected void DrawBorder(SpriteBatch spriteBatch)
        {
            // Draw top line
            spriteBatch.Draw(Pixel, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1), Color.Black);

            // Draw left line
            spriteBatch.Draw(Pixel, new Rectangle(Bounds.X, Bounds.Y, 1, Bounds.Height), Color.Black);

            // Draw right line
            spriteBatch.Draw(Pixel, new Rectangle((Bounds.X + Bounds.Width - 1), Bounds.Y, 1, Bounds.Height), Color.Black);

            // Draw bottom line
            spriteBatch.Draw(Pixel, new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - 1, Bounds.Width, 1), Color.Black);
        }

        // This method determines where the sprite is drawn relative to the bounding box
        // Currently set to draw the sprite so the sprite's center lines up with the box's center
        //    and the sprite's bottom edge is 1/4 of the way from the bottom of the box
        protected abstract Vector2 GetSpriteDrawPos();

        // Draw this entity
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        // Update this entity
        virtual public void Update(GameTime gameTime)
        { }

        // Handle collisions with this entity
        public virtual void HandleCollision(Entity collider) { }

    }
}
