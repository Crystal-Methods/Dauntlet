using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;

namespace Dauntlet.Entities
{
    public class Zombie : EnemyEntity
    {
        private static float TopSpeed          =  0.05f; // Top speed of Zombie
        private const float WanderSpeed       =  0.01f; // Wandering speed of Zombie
        private const int   MaxHp             =  5;     // Max health of Zombie
        private const int   ExpValue          =  5;     // How much experience Zombie is worth when killed
        private const float ChaseDistance     =  5f;    // How far Zombie will look to chase a player, in sim units
        private const float CaughtDistance    =  .75f;    // How close Zombie will get to the player before stopping, in sim units
        private const float Hysteresis        =  0.5f;  // Variance in Caught and Chase thresholds based on current state, in sim units
        private const float TurnSpeed         =  0.2f;  // How quickly Zombie can turn
        private const float ZombieRadius      = 14f;    // Radius of the collision body, in pixels
        private const float ZombieFloatHeight =  0f;    // Vertical offset between shadow and sprite (for "floating" effect), in pixels

        // -----------------------------------------------------

        //AI Stuff
        private Vector2 _wanderDirection;
        private static readonly Random Random = new Random();
        ZombieState _zombieState = ZombieState.Wander;

        enum ZombieState
        {
            Chasing,
            Caught,
            Wander
        }

        public Zombie(World world, Vector2 position, AnimatedTexture2D spriteTexture) : base(world, position, spriteTexture, TopSpeed, ZombieRadius)
        {
            OffGroundHeight = ZombieFloatHeight;
            HitPoints = MaxHp;
            ExpDrop = ExpValue;
            CollisionBody.LinearDamping = 15f;
        }

        public override void InflictDamage(int damage)
        {
            if (!Dying) base.InflictDamage(damage);
            this.Speed += 0.01f;
            if (HitPoints > 0) Dauntlet.SoundBank.PlayCue("ZombieHurt_1");
        }

        protected override void UpdateAi(GameTime gameTime)
        {
            //First, Set Thresholds
            float chaseThreshold = ChaseDistance;
            float caughtThreshold = CaughtDistance;

            switch (_zombieState)
            {
                case ZombieState.Wander:
                    chaseThreshold -= Hysteresis / 2;
                    break;
                case ZombieState.Chasing:
                    chaseThreshold += Hysteresis / 2;
                    caughtThreshold -= Hysteresis / 2;
                    break;
                case ZombieState.Caught:
                    caughtThreshold += Hysteresis / 2;
                    break;
            }

            //Second, decide state
            float distanceFromPlayer = Vector2.Distance(Player.Position, Position);

            if (distanceFromPlayer > chaseThreshold)
                _zombieState = ZombieState.Wander;
            else if (distanceFromPlayer > caughtThreshold)
                _zombieState = ZombieState.Chasing;
            else
                _zombieState = ZombieState.Caught;

            //Third, move
            if (_zombieState == ZombieState.Chasing) Chase();
            else if (_zombieState == ZombieState.Wander) Wander();
        }

        /// <summary>
        /// Use Wander AI
        /// </summary>
        private void Wander()
        {
            //Finds a random direction to go in. The .25 is how erratic the wander is.
            _wanderDirection.X += MathHelper.Lerp(-.25f, .25f, (float)Random.NextDouble());
            _wanderDirection.Y += MathHelper.Lerp(-.25f, .25f, (float)Random.NextDouble());
            _wanderDirection.Normalize();

            CollisionBody.Rotation = EntityHelpers.WrapAngle((float)Math.Atan2(_wanderDirection.Y, _wanderDirection.X));
            CollisionBody.ApplyLinearImpulse(_wanderDirection * WanderSpeed);
        }

        /// <summary>
        /// Use Chase AI
        /// </summary>
        private void Chase()
        {
            CollisionBody.Rotation = EntityHelpers.TurnToFace(Position, Player.Position, Rotation, TurnSpeed);
            var heading = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            heading.Normalize();
            CollisionBody.ApplyLinearImpulse(heading * Speed);
        }

        public override void Die()
        {
            Dying = true;
            Dauntlet.SoundBank.PlayCue("ZombieDeath_1");
        }

    }
}
