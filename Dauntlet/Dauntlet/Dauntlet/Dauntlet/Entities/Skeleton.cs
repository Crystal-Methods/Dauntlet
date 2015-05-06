using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;

namespace Dauntlet.Entities
{
    public class Skeleton : EnemyEntity
    {
        private const float TopSpeed         =  0.045f; // Top speed of Skeleton
        private const float WanderSpeed      =  0.01f; // Wandering speed of Skeleton
        private const int   MaxHp            =  4;     // Max health of Skeleton
        private const int   ExpValue         =  3;     // How much experience Skeleton is worth when killed
        private const float ChaseDistance    =  6f;    // How far Skeleton will look to chase a player, in sim units
        private const float CaughtDistance   =  .5f;    // How close Skeleton will get to the player before stopping, in sim units
        private const float Hysteresis       =  0.5f;  // Variance in Caught and Chase thresholds based on current state, in sim units
        private const float TurnSpeed        =  0.2f;  // How quickly Skeleton can turn
        private const float SkeletonRadius      = 14f;    // Radius of the collision body, in pixels
        private const float SkeletonFloatHeight = 15f;    // Vertical offset between shadow and sprite (for "floating" effect), in pixels

        // -----------------------------------------------------

        //AI Stuff
        private Vector2 _wanderDirection;
        private static readonly Random Random = new Random();
        SkeletonState _skeletonState = SkeletonState.Wander;

        enum SkeletonState
        {
            Chasing,
            Fleeing,
            Caught,
            Wander
        }

        // ---------------------------------

        /// <summary>
        /// Creates a new Skeleton entity
        /// </summary>
        /// <param name="world">the Farseer World in which to add this entity</param>
        /// <param name="position">initial position of this entity, in sim units</param>
        /// <param name="spriteTexture">texture for Skeleton</param>
        public Skeleton(World world, Vector2 position, AnimatedTexture2D spriteTexture) : base(world, position, spriteTexture, TopSpeed, SkeletonRadius)
        {
            OffGroundHeight = SkeletonFloatHeight;
            HitPoints = MaxHp;
            ExpDrop = ExpValue;
            CollisionBody.LinearDamping = 10f;
        }

        public override void InflictDamage(int damage)
        {
            if (!Dying) base.InflictDamage(damage);
            if (HitPoints > 0)
            {
                int randomInt = Random.Next(2);
                if (randomInt == 1)
                {
                    Dauntlet.SoundBank.PlayCue("SkeletonPunch_1");
                }
                else
                {
                    Dauntlet.SoundBank.PlayCue("SkeletonPunch_2");
                }
            }
        }
        protected override void UpdateAi(GameTime gameTime)
        {
            //First, Set Thresholds
            float chaseThreshold = ChaseDistance;
            float caughtThreshold = CaughtDistance;

            switch (_skeletonState)
            {
                case SkeletonState.Wander:
                    chaseThreshold -= Hysteresis / 2;
                    break;
                case SkeletonState.Chasing:
                    chaseThreshold += Hysteresis / 2;
                    caughtThreshold -= Hysteresis / 2;
                    break;
                case SkeletonState.Caught:
                    caughtThreshold += Hysteresis / 2;
                    break;
            }

            //Second, decide state
            float distanceFromPlayer = Vector2.Distance(Player.Position, Position);

            if (distanceFromPlayer > chaseThreshold)
                _skeletonState = SkeletonState.Wander;
            else if (distanceFromPlayer > caughtThreshold)
                _skeletonState = SkeletonState.Chasing;
            else
                _skeletonState = SkeletonState.Caught;

            //Third, move
            if (_skeletonState == SkeletonState.Chasing) Chase();
            else if (_skeletonState == SkeletonState.Wander) Wander();
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

        /// <summary>
        /// Use Evade AI
        /// </summary>
        private void Evade()
        {
            Vector2 seekPosition = 2 * Position - Player.Position;

            CollisionBody.Rotation = EntityHelpers.TurnToFace(Position, seekPosition, Rotation, TurnSpeed);

            var heading = new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation));
            heading.Normalize();
            CollisionBody.ApplyLinearImpulse(heading * Speed);
        }

        public override void Die()
        {
            Dying = true;
            Dauntlet.SoundBank.PlayCue("SkeletonDeath_1");
        }

    }
}
