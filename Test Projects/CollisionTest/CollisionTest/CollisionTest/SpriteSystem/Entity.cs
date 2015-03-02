using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest.SpriteSystem
{
    /// <summary>
    /// Represents an object that can interact with the game world
    /// </summary>
    public abstract class Entity
    {
        protected bool ShowBounds = true; // true if the bounding boxes should be drawn

        protected static Texture2D Pixel = Game1.Pixel; // Used for drawing bounding boxes
        protected List<Entity> CollidedEntities = new List<Entity>(); 
        protected Game Game; // Reference to the Game instance
        protected AABB _bounds;
        protected float _mass;
        protected Vector2 _velocity;
        //public Vector2 Position { get; set; } // Where the top left corner of the entity's bounding box is

        public float Mass { get { return _mass; } }
        public Vector2 Velocity { get { return _velocity; } set { _velocity = value; } }
        public float InvMass { get { return Math.Abs(_mass) < 0.000001 ? 0 : 1/_mass; } }

        public AABB FutureBounds
        {
            get
            {
                var newBounds = new AABB
                {
                    Top = _bounds.Top + _velocity.Y,
                    Bottom = _bounds.Bottom + _velocity.Y,
                    Left = _bounds.Left + _velocity.X,
                    Right = _bounds.Right + _velocity.X
                };
                return newBounds;
            }
        }

        // The bounding box represented as an AABB
        public AABB Bounds
        {
            get { return _bounds; }
        }

        protected Entity(Game game, Vector2 position, float boundHeight, float boundWidth)
        {
            Game = game;
            //Position = position;
            _bounds = new AABB(position.X, position.Y, boundHeight, boundWidth);
        }

        /// Draws the bounding box
        /// By Sean Colombo, from http://bluelinegamestudios.com/blog
        protected void DrawBorder(SpriteBatch spriteBatch)
        {
            // Draw top line
            spriteBatch.Draw(Pixel, new Rectangle((int)Bounds.Left, (int)Bounds.Top, (int)Bounds.Width, 1), Color.Black);

            // Draw left line
            spriteBatch.Draw(Pixel, new Rectangle((int)Bounds.Left, (int)Bounds.Top, 1, (int)Bounds.Height), Color.Black);

            // Draw right line
            spriteBatch.Draw(Pixel, new Rectangle(((int)(Bounds.Left + Bounds.Width) - 1), (int)Bounds.Top, 1, (int)Bounds.Height), Color.Black);

            // Draw bottom line
            spriteBatch.Draw(Pixel, new Rectangle((int)Bounds.Left, (int)(Bounds.Top + Bounds.Height) - 1, (int)Bounds.Width, 1), Color.Black);
        }

        // This method determines where the sprite is drawn relative to the bounding box
        // Currently set to draw the sprite so the sprite's center lines up with the box's center
        //    and the sprite's bottom edge is 1/4 of the way from the bottom of the box
        protected abstract Vector2 GetSpriteDrawPos();

        // Draw this entity
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        // Update this entity
        virtual public void Update(GameTime gameTime)
        {
            CollidedEntities.Clear();
        }

        // Handle collisions with this entity
        public virtual void HandleCollision(Entity collider) { }

    }
}
