//used for testing
using System;
using Dauntlet.GameScreens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dauntlet.Entities
{
    public class PlayerEntity : Entity
    {
        private const float PlayerSpeed = 5f;
        private const float PlayerRadius = 15f; // Radius of player's bounding circle
        private const float PlayerFloatHeight = 14f; // How far the base of the sprite is from the center of the shadow
        private const float PlayerMass = 1f;
        private const int BaseHealth = 5;

        // ---------------------------------

        private bool _isTeleporting;
        private float _punchTime;
        private Vector2 _punchVector;
        private AnimatedTexture2D _gauntletTexture;
        public bool IsPunching;
        public int Power;
        public int Exp;
        public int Level;
        public int ExpToNextLevel { get { return (int) ((4f/5f) * (Level + 1) * (Level + 1)); } }

        public Body GauntletBody;

        // --------------------------------

        public PlayerEntity(World world, Vector2 position, Texture2D playerTexture, Texture2D gauntletTexture)
        {
            Speed = PlayerSpeed;
            Radius = PlayerRadius;
            OffGroundHeight = PlayerFloatHeight;
            IsBobbing = true;
            HitPoints = BaseHealth;
            Level = 1;
            Exp = 0;

            SpriteTexture = new AnimatedTexture2D(playerTexture);
            SpriteTexture.AddAnimation("LookDown", 0, 0, 23, 33, 6, 1 / 12f, false, false);
            SpriteTexture.AddAnimation("LookDownLeft", 0, 33, 23, 33, 6, 1 / 12f, false, false);
            SpriteTexture.AddAnimation("LookLeft", 0, 66, 23, 33, 6, 1 / 12f, false, false);
            SpriteTexture.AddAnimation("LookUpLeft", 0, 99, 23, 33, 6, 1 / 12f, false, false);
            SpriteTexture.AddAnimation("LookUp", 0, 132, 23, 33, 6, 1 / 12f, false, false);
            SpriteTexture.AddAnimation("LookDownRight", 0, 33, 23, 33, 6, 1 / 12f, true, false);
            SpriteTexture.AddAnimation("LookRight", 0, 66, 23, 33, 6, 1 / 12f, true, false);
            SpriteTexture.AddAnimation("LookUpRight", 0, 99, 23, 33, 6, 1 / 12f, true, false);
            SpriteTexture.SetAnimation("LookRight");

            _gauntletTexture = new AnimatedTexture2D(gauntletTexture);

            Vector2 circlePosition = ConvertUnits.ToSimUnits(position) + new Vector2(0, -1f);

            // Create player body
            float density = PlayerMass / (float)(Math.PI * Math.Pow(ConvertUnits.ToSimUnits(Radius), 2));
            CollisionBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), density, circlePosition);
            CollisionBody.BodyType = BodyType.Dynamic;
            CollisionBody.CollisionCategories = Category.Cat2;
            CollisionBody.FixedRotation = true;
            CollisionBody.Restitution = 0;
            CollisionBody.Friction = 0.5f;
            CollisionBody.LinearDamping = 35f;
            CollisionBody.AngularDamping = 100f;
            CollisionBody.OnCollision += CollisionBodyOnCollision;

            // Create Gauntlet body
            float gauntletdensity = PlayerMass / (float)(Math.PI * Math.Pow(ConvertUnits.ToSimUnits(Radius), 2));
            GauntletBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(32), ConvertUnits.ToSimUnits(24), gauntletdensity,
                circlePosition + new Vector2(15, 0));
            GauntletBody.BodyType = BodyType.Kinematic;
            GauntletBody.CollidesWith = Category.Cat10;
            GauntletBody.FixedRotation = true;
            GauntletBody.LinearDamping = 5f;
            GauntletBody.OnCollision += OnGauntletCollision;
        }

        bool CollisionBodyOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // Detects teleportation
            if (fixtureA.Body.GetType() == CollisionBody.GetType() && fixtureB.CollisionCategories == Category.Cat29 && !_isTeleporting)
            {
                // Work out collision direction
                char direction;
                Vector2 colNorm = contact.Manifold.LocalNormal;
                if (Math.Abs(colNorm.X) > Math.Abs(colNorm.Y))
                {
                    // X direction is dominant
                    direction = colNorm.X > 0 ? 'E' : 'W';
                }
                else
                {
                    // Y direction is dominant
                    direction = colNorm.Y > 0 ? 'S' : 'N';
                }
                TileEngine.HandleTeleport(fixtureB.Body, direction);
                _isTeleporting = true;
            }
            if (fixtureA.Body.GetType() == CollisionBody.GetType() && fixtureB.CollisionCategories == Category.Cat10 && !Hurt)
            {
                Hurt = true;
                HurtTimer = 0;
                InflictDamage(1);
                Dauntlet.SoundBank.PlayCue("Hurt");
            }
            if (fixtureA.Body.GetType() == CollisionBody.GetType() && fixtureB.CollisionCategories == Category.Cat24)
            {
                ((ExpOrb)fixtureB.Body.UserData).Die();
                Exp++;
            }
            return true;
        }

        bool OnGauntletCollision(Fixture a, Fixture b, Contact c)
        {
            if (a.Body.GetType() == GauntletBody.GetType() && a.Body.FixtureList[0].CollisionCategories == Category.Cat3
                && b.CollisionCategories == Category.Cat10)
            {
                var enemy = (EnemyEntity)b.Body.UserData;
                enemy.InflictDamage(1);
                enemy.Hurt = true;
            }
            return true;
        }

        public void ChangeRoom(World world, Vector2 newPos)
        {
            // Create player body
            float density = PlayerMass / (float)(Math.PI * Math.Pow(ConvertUnits.ToSimUnits(Radius), 2));
            Body newBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), density, newPos);
            newBody.BodyType = BodyType.Dynamic;
            newBody.CollisionCategories = Category.Cat2;
            newBody.FixedRotation = true;
            newBody.Restitution = CollisionBody.Restitution;
            newBody.Friction = CollisionBody.Friction;
            newBody.LinearDamping = CollisionBody.LinearDamping;
            newBody.AngularDamping = CollisionBody.AngularDamping;

            // Create Gauntlet body
            float gauntletdensity = PlayerMass / (float)(Math.PI * Math.Pow(ConvertUnits.ToSimUnits(Radius), 2));
            Body newGauntletBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(32), ConvertUnits.ToSimUnits(24), gauntletdensity,
                newPos + new Vector2(15, 0));
            newGauntletBody.BodyType = BodyType.Kinematic;
            newGauntletBody.CollidesWith = Category.Cat10;
            newGauntletBody.FixedRotation = true;
            newGauntletBody.LinearDamping = GauntletBody.LinearDamping;

            // Kill old body and set new one
            CollisionBody.Dispose();
            CollisionBody = newBody;
            CollisionBody.OnCollision += CollisionBodyOnCollision;
            GauntletBody.Dispose();
            GauntletBody = newGauntletBody;
            GauntletBody.OnCollision += OnGauntletCollision;
        }

        public void Move(KeyboardState keyState, GamePadState padState)
        {
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

        public void Punch(GameTime gameTime)
        {
            IsPunching = true;
            GauntletBody.CollisionCategories = Category.Cat3;
            _punchVector = Vector2.Normalize(new Vector2((float)Math.Cos(CollisionBody.Rotation),
                (float)Math.Sin(CollisionBody.Rotation)));
            _punchTime = 0;
        }

        public void Rotate(GamePadState padState)
        {
            if (padState.ThumbSticks.Right.Length() > 0.2)
                CollisionBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Right.Y, padState.ThumbSticks.Right.X);
        }

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
            if (IsPunching) {
                _punchTime += gameTime.ElapsedGameTime.Milliseconds;
                float x = _punchTime/64;
                var v = (float) (30f * Math.Exp(-Math.Pow(x, 2)) * (1f - 2f * Math.Pow(x, 2)));
                if (v < 0) GauntletBody.CollisionCategories = Category.Cat1;
                GauntletBody.LinearVelocity = _punchVector*v;
                if (_punchTime > 100)
                {
                    IsPunching = false;
                    GauntletBody.LinearVelocity = Vector2.Zero;
                    GauntletBody.Position = CollisionBody.Position +
                                        ConvertUnits.ToSimUnits(new Vector2(
                                            -(float)Math.Sin(CollisionBody.Rotation) * 15,
                                            (float)Math.Cos(CollisionBody.Rotation) * 15));
                    GauntletBody.Rotation = CollisionBody.Rotation;
                }
            }
            else
            {
                GauntletBody.Position = CollisionBody.Position +
                                        ConvertUnits.ToSimUnits(new Vector2(
                                            -(float)Math.Sin(CollisionBody.Rotation) * 15,
                                            (float)Math.Cos(CollisionBody.Rotation) * 15));
                GauntletBody.Rotation = CollisionBody.Rotation;
            }
            
            ResolveAnimation();
            _isTeleporting = false;

            //if (HitPoints <= 0 && !Dying && !Dead)
                //Die();
            if (Hurt)
            {
                HurtTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (HurtTimer > 2000)
                    Hurt = false;
            }
            if (Exp >= ExpToNextLevel)
            {
                Exp = Exp%ExpToNextLevel;
                Level++;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteTexture.StepAnimation(gameTime);
            spriteBatch.Draw(Shadow, DisplayPosition, null, Color.White, 0f,
                ShadowOrigin, 0.8f, SpriteEffects.None, LayerDepth - 3 / 10000f);

            if (GameplayScreen.DebugCollision)
            {
                spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                    CenterOrigin(DebugCircleTexture), 2 * Radius / 50f, SpriteEffects.None, LayerDepth - 2 / 10000f);

                Texture2D rect = SpriteFactory.GetRectangleTexture(24, 32,
                    new Color(1, GauntletBody.FixtureList[0].CollisionCategories == Category.Cat3 ? 0.5f : 1,
                        GauntletBody.FixtureList[0].CollisionCategories == Category.Cat3 ? 0.5f : 0, 0.1f));
                spriteBatch.Draw(rect, ConvertUnits.ToDisplayUnits(GauntletBody.Position), null, Color.White, GauntletBody.Rotation,
                    CenterOrigin(rect), 1f, SpriteEffects.None, 0f);
            }

            float bobFactor = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4) * 3 + 1;

            spriteBatch.Draw(SpriteTexture.Sheet, IsBobbing ? SpritePosition(bobFactor) : SpritePosition(), SpriteTexture.CurrentFrame,
                Hurt ? new Color(1, 0, 0, 1f) : Color.White, 0f,
                SpriteOrigin, 1f, SpriteTexture.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, LayerDepth);

            spriteBatch.Draw(_gauntletTexture.Sheet, new Vector2(ConvertUnits.ToDisplayUnits(GauntletBody.Position.X),
                ConvertUnits.ToDisplayUnits(GauntletBody.Position.Y) - OffGroundHeight), _gauntletTexture.CurrentFrame,
                Color.White, GauntletBody.Rotation, CenterOrigin(_gauntletTexture.Sheet), 1f, SpriteEffects.FlipHorizontally, GetLayerDepth(GauntletBody) - 1 / 10000f);
        }

    }
}
