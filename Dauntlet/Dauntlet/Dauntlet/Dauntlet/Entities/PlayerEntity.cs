//used for testing
using System;
using Dauntlet.GameScreens;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dauntlet.Entities
{
    public class PlayerEntity : Entity
    {
        private const float PlayerSpeed = 15f;
        private const float PlayerRadius = 15f; // Radius of player's bounding circle
        private const float PlayerFloatHeight = 14f; // How far the base of the sprite is from the center of the shadow
        private const float PlayerMass = 1f;

        // ---------------------------------

        private bool _isTeleporting;
        private float _punchTime;
        private AnimatedTexture2D _gauntletTexture;
        public bool IsPunching;
        public int Health;
        public int Power;

        public Body GauntletBody;

        // --------------------------------

        public PlayerEntity(World world, Vector2 position, Texture2D playerTexture, Texture2D gauntletTexture)
        {
            Speed = PlayerSpeed;
            Radius = PlayerRadius;
            OffGroundHeight = PlayerFloatHeight;
            IsBobbing = true;

            SpriteTexture = new AnimatedTexture2D(playerTexture);
            SpriteTexture.AddAnimation("LookDown", 0, 0, 23, 33, 6, 1 / 12f, false);
            SpriteTexture.AddAnimation("LookDownLeft", 0, 33, 23, 33, 6, 1 / 12f, false);
            SpriteTexture.AddAnimation("LookLeft", 0, 66, 23, 33, 6, 1 / 12f, false);
            SpriteTexture.AddAnimation("LookUpLeft", 0, 99, 23, 33, 6, 1 / 12f, false);
            SpriteTexture.AddAnimation("LookUp", 0, 132, 23, 33, 6, 1 / 12f, false);
            SpriteTexture.AddAnimation("LookDownRight", 0, 33, 23, 33, 6, 1 / 12f, true);
            SpriteTexture.AddAnimation("LookRight", 0, 66, 23, 33, 6, 1 / 12f, true);
            SpriteTexture.AddAnimation("LookUpRight", 0, 99, 23, 33, 6, 1 / 12f, true);
            SpriteTexture.SetAnimation("LookRight");

            _gauntletTexture = new AnimatedTexture2D(gauntletTexture);

            Vector2 circlePosition = ConvertUnits.ToSimUnits(position) + new Vector2(0, -1f);

            // Create player body
            float density = PlayerMass / (float)(Math.PI * Math.Pow(ConvertUnits.ToSimUnits(Radius), 2));
            CollisionBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), density, circlePosition);
            CollisionBody.BodyType = BodyType.Dynamic;
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
            GauntletBody.CollidesWith = Category.None;
            GauntletBody.FixedRotation = true;
            GauntletBody.LinearDamping = 5f;
            GauntletBody.OnCollision += OnGauntletCollision;
        }

        bool CollisionBodyOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // Detects teleportation
            if (fixtureA.Body.GetType() == CollisionBody.GetType() & fixtureB.CollisionCategories == Category.Cat10 && !_isTeleporting)
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
            return true;
        }

        bool OnGauntletCollision(Fixture a, Fixture b, Contact c)
        {
            if (a.Body.GetType() == GauntletBody.GetType() & b.CollisionCategories == Category.Cat5 && IsPunching)
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
            newBody.FixedRotation = true;
            newBody.Restitution = CollisionBody.Restitution;
            newBody.Friction = CollisionBody.Friction;
            newBody.LinearDamping = CollisionBody.LinearDamping;
            newBody.AngularDamping = CollisionBody.AngularDamping;

            // Create Gauntlet body
            float gauntletdensity = PlayerMass / (float)(Math.PI * Math.Pow(ConvertUnits.ToSimUnits(Radius), 2));
            Body newGauntletBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(32), ConvertUnits.ToSimUnits(24), gauntletdensity,
                newPos + new Vector2(15, 0));
            newGauntletBody.BodyType = BodyType.Dynamic;
            newGauntletBody.CollidesWith = Category.None;
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
            GauntletBody.BodyType = BodyType.Dynamic;
            GauntletBody.Mass = PlayerMass;
            GauntletBody.FixtureList[0].CollidesWith = Category.Cat5;
            GauntletBody.ApplyLinearImpulse(new Vector2((float)Math.Cos(CollisionBody.Rotation), (float)Math.Sin(CollisionBody.Rotation)) * 20);
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
            if (!IsPunching)
            {
                GauntletBody.Position = CollisionBody.Position +
                                        ConvertUnits.ToSimUnits(new Vector2(
                                            -(float)Math.Sin(CollisionBody.Rotation) * 15,
                                            (float)Math.Cos(CollisionBody.Rotation) * 15));
                GauntletBody.Rotation = CollisionBody.Rotation;
            }
            if (IsPunching)
            {
                _punchTime += gameTime.ElapsedGameTime.Milliseconds;
                GauntletBody.LinearDamping = (_punchTime);
                if (GauntletBody.LinearVelocity == Vector2.Zero)
                {
                    GauntletBody.BodyType = BodyType.Kinematic;
                    GauntletBody.LinearDamping = 0;
                    IsPunching = false;
                }
            }
            ResolveAnimation();
            _isTeleporting = false;
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

                Texture2D rect = SpriteFactory.GetRectangleTexture(24, 32, new Color(1, IsPunching ? 0.5f : 1, IsPunching ? 0.5f : 0, 0.1f));
                spriteBatch.Draw(rect, ConvertUnits.ToDisplayUnits(GauntletBody.Position), null, Color.White, GauntletBody.Rotation,
                    CenterOrigin(rect), 1f, SpriteEffects.None, 0f);
            }

            float bobFactor = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4) * 3 + 1;

            spriteBatch.Draw(SpriteTexture.Sheet, IsBobbing ? SpritePosition(bobFactor) : SpritePosition(), SpriteTexture.CurrentFrame, Color.White, 0f,
                SpriteOrigin, 1f, SpriteTexture.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, LayerDepth);

            spriteBatch.Draw(_gauntletTexture.Sheet, new Vector2(ConvertUnits.ToDisplayUnits(GauntletBody.Position.X),
                ConvertUnits.ToDisplayUnits(GauntletBody.Position.Y) - OffGroundHeight), _gauntletTexture.CurrentFrame,
                Color.White, GauntletBody.Rotation, CenterOrigin(_gauntletTexture.Sheet), 1f, SpriteEffects.FlipHorizontally, GetLayerDepth(GauntletBody) - 1 / 10000f);
        }

    }
}
