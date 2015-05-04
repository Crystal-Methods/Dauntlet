using System;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.Entities
{
    public abstract class Entity
    {
        public static Texture2D DebugCircleTexture;
        public static Texture2D Shadow;

        // ============================================

        protected AnimatedTexture2D SpriteTexture; // This entity's primary texture
        protected Body CollisionBody;              // This entity's collision body
        protected bool IsBobbing;                  // True if this entity "bobs" when floating
        protected float DeathTimer;                // Elapsed time since this entity has entered the "Dying" phase
        protected float HurtTimer;                 // Elapsed time since this entity has been hurt

        /// <summary>
        /// The layer depth of this entity.  Used for creating the illusion of depth in a SpriteBatch.Draw call.
        /// 
        /// This entity must have a collision body defined to use this property.
        /// </summary>
        protected float LayerDepth { get { return Position.Y/100f; } }
        /// <summary>
        /// The layer depth of the provided body.  Used for creating the illusion of depth in a SpriteBatch.Draw call.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected float GetLayerDepth(Body b) { return b.Position.Y/100f; }
        /// <summary>
        /// Distance between the base of this entity's texture from the center of its collision body.  Used to create the illusion of flying or levitation.
        /// </summary>
        protected float OffGroundHeight { get; set; }
        /// <summary>
        /// Calculates the center point of the given texture.
        /// </summary>
        /// <param name="texture">the texture of which to find the center</param>
        /// <returns></returns>
        protected Vector2 CenterOrigin(Texture2D texture) { return new Vector2(texture.Width/2f, texture.Height/2f); }
        /// <summary>
        /// The center point of this entity's shadow texture.
        /// </summary>
        protected Vector2 ShadowOrigin { get { return new Vector2(Shadow.Width / 2f, Shadow.Height / 2f); } }
        /// <summary>
        /// The point at which this entity's texture is drawn relative to the texture.
        /// 
        /// The point is given as half the width and a quarter of the way up from the base.
        /// </summary>
        protected Vector2 SpriteOrigin { get { return new Vector2(SpriteTexture.Width / 2f, 3 * SpriteTexture.Height / 4f); } }
        /// <summary>
        /// Gets the position at which to draw the sprite relative to the collision body
        /// </summary>
        /// <returns>the position at which to draw the sprite, given in sim units</returns>
        protected Vector2 SpritePosition() { return new Vector2(DisplayPosition.X, DisplayPosition.Y - OffGroundHeight); }
        /// <summary>
        /// Gets the position at which to draw the sprite relative to the collision body
        /// </summary>
        /// <param name="bobFactor">extra vertical distance to create a "bobbing" effect</param>
        /// <returns>the position at which to draw the sprite, given in sim units</returns>
        protected Vector2 SpritePosition(float bobFactor) { return new Vector2(DisplayPosition.X, DisplayPosition.Y - OffGroundHeight + bobFactor);}

        // =============================================

        public float Speed;    // Entity's top speed
        public float Mass;     // Collision body's mass
        public int  HitPoints; // Entity's max hit points
        public bool Dying;     // True if this entity is about to die.  Used for post-death animations.
        public bool Dead;      // True if this entity is dead
        public bool Hurt;      // True if this entity has been damaged.  Used for post-hit invulnerability.

        /// <summary>
        /// Collision body's radius, if circular, given in sim units
        /// </summary>
        public float Radius { get; set; }
        /// <summary>
        /// Collision body's radius, if circular, given in display units
        /// </summary>
        public float DisplayRadius { get { return ConvertUnits.ToDisplayUnits(Radius); } }
        /// <summary>
        /// Collision body's height, if rectangular, given in sim units
        /// </summary>
        public float Height { get; set; }
        /// <summary>
        /// Collision body's height, if rectangular, given in display units
        /// </summary>
        public float DisplayHeight { get { return ConvertUnits.ToDisplayUnits(Height); } }
        /// <summary>
        /// Collision body's width, if rectangular, given in sim units
        /// </summary>
        public float Width { get; set; }
        /// <summary>
        /// Collision body's width, if rectangular, given in display units
        /// </summary>
        public float DisplayWidth { get { return ConvertUnits.ToDisplayUnits(Width); } }
        /// <summary>
        /// The position of this entity given in Farseer (sim) units
        /// </summary>
        public Vector2 Position { get { return CollisionBody.Position; } }
        /// <summary>
        /// The position of this entity given in screen (display) units
        /// </summary>
        public Vector2 DisplayPosition { get { return ConvertUnits.ToDisplayUnits(CollisionBody.Position); } }
        /// <summary>
        /// The rotation of this entity given in radians.
        /// 
        /// 0 is facing right.  Counterclockwise is positive, clockwise is negative.
        /// </summary>
        public float Rotation { get { return CollisionBody.Rotation; } }
        /// <summary>
        /// A reference to this entity's collision body
        /// </summary>
        public Body GetBody
        {
            get { return CollisionBody; }
            set { CollisionBody = value; }
        }

        // ==============================================

        /// <summary>
        /// Update the entity
        /// </summary>
        /// <param name="gameTime">the game's GameTime object</param>
        public abstract void Update(GameTime gameTime);
        /// <summary>
        /// Draw the entity
        /// </summary>
        /// <param name="gameTime">the game's GameTime object</param>
        /// <param name="spriteBatch">the game's SpriteBatch object</param>
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        /// <summary>
        /// Kill off the entity
        /// 
        /// A killed entity will not be drawn or updated, or brought back into play.  An entity's death persists across room traversals and across save loads.
        /// </summary>
        public abstract void Die();

        // ==============================================

        /// <summary>
        /// Inflicts damage upon an entity, subtracting from that entity's HitPoints total.
        /// </summary>
        /// <param name="damage">the amount of damage to inflict</param>
        public virtual void InflictDamage(int damage)
        {
            Hurt = true;
            CollisionBody.FixtureList[0].CollisionCategories = Category.Cat25;
            HurtTimer = 0f;
            HitPoints -= damage;
        }

        /// <sumary>
        /// Heales an entity by adding to the HitPoints without starting HurtTimer
        /// </sumary>
        /// <param name="health">The amount to heal the entity by
        public virtual void Heal(int health)
        {
            HitPoints += health;
        }
    }

    // Helper methods
    public static class EntityHelpers
    {
        /// <summary>
        /// Use this to set a lot of a Body's properties at once
        /// </summary>
        /// <param name="body">the Body itself</param>
        /// <param name="bodyType">this body's movement type</param>
        /// <param name="collisionCategories">the category to which this body belongs</param>
        /// <param name="collidesWith">what body categories can collide with this body</param>
        /// <param name="fixedRotation">true if this body should never rotate</param>
        /// <param name="restitution">restitution (bounciness) of this body</param>
        /// <param name="friction">friction of this body</param>
        /// <param name="linearDamping">linear damping (drag) of this body</param>
        /// <param name="angularDamping">angular damping (rotational drag) of this body</param>
        public static void InitBody(this Body body, BodyType bodyType, Category collisionCategories, Category collidesWith,
            bool fixedRotation, float restitution, float friction, float linearDamping, float angularDamping)
        {
            body.BodyType = bodyType;
            body.CollisionCategories = collisionCategories;
            body.CollidesWith = collidesWith;
            body.FixedRotation = fixedRotation;
            body.Restitution = restitution;
            body.Friction = friction;
            body.LinearDamping = linearDamping;
            body.AngularDamping = angularDamping;
        }

        /// <summary>
        /// Returns this body's density as a function of its mass and size
        /// </summary>
        /// <param name="entity">this entity</param>
        /// <returns>this entity's density</returns>
        public static float Density(this Entity entity)
        {
            if (entity.Radius > 0)
                return  entity.Mass / (float)(Math.PI * Math.Pow(entity.Radius, 2));
            return 1;
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// <param name="radians">the angle to wrap, in radians.</param>
        /// <returns>the input value expressed in radians from -Pi to Pi.</returns>
        /// </summary>
        public static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
                radians += MathHelper.TwoPi;
            while (radians > MathHelper.Pi)
                radians -= MathHelper.TwoPi;
            return radians;
        }

        /// <summary>
        /// Calculates the rotation necessary to point a body towards another position
        /// </summary>
        /// <param name="position">the body's position</param>
        /// <param name="faceThis">the position to face</param>
        /// <returns></returns>
        public static float TurnToFace(Vector2 position, Vector2 faceThis)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            return WrapAngle((float)Math.Atan2(y, x));
        }

        /// <summary>
        /// Calculates the rotation necessary to point a body towards another position
        /// </summary>
        /// <param name="position">the body's position</param>
        /// <param name="faceThis">the position to face</param>
        /// <param name="currentAngle">the body's current rotation angle</param>
        /// <param name="turnSpeed">how quickly this body is allowed to rotate</param>
        /// <returns></returns>
        public static float TurnToFace(Vector2 position, Vector2 faceThis, float currentAngle, float turnSpeed)
        {
            var desiredAngle = TurnToFace(position, faceThis);
            float difference = WrapAngle(desiredAngle - currentAngle);
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return WrapAngle(currentAngle + difference);
        }
    }
}
