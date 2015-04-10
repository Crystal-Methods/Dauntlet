using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Dauntlet.Entities
{
    public class Guapo : EnemyEntity
    {

        private const float ChaseSpeed       =  0.02f;
        private const float WanderSpeed      =  0.01f;
        private const float ChaseDistance    =  3f;
        private const float CaughtDistance   =  1f;
        private const float Hysteresis       =  0.5f;
        private const float TurnSpeed        =  0.2f;
        private const float GuapoRadius      = 14f;
        private const float GuapoFloatHeight = 15f;
        private const int MaxHp              = 3;

        // -----------------------------------------------------

        //AI Stuff
        private Vector2 _wanderDirection;
        private static readonly Random Random = new Random();
        GuapoState _guapoState = GuapoState.Wander;

        enum GuapoState
        {
            Chasing,
            Caught,
            Wander
        }

        // ---------------------------------

        public Guapo(World world, Vector2 position, Texture2D spriteTexture) : base(world, position, spriteTexture, ChaseSpeed, GuapoRadius)
        {
            OffGroundHeight = GuapoFloatHeight;
            HitPoints = MaxHp;

            SpriteTexture.AddAnimation("Fly", 0, 0, 32, 32, 5, 1/24f, false, false);
            SpriteTexture.SetAnimation("Fly");
        }

        // Get player position and other data like this:
        public void UpdateGuapo(GameTime gameTime)
        {
            //First, Set Thresholds
            float chaseThreshold = ChaseDistance;
            float caughtThreshold = CaughtDistance;

            switch (_guapoState) {
                 case GuapoState.Wander:
                    chaseThreshold -= Hysteresis/2;
                    break;
                case GuapoState.Chasing:
                    chaseThreshold += Hysteresis/2;
                    caughtThreshold -= Hysteresis/2;
                    break;
                case GuapoState.Caught:
                    caughtThreshold += Hysteresis/2;
                    break;
            }

            //Second, decide state
            float distanceFromPlayer = Vector2.Distance(Player.SimPosition, SimPosition);
            
            if (distanceFromPlayer > chaseThreshold) _guapoState = GuapoState.Wander;
            else if (distanceFromPlayer > caughtThreshold) _guapoState = GuapoState.Chasing;
            else _guapoState = GuapoState.Caught;
                
            //Third, move
            if (_guapoState == GuapoState.Chasing)
                Chase();
            else if (_guapoState == GuapoState.Wander)
                Wander();
        }

        private static float TurnToFace(Vector2 position, Vector2 faceThis, float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            var desiredAngle = (float)Math.Atan2(y, x);
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
                radians += MathHelper.TwoPi;
            while (radians > MathHelper.Pi)
                radians -= MathHelper.TwoPi;
            return radians;
        }

        public void Wander()
        {
            //Finds a random direction to go in. The .25 is how erratic the wander is.
            _wanderDirection.X += MathHelper.Lerp(-.25f, .25f, (float)Random.NextDouble());
            _wanderDirection.Y += MathHelper.Lerp(-.25f, .25f, (float)Random.NextDouble());
            _wanderDirection.Normalize();

            CollisionBody.Rotation = WrapAngle((float)Math.Atan2(_wanderDirection.Y, _wanderDirection.X));
            CollisionBody.ApplyLinearImpulse(_wanderDirection * WanderSpeed);
        }

        public void Chase()
        {
            CollisionBody.Rotation = TurnToFace(SimPosition, Player.SimPosition, CollisionBody.Rotation, TurnSpeed);
            var heading = new Vector2((float)Math.Cos(CollisionBody.Rotation), (float)Math.Sin(CollisionBody.Rotation));
            heading.Normalize();
            CollisionBody.ApplyLinearImpulse(heading * Speed);
        }

        public override void Die()
        {
            Dying = true;
            Dauntlet.SoundBank.PlayCue("GuapoDeath");
        }

        public override void InflictDamage(int damage)
        {
            if(!Dying) base.InflictDamage(damage);
            if(HitPoints > 0) Dauntlet.SoundBank.PlayCue("GuapoHurt");
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
                    Poof.SummonPoof(DisplayPosition);
                    CollisionBody.Dispose();
                }
            }

            UpdateGuapo(gameTime);
            base.Update(gameTime);
        }
    }
}
