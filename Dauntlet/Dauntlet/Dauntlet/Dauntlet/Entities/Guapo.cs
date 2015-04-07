using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Windows.Forms;

namespace Dauntlet.Entities
{
    public class Guapo : EnemyEntity
    {

        private const float GuapoSpeed = .1f; // Speed of the player; CHANGES DEPENDING ON RADIUS!!
        private const float GuapoRadius = 14f; // Radius of player's bounding circle
        private const float GuapoFloatHeight = 15f;

        //AI Stuff
        private float guapoOrientation;
        private Vector2 wanderDirection;
        private static readonly Random Random = new Random();
        const float turnSpeed = 0.2f;
        const float maxSpeed = .02f;//speed of chasing enemy
        const float chaseDistance = 6.5f;
        const float caughtDistance = 1f;
        const float hysteresis = 7.0f;//space in which it does nothing
        private Vector2 playerPosition;
        
        GuapoState guapoState = GuapoState.Wander;

        enum GuapoState
        {
            Chasing,
            Caught,
            Wander
        }

        // ---------------------------------

        public Guapo(World world, Vector2 position, Texture2D spriteTexture) : base(world, position, spriteTexture, GuapoSpeed, GuapoRadius)
        {
            OffGroundHeight = GuapoFloatHeight;
            HitPoints = 3;

            SpriteTexture.AddAnimation("Fly", 0, 0, 32, 32, 5, 1/24f, false);
            SpriteTexture.SetAnimation("Fly");
        }

        // Get player position and other data like this:
        public void UpdateGuapo(Vector2 guapoPos)
        {
            playerPosition = Player.SimPosition;
            

            
            //First, Set Thresholds
            float chaseThreshold = chaseDistance;
            float caughtThreshold = caughtDistance;

            //Make him less likely to attack when idle
            if (guapoState == GuapoState.Wander)
            {
                chaseThreshold -= hysteresis / 2;
            }
            //More likely to be active when active
            else if (guapoState == GuapoState.Chasing)
            {
                chaseThreshold += hysteresis / 2;
                caughtThreshold -= hysteresis / 2;
            }
            //More likely to be caught when caught
            else if (guapoState == GuapoState.Caught)
            {
                caughtThreshold += hysteresis / 2;
            }

            //Second, decide state
            float distanceFromPlayer = Vector2.Distance(playerPosition,guapoPos);
            
            
            //Check for wander
            if (distanceFromPlayer > chaseThreshold)
            {
                guapoState = GuapoState.Wander;
            }
            //Check for chase
            else if (distanceFromPlayer > caughtThreshold)
            {
                guapoState = GuapoState.Chasing;
            }
            //Check for caught
            else
                guapoState = GuapoState.Caught;
                
            

            //Third, move
            float currentSpeed;

            if (guapoState == GuapoState.Chasing)
            {
                guapoOrientation = TurnToFace(guapoPos, playerPosition, guapoOrientation, turnSpeed);
                currentSpeed = maxSpeed;
            }
            else if (guapoState == GuapoState.Wander)
            {
                Wander(guapoPos, ref wanderDirection, ref guapoOrientation,
                   turnSpeed);

                currentSpeed = .05f * maxSpeed;
            }
            else
                currentSpeed = 0.0f;
                
            

            Vector2 heading = new Vector2((float)Math.Cos(guapoOrientation), (float)Math.Sin(guapoOrientation));
            heading.Normalize();

            CollisionBody.ApplyLinearImpulse(heading * currentSpeed);

        }


        private static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {
            
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);

           
            float difference = WrapAngle(desiredAngle - currentAngle);

           
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return WrapAngle(currentAngle + difference);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// <param name="radians">the angle to wrap, in radians.</param>
        /// <returns>the input value expressed in radians from -Pi to Pi.</returns>
        /// </summary>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }



        public void Wander(Vector2 position, ref Vector2 wanderDirection,
            ref float orientation, float turnSpeed)
        {
            //Finds a random direction to go in. The .25 is how erratic the wander is.
            wanderDirection.X +=
                MathHelper.Lerp(-.25f, .25f, (float)Random.NextDouble());
            wanderDirection.Y +=
                MathHelper.Lerp(-.25f, .25f, (float)Random.NextDouble());

            wanderDirection.Normalize();

            orientation = TurnToFace(position, position + wanderDirection, orientation, .15f * turnSpeed);

            //find center of screen
            Vector2 screenCenter = Vector2.Zero;
            screenCenter.X = 1366 / 2;
            screenCenter.Y = 768 / 2;
            //turn the enemy
            float distanceFromScreenCenter = Vector2.Distance(screenCenter, position);
            float MaxDistanceFromScreenCenter =
                Math.Min(screenCenter.Y, screenCenter.X);

            float normalizedDistance =
                distanceFromScreenCenter / MaxDistanceFromScreenCenter;

            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance *
                turnSpeed;

            orientation = TurnToFace(position, screenCenter, orientation,
                turnToCenterSpeed);

            CollisionBody.ApplyLinearImpulse(wanderDirection * orientation * .01f);

        }



        public override void Die()
        {
            Dying = true;
            Dauntlet.SoundBank.PlayCue("GuapoDeath");
            //SoundManager.Play("GuapoDeath");
        }

        public override void InflictDamage(int damage)
        {
            base.InflictDamage(damage);
            Dauntlet.SoundBank.PlayCue("GuapoHurt");
            //SoundManager.Play("GuapoHurt");
        }

        public override void Update(GameTime gameTime)
        {
            if (Dying)
            {
                DeathTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (DeathTimer > 500)
                {
                    Dying = false;
                    Dead = true;
                    CollisionBody.Dispose();
                }
            }

            UpdateGuapo(this.SimPosition);
            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            
        }
    }
}
