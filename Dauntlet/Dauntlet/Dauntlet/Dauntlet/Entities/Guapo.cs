using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;

namespace Dauntlet.Entities
{
    public class Guapo : EnemyEntity
    {
        private const float TopSpeed         =  0.035f; // Top speed of Guapo
        private const float WanderSpeed      =  0.01f; // Wandering speed of Guapo
        private const int   MaxHp            =  2;     // Max health of Guapo
        private const int   ExpValue         =  2;     // How much experience Guapo is worth when killed
        private const float ChaseDistance    =  7f;    // How far Guapo will look to chase a player, in sim units
        private const float CaughtDistance   =  1f;    // How close Guapo will get to the player before stopping, in sim units
        private const float Hysteresis       =  0.5f;  // Variance in Caught and Chase thresholds based on current state, in sim units
        private const float TurnSpeed        =  0.2f;  // How quickly Guapo can turn
        private const float GuapoRadius      = 14f;    // Radius of the collision body, in pixels
        private const float GuapoFloatHeight = 15f;    // Vertical offset between shadow and sprite (for "floating" effect), in pixels
        private bool attentionToPlayer = false; // Used for method to play sound once when attention is drawn to player

        // -----------------------------------------------------

        //AI Stuff
        private Vector2 _wanderDirection;
        private static readonly Random Random = new Random();
        GuapoState _guapoState = GuapoState.Wander;

        enum GuapoState
        {
            Chasing,
            Fleeing,
            Caught,
            Wander
        }

        // ---------------------------------

        /// <summary>
        /// Creates a new Guapo entity
        /// </summary>
        /// <param name="world">the Farseer World in which to add this entity</param>
        /// <param name="position">initial position of this entity, in sim units</param>
        /// <param name="spriteTexture">texture for Guapo</param>
        public Guapo(World world, Vector2 position, AnimatedTexture2D spriteTexture) : base(world, position, spriteTexture, TopSpeed, GuapoRadius)
        {
            OffGroundHeight = GuapoFloatHeight;
            HitPoints = MaxHp;
            ExpDrop = ExpValue;
        }

        public void playAttentionSound()
        {
            int randomInt = Random.Next(3);
            if (randomInt == 0)
            {
                Dauntlet.SoundBank.PlayCue("Guapo_1");
            }
            else if (randomInt == 1)
            {
                Dauntlet.SoundBank.PlayCue("Guapo_2");
            }
            else
            {
                Dauntlet.SoundBank.PlayCue("Guapo_3");
            }

            attentionToPlayer = true;
        }

        public override void InflictDamage(int damage)
        {
            if (!Dying) base.InflictDamage(damage);
            if (HitPoints > 0)
            {
            int randomInt = Random.Next(4);
            if (randomInt == 0)
            {
                Dauntlet.SoundBank.PlayCue("GuapoPunch_1");
            }
            else if(randomInt == 1)
            {
                Dauntlet.SoundBank.PlayCue("GuapoPunch_2");
            }
            else if (randomInt == 2)
            {
                Dauntlet.SoundBank.PlayCue("GuapoPunch_3");
            }
            else
            {
                Dauntlet.SoundBank.PlayCue("GuapoPunch_4");
            }
            }
        }

        protected override void UpdateAi(GameTime gameTime)
        {
            //First, Set Thresholds
            float chaseThreshold = ChaseDistance;
            float caughtThreshold = CaughtDistance;

            switch (_guapoState)
            {
                case GuapoState.Wander:
                    chaseThreshold -= Hysteresis / 2;
                    break;
                case GuapoState.Chasing:
                    chaseThreshold += Hysteresis / 2;
                    caughtThreshold -= Hysteresis / 2;
                    break;
                case GuapoState.Caught:
                    caughtThreshold += Hysteresis / 2;
                    break;
                case GuapoState.Fleeing:
                    caughtThreshold = 0;
                    chaseThreshold += Hysteresis / 2;
                    break;
            }

            //Second, decide state
            float distanceFromPlayer = Vector2.Distance(Player.Position, Position);

            if (distanceFromPlayer > chaseThreshold)
                _guapoState = GuapoState.Wander;
            else if (distanceFromPlayer > caughtThreshold && HitPoints == 1)
                _guapoState = GuapoState.Fleeing;
            else if (distanceFromPlayer > caughtThreshold && HitPoints > 1)
                _guapoState = GuapoState.Chasing;
            else
                _guapoState = GuapoState.Caught;

            //Third, move
            if (_guapoState == GuapoState.Chasing) Chase();
            else if (_guapoState == GuapoState.Fleeing)
            { 
                Evade();
                Speed = .02f;
            }
            else if (_guapoState == GuapoState.Wander) Wander();
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
            if (attentionToPlayer == false)
            {
                playAttentionSound();
            }
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
            Dauntlet.SoundBank.PlayCue("GuapoDeath_1");
        }

    }
}
