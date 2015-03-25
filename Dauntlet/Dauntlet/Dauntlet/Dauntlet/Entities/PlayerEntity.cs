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
        private const float speed = 15f;
        private const float radius = 15f; // Radius of player's bounding circle
        private const float defaultOffGroundHeight = 10f; // How far the base of the sprite is from the center of the shadow
        private const float mass = 1f;
        
        // ---------------------------------

        private bool _isTeleporting;

        // --------------------------------

        public PlayerEntity(World world, Vector2 roomCenter, Texture2D spriteTexture)
        {
            Speed = speed;
            Radius = radius;
            OffGroundHeight = defaultOffGroundHeight;
            IsBobbing = true;

            SpriteTexture = new AnimatedTexture2D(spriteTexture);
            SpriteTexture.AddAnimation("LookDown", 0, 0, 32, 32, 4, 1/8f, false);
            SpriteTexture.AddAnimation("LookDownRight", 0, 32, 32, 32, 4, 1 / 8f, false);
            SpriteTexture.AddAnimation("LookRight", 0, 64, 32, 32, 4, 1 / 8f, false);
            SpriteTexture.AddAnimation("LookUpRight", 0, 96, 32, 32, 4, 1 / 8f, false);
            SpriteTexture.AddAnimation("LookUp", 0, 128, 32, 32, 4, 1 / 8f, false);
            SpriteTexture.AddAnimation("LookDownLeft", 0, 32, 32, 32, 4, 1 / 8f, true);
            SpriteTexture.AddAnimation("LookLeft", 0, 64, 32, 32, 4, 1 / 8f, true);
            SpriteTexture.AddAnimation("LookUpLeft", 0, 96, 32, 32, 4, 1 / 8f, true);
            SpriteTexture.SetAnimation("LookRight");

            Vector2 circlePosition = ConvertUnits.ToSimUnits(roomCenter) + new Vector2(0, -1f);

            // Create player body
            float density = mass/(float)(Math.PI*Math.Pow(ConvertUnits.ToSimUnits(Radius), 2));
            CollisionBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), density, circlePosition);
            CollisionBody.BodyType = BodyType.Dynamic;
            CollisionBody.FixedRotation = true;
            CollisionBody.Restitution = 0;
            CollisionBody.Friction = 0.5f;
            CollisionBody.LinearDamping = 25f;
            CollisionBody.AngularDamping = 100f;
            float m = CollisionBody.Mass;

            CollisionBody.OnCollision += CollisionBodyOnCollision;
        }

        bool CollisionBodyOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // Detects teleportation
            if (fixtureA.Body.GetType() == CollisionBody.GetType() & fixtureB.CollisionCategories == Category.Cat10 && !_isTeleporting)
            {
                TileEngine.HandleTeleport(fixtureB.Body);
                _isTeleporting = true;
            }
            return true;
        }

        public void ChangeRoom(World world, Vector2 newPos)
        {
            // Create new body in the new room
            Body newBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), 0.7f, newPos);
            newBody.BodyType = BodyType.Dynamic;
            newBody.FixedRotation = true;
            newBody.Restitution = CollisionBody.Restitution;
            newBody.Friction = CollisionBody.Friction;
            newBody.LinearDamping = CollisionBody.LinearDamping;
            newBody.AngularDamping = CollisionBody.AngularDamping;
            newBody.Rotation = CollisionBody.Rotation;

            //Sound Test
            SoundManager.PlaySong(TileEngine.CurrentRoomName == "testroom1" ? "SkeletonSwing" : "NoCombat");


            // Kill old body and set new one
            CollisionBody.Dispose();
            CollisionBody = newBody;
            CollisionBody.OnCollision += CollisionBodyOnCollision;
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
                CollisionBody.ApplyLinearImpulse(force*Speed);
                CollisionBody.Rotation = (float)Math.Atan2(force.Y, force.X);
            }

            if (!padState.IsConnected) return;
            force = padState.ThumbSticks.Right;
            force.Y *= -1;
            CollisionBody.ApplyLinearImpulse(force * Speed);

            if (padState.ThumbSticks.Right.Length() > 0.2)
                CollisionBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Right.Y, padState.ThumbSticks.Right.X);
        }

        public void ResolveAnimation()
        {
            if (CollisionBody.Rotation > 7 * Math.PI / 8f || CollisionBody.Rotation <= -7 * Math.PI / 8f)
                SpriteTexture.SetAnimation("LookLeft");
            else if (CollisionBody.Rotation > 5 * Math.PI/8f)
                SpriteTexture.SetAnimation("LookDownLeft");
            else if (CollisionBody.Rotation > 3 * Math.PI / 8f)
                SpriteTexture.SetAnimation("LookDown");
            else if (CollisionBody.Rotation > Math.PI / 8f)
                SpriteTexture.SetAnimation("LookDownRight");
            else if (CollisionBody.Rotation > -Math.PI / 8f)
                SpriteTexture.SetAnimation("LookRight");
            else if (CollisionBody.Rotation > -3 * Math.PI / 8f)
                SpriteTexture.SetAnimation("LookUpRight");
            else if (CollisionBody.Rotation > -5 * Math.PI / 8f)
                SpriteTexture.SetAnimation("LookUp");
            else if (CollisionBody.Rotation > -7 * Math.PI / 8f)
                SpriteTexture.SetAnimation("LookUpLeft");
        }

        //private void HandleGamePad()
        //{
        //    GamePadState padState = GamePad.GetState(0, GamePadDeadZone.Circular);

        //    if (padState.IsConnected)
        //    {
        //        if (padState.Buttons.A == ButtonState.Pressed)
        //            SoundManager.Play("Swish");

        //        if (padState.ThumbSticks.Left.Length() > 0.2)
        //            CollisionBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Left.Y, padState.ThumbSticks.Left.X);
        //    }
        //}

        //private void HandleKeyboard()
        //{
        //    Vector2 force = Vector2.Zero;
        //    //Keys are really long because i was testing something that conflicted with the word Keys
        //    if (state.IsKeyDown(Keys.F3) && _oldKeyboardState.IsKeyUp(Keys.F3))
        //        GameplayScreen.DebugCollision = !GameplayScreen.DebugCollision;
            
        //    if (state.IsKeyDown(Keys.Space))
        //        SoundManager.Play("Swish");
        //}

        public void Update(GameTime gameTime)
        {
            ResolveAnimation();
            _isTeleporting = false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteTexture.StepAnimation(gameTime);
            spriteBatch.Draw(Shadow, DisplayPosition, null, Color.White, 0f,
                ShadowOrigin, 1f, SpriteEffects.None, LayerDepth - 2/10000f);
            if (GameplayScreen.DebugCollision)
                spriteBatch.Draw(DebugCircleTexture, DisplayPosition, null, Color.White, CollisionBody.Rotation,
                    CenterOrigin(DebugCircleTexture), 2*Radius/50f, SpriteEffects.None, LayerDepth - 1/10000f);
            float bobFactor = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 4)*3 + 1;
            spriteBatch.Draw(SpriteTexture.Sheet, IsBobbing ? SpritePosition(bobFactor) : SpritePosition(), SpriteTexture.CurrentFrame, Color.White, 0f,
                SpriteOrigin, 2f, SpriteTexture.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, LayerDepth);
        }

    }
}
