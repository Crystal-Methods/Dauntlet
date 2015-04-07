using Dauntlet.GameScreens;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;

namespace Dauntlet.Entities
{
    public class Zombie : EnemyEntity
    {
        private const float ZombieSpeed = 30f; // Speed of the player; CHANGES DEPENDING ON RADIUS!!
        private const float ZombieRadius = 14f; // Radius of player's bounding circle
        private const float ZombieFloatHeight = 14f; //How far the base of the zombie is from the shadow

        // ---------------------------------

        private float zombieOrientation;
        private Vector2 wanderDirection;
        private Random random = new Random();
        const float turnSpeed = 0.2f;
        const float maxSpeed = .02f;//speed of chasing enemy
        const float chaseDistance = 6.5f;
        const float caughtDistance = 1f;
        const float hysteresis = 7.0f;//space in which it does nothing
        private Vector2 playerPosition;

        ZombieState guapoState = ZombieState.Wander;

        enum ZombieState
        {
            Chasing,
            Caught,
            Wander
        }

        public Zombie(World world, Vector2 position, Texture2D spriteTexture) : base(world, position, spriteTexture, ZombieSpeed, ZombieRadius)
        {
            OffGroundHeight = ZombieFloatHeight;
            HitPoints = 4;
            SpriteTexture.AddAnimation("Walk", 0, 0, 64, 64, 8, 1/4f, false);
            SpriteTexture.SetAnimation("Walk");
            
        }

        public void UpdateZombie(Vector2 zombiePos)
        {
            playerPosition = Player.SimPosition;



            //First, Set Thresholds
            float chaseThreshold = chaseDistance;
            float caughtThreshold = caughtDistance;

            //Make him less likely to attack when idle
            if (guapoState == ZombieState.Wander)
            {
                chaseThreshold -= hysteresis / 2;
            }
            //More likely to be active when active
            else if (guapoState == ZombieState.Chasing)
            {
                chaseThreshold += hysteresis / 2;
                caughtThreshold -= hysteresis / 2;
            }
            //More likely to be caught when caught
            else if (guapoState == ZombieState.Caught)
            {
                caughtThreshold += hysteresis / 2;
            }

            //Second, decide state
            float distanceFromPlayer = Vector2.Distance(playerPosition, zombiePos);


            //Check for wander
            if (distanceFromPlayer > chaseThreshold)
            {
                guapoState = ZombieState.Wander;
            }
            //Check for chase
            else if (distanceFromPlayer > caughtThreshold)
            {
                guapoState = ZombieState.Chasing;
            }
            //Check for caught
            else
                guapoState = ZombieState.Caught;



            //Third, move
            float currentSpeed;

            if (guapoState == ZombieState.Chasing)
            {
                zombieOrientation = TurnToFace(zombiePos, playerPosition, zombieOrientation, turnSpeed);
                currentSpeed = maxSpeed;
            }
            else if (guapoState == ZombieState.Wander)
            {
                Wander(zombiePos, ref wanderDirection, ref zombieOrientation,
                   turnSpeed);

                currentSpeed = .05f * maxSpeed;
            }
            else
                currentSpeed = 0.0f;



            Vector2 heading = new Vector2((float)Math.Cos(zombieOrientation), (float)Math.Sin(zombieOrientation));
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
                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());
            wanderDirection.Y +=
                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());

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
            //SoundManager.Play("ZombieDeath");
        }

        public override void InflictDamage(int damage)
        {
            base.InflictDamage(damage);
            //SoundManager.Play("ZombieHurt");
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
            UpdateZombie(this.SimPosition);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteTexture.StepAnimation(gameTime);
            spriteBatch.Draw(Shadow, DisplayPosition, null, Color.White, 0f,
                ShadowOrigin, 0.8f, SpriteEffects.None, LayerDepth - 2 / 10000f);
            if (GameplayScreen.DebugCollision)
                spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                    CenterOrigin(DebugCircleTexture), 2 * Radius / 50f, SpriteEffects.None, LayerDepth - 1 / 10000f);
            spriteBatch.Draw(SpriteTexture.Sheet, SpritePosition(), SpriteTexture.CurrentFrame, Hurt ? new Color(1, 0, 0, 1f) : Color.White, 0f, SpriteOrigin, 1f, SpriteEffects.None, LayerDepth);
        }
    }
}
