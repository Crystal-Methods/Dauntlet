using System;
using System.Collections.Generic;
using Dauntlet.GameScreens;
using Dauntlet.TileEngine;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Dauntlet.Entities
{
    public class PlayerEntity : Entity
    {
        private const float PlayerSpeed       =  5f; // Max movement speed
        private const float PlayerRadius      = 15f; // Radius of the collision body, in pixels
        private const float PlayerFloatHeight = 14f; // Vertical offset between shadow and sprite (for "floating" effect), in pixels
        private const float PlayerMass        =  1f; // Mass of the body
        private const int   BaseHealth        =  5;  // Initial health

        // ---------------------------------

        private bool              _isTeleporting;   // True if the player must teleport next update
        private float             _punchTime;       // Elapsed time of current punch
        private Vector2           _punchVector;     // Direction in which a punch is being thrown
        private AnimatedTexture2D _gauntletTexture; // Texture for the gauntlet

        public bool  IsPunching = false; // True if the player is in the middle of a punch
        public int   Power      = 1;     // Base damage dealt from a basic attack
        public int   Exp        = 0;     // Player's experience
        public float SmoothExp  = 0;     // Used for filling the EXP bar on the HUD
        public int   Level      = 1;     // Player's level
        public Body  GauntletBody;       // Collision body for the gauntlet

        // How much EXP is needed to reach the next level
        public int ExpToNextLevel { get { return (int) ((4f/5f) * (Level + 1) * (Level + 1)); } }
        
        // --------------------------------

        /// <summary>
        /// Create a new Player entity
        /// </summary>
        /// <param name="world">the Farseer World in which to add this entity</param>
        /// <param name="position">initial position of the entity, in Sim units</param>
        /// <param name="playerTexture">texture for the player</param>
        /// <param name="gauntletTexture">texture for the gauntlet</param>
        public PlayerEntity(World world, Vector2 position, AnimatedTexture2D playerTexture, AnimatedTexture2D gauntletTexture)
        {
            Speed = PlayerSpeed;
            Mass = PlayerMass;
            Radius = PlayerRadius.Sim();
            OffGroundHeight = PlayerFloatHeight;
            IsBobbing = true;
            HitPoints = BaseHealth;
            SpriteTexture = playerTexture;
            _gauntletTexture = gauntletTexture;

            // Create player body
            CollisionBody = BodyFactory.CreateCircle(world, Radius, this.Density(), position);
            CollisionBody.InitBody(BodyType.Dynamic, Category.Cat2, Category.All, true, 0f, 0.5f, 35f, 100f);
            CollisionBody.OnCollision += OnPlayerCollision;

            // Create Gauntlet body
           GauntletBody = BodyFactory.CreateRectangle(world, 32f.Sim(), 24f.Sim(), this.Density(), position + new Vector2(15, 0));
            GauntletBody.InitBody(BodyType.Kinematic, Category.Cat1, Category.Cat10, true, 0f, 0.5f, 5f, 100f);
            GauntletBody.OnCollision += OnGauntletCollision;
            GauntletBody.OnSeparation += OnGauntletSeparation;
        }

        bool OnPlayerCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Body playerBody = fixtureA.Body;
            Body otherBody = fixtureB.Body;

            // Detects teleportation
            if (playerBody.GetType() == CollisionBody.GetType() && fixtureB.CollisionCategories == Category.Cat29 && !_isTeleporting)
            {
                Dir direction;
                Vector2 colNorm = contact.Manifold.LocalNormal;

                // Work out collision direction
                if (Math.Abs(colNorm.X) > Math.Abs(colNorm.Y)) direction = colNorm.X > 0 ? Dir.E : Dir.W;
                else direction = colNorm.Y > 0 ? Dir.S : Dir.N;

                TileEngine.TileEngine.HandleTeleport((char)((List<Object>)otherBody.UserData)[0], direction);
                _isTeleporting = true;
            }

            // Detects damage from enemy
            if (playerBody.GetType() == CollisionBody.GetType() && fixtureB.CollisionCategories == Category.Cat10 && !Hurt
                && !((EnemyEntity)otherBody.UserData).Dying)
            {
                Vector2 collisionNormal = Vector2.Normalize(Position - otherBody.Position);
                CollisionBody.LinearDamping = 10f;
                CollisionBody.ApplyLinearImpulse(collisionNormal * 20f);
                Hurt = true;
                HurtTimer = 0;
                InflictDamage(1);
                Dauntlet.SoundBank.PlayCue("Hurt");

                if (HitPoints == 0)
                    Dying = false;
            }

            // Detects collection of EXP orb
            if (playerBody.GetType() == CollisionBody.GetType() && fixtureB.CollisionCategories == Category.Cat24)
            {
                ((ExpOrb)otherBody.UserData).Die();
                Exp++;
            }

            return true;
        }

        bool OnGauntletCollision(Fixture a, Fixture b, Contact c)
        {
            // Detects collision with harmable enemy
            if (a.Body.GetType() == GauntletBody.GetType() && a.Body.FixtureList[0].CollisionCategories == Category.Cat3
                && b.CollisionCategories == Category.Cat10 && !((EnemyEntity)b.Body.UserData).Hurt)
            {
                var enemy = (EnemyEntity)b.Body.UserData;
                enemy.InflictDamage(1);
                enemy.Hurt = true;
            }

            return true;
        }

        void OnGauntletSeparation(Fixture a, Fixture b)
        {
            if (a.Body.GetType() == GauntletBody.GetType() && a.Body.FixtureList[0].CollisionCategories == Category.Cat3
                && b.CollisionCategories == Category.Cat10 && !((EnemyEntity)b.Body.UserData).Hurt)
            {
                var enemy = (EnemyEntity)b.Body.UserData;
                enemy.InflictDamage(1);
                enemy.Hurt = true;
            }
        }

        /// <summary>
        /// Move the player to a new room
        /// </summary>
        /// <param name="world">next room's Farseer World</param>
        /// <param name="newPos">player's future position in the new room, in sim units</param>
        public void ChangeRoom(World world, Vector2 newPos)
        {
            // Create player body
            Body newBody = BodyFactory.CreateCircle(world, Radius, this.Density(), newPos);
            newBody.InitBody(BodyType.Dynamic, Category.Cat2, Category.All, true, 0f, 0.5f, 35f, 100f);

            // Create Gauntlet body
            Body newGauntletBody = BodyFactory.CreateRectangle(world, 32f.Sim(), 24f.Sim(), this.Density(), newPos + new Vector2(15, 0));
            newGauntletBody.InitBody(BodyType.Kinematic, Category.Cat1, Category.Cat10, true, 0f, 0.5f, 5f, 100f);

            // Kill old bodies and set new ones
            CollisionBody.Dispose();
            CollisionBody = newBody;
            CollisionBody.OnCollision += OnPlayerCollision;

            GauntletBody.Dispose();
            GauntletBody = newGauntletBody;
            GauntletBody.OnCollision += OnGauntletCollision;
            GauntletBody.OnSeparation += OnGauntletSeparation;
        }

        /// <summary>
        /// Move the player
        /// </summary>
        /// <param name="keyState">current keyboard state</param>
        /// <param name="padState">current gamepad state</param>
        public void Move(KeyboardState keyState, GamePadState padState)
        {
            if (!Dead && !Dying)
            {
                if (Hurt && HurtTimer < 500) return;
                Vector2 force = Vector2.Zero;

                if (keyState.IsKeyDown(Keys.W)) force.Y--;
                if (keyState.IsKeyDown(Keys.A)) force.X--;
                if (keyState.IsKeyDown(Keys.S)) force.Y++;
                if (keyState.IsKeyDown(Keys.D)) force.X++;
                if (force != Vector2.Zero)
                {
                    force.Normalize();
                    CollisionBody.ApplyLinearImpulse(force * Speed);
                    CollisionBody.Rotation = (float)Math.Atan2(force.Y, force.X);
                }

                if (!padState.IsConnected) return;
                force = padState.ThumbSticks.Left;
                force.Y *= -1;
                CollisionBody.ApplyLinearImpulse(force * Speed);

                if (padState.ThumbSticks.Left.Length() > 0.2)
                    CollisionBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Left.Y, padState.ThumbSticks.Left.X);
            }
        }

        /// <summary>
        /// Throw a punch
        /// </summary>
        /// <param name="gameTime">the game's GameTime object</param>
        public void Punch(GameTime gameTime)
        {
            IsPunching = true;
            GauntletBody.CollisionCategories = Category.Cat3;
            _punchVector = Vector2.Normalize(new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)));
            _punchTime = 0;
        }

        /// <summary>
        /// Rotate the player
        /// </summary>
        /// <param name="padState">current gamepad state</param>
        public void Rotate(GamePadState padState)
        {
            if (padState.ThumbSticks.Right.Length() > 0.2)
                CollisionBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Right.Y, padState.ThumbSticks.Right.X);
        }

        /// <summary>
        /// Decide which direction to face the animation based on rotation
        /// </summary>
        public void ResolveAnimation()
        {
            const float e = (float)Math.PI / 8f;
            float r = CollisionBody.Rotation;

            if (r > 7 * e || r <= -7 * e) SpriteTexture.SetAnimation("LookLeft");
            else if (r > 5 * e) SpriteTexture.SetAnimation("LookDownLeft");
            else if (r > 3 * e) SpriteTexture.SetAnimation("LookDown");
            else if (r > e) SpriteTexture.SetAnimation("LookDownRight");
            else if (r > -e) SpriteTexture.SetAnimation("LookRight");
            else if (r > -3 * e) SpriteTexture.SetAnimation("LookUpRight");
            else if (r > -5 * e) SpriteTexture.SetAnimation("LookUp");
            else if (r > -7 * e) SpriteTexture.SetAnimation("LookUpLeft");
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            ResolveAnimation();
            _isTeleporting = false;

            if (Dying)
            {
                DeathTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (DeathTimer > 500)
                {
                    Poof.SummonPoof(DisplayPosition, OffGroundHeight);
                    CollisionBody.Dispose();
                }
                if (DeathTimer > 1000)
                {
                    Dead = true;
                    Dying = false;
                }   
            }
                
            

            if (IsPunching) {
                _punchTime += gameTime.ElapsedGameTime.Milliseconds;
                float x = _punchTime/64;
                var v = (float) (30f * Math.Exp(-Math.Pow(x, 2)) * (1f - 2f * Math.Pow(x, 2)));
                GauntletBody.LinearVelocity = _punchVector * v;

                if (v < 0) GauntletBody.CollisionCategories = Category.Cat1;

                if (_punchTime > 100) {
                    IsPunching = false;
                    GauntletBody.LinearVelocity = Vector2.Zero;
                    GauntletBody.Position = Position +
                        new Vector2(-(float)Math.Sin(Rotation) * Radius, (float)Math.Cos(Rotation) * Radius);
                    GauntletBody.Rotation = Rotation;
                }
            } else if(!Dead) {
                GauntletBody.Position = Position +
                    new Vector2(-(float)Math.Sin(Rotation) * Radius, (float)Math.Cos(Rotation) * Radius);
                GauntletBody.Rotation = Rotation;
                if(Dead)
                    GauntletBody.Dispose();
            }
            
            if (Hurt) {
                HurtTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (HurtTimer >= 500) CollisionBody.LinearDamping = 35;
                if (HurtTimer > 2000) Hurt = false;
            }

            if (SmoothExp < Exp) SmoothExp += 0.2f;

            if (SmoothExp >= ExpToNextLevel) {
                Exp = Exp % ExpToNextLevel;
                SmoothExp = 0;
                Level++;

                //Heal player on level up
                if (HitPoints < BaseHealth)
                {
                    Heal(1);
                }
                
                Dauntlet.SoundBank.PlayCue("LevelUp");
                HUD.LevelledUp();
            }
            
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteTexture.StepAnimation(gameTime);
            
            // Draw shadow
            spriteBatch.Draw(Shadow, DisplayPosition, null, Color.White, 0f,
                ShadowOrigin, 0.8f, SpriteEffects.None, LayerDepth - 3 / 10000f);

            // Draw debug
            if (GameplayScreen.DebugCollision)
            {
                spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, Rotation,
                    CenterOrigin(DebugCircleTexture), 2 * DisplayRadius / 50f, SpriteEffects.None, 1f);

                Texture2D rect = SpriteFactory.GetRectangleTexture(24, 32,
                    new Color(1, GauntletBody.FixtureList[0].CollisionCategories == Category.Cat3 ? 0.5f : 1,
                        GauntletBody.FixtureList[0].CollisionCategories == Category.Cat3 ? 0.5f : 0, 0.1f));
                spriteBatch.Draw(rect, GauntletBody.Position.Dis(), null, Color.White, GauntletBody.Rotation,
                    CenterOrigin(rect), 1f, SpriteEffects.None, 1f);
            }

            // Draw player
            float bobFactor = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4) * 3 + 1;
            spriteBatch.Draw(SpriteTexture.Sheet, IsBobbing ? SpritePosition(bobFactor) : SpritePosition(), SpriteTexture.CurrentFrame,
                Hurt ? new Color(1, 0, 0, 1f) : Color.White, 0f, SpriteOrigin, 1f,
                SpriteTexture.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, LayerDepth);

            // Draw gauntlet
            spriteBatch.Draw(_gauntletTexture.Sheet, new Vector2(GauntletBody.Position.X.Dis(), GauntletBody.Position.Y.Dis() - OffGroundHeight),
                _gauntletTexture.CurrentFrame, Color.White, GauntletBody.Rotation, CenterOrigin(_gauntletTexture.Sheet), 1f,
                SpriteEffects.FlipHorizontally, GetLayerDepth(GauntletBody) - 1 / 10000f);

        }

    }
}
