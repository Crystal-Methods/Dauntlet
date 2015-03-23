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
        private const float speed = 30f; // Speed of the player; CHANGES DEPENDING ON RADIUS!!
        private const float radius = 15f; // Radius of player's bounding circle

        // ---------------------------------

        private bool _isTeleporting;
        private KeyboardState _oldKeyboardState;
        internal float Fps = 5 / 24f;
        internal float Timer = 0;

        // --------------------------------

        public PlayerEntity(World world, Vector2 roomCenter, Texture2D spriteTexture)
        {
            Speed = speed;
            Radius = radius;

            SpriteTexture = new AnimatedTexture2D(spriteTexture);
            SpriteTexture.AddAnimation("Animate", 0, 0, 16, 16, 2);
            SpriteTexture.SetAnimation("Animate");

            SpriteOrigin = new Vector2(SpriteTexture.Width / 2f, SpriteTexture.Height / 2f);
            Vector2 circlePosition = ConvertUnits.ToSimUnits(roomCenter) + new Vector2(0, -1f);

            // Create player body
            CollisionBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), 0.7f, circlePosition);
            CollisionBody.BodyType = BodyType.Dynamic;
            CollisionBody.FixedRotation = true;
            CollisionBody.Restitution = 0.3f;
            CollisionBody.Friction = 0.5f;
            CollisionBody.LinearDamping = 50f;
            CollisionBody.AngularDamping = 100f;

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

        public void Update(GameTime gameTime)
        {
            _isTeleporting = false;
            HandleKeyboard();
            HandleGamePad();

            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (!(Timer > Fps * 1000)) return;
            SpriteTexture.Frame++;
            Timer = 0;
        }

        private void HandleGamePad()
        {
            GamePadState padState = GamePad.GetState(0, GamePadDeadZone.Circular);

            if (padState.IsConnected)
            {
                if (padState.Buttons.A == ButtonState.Pressed)
                    SoundManager.Play("Swish");

                Vector2 force = padState.ThumbSticks.Right;
                force.Y *= -1;
                CollisionBody.ApplyLinearImpulse(force * Speed);

                if (padState.ThumbSticks.Right.Length() > 0.2)
                    CollisionBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Right.Y, padState.ThumbSticks.Right.X);

                if (padState.ThumbSticks.Left.Length() > 0.2)
                    CollisionBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Left.Y, padState.ThumbSticks.Left.X);
            }
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            Vector2 force = Vector2.Zero;
            //Keys are really long because i was testing something that conflicted with the word Keys
            if (state.IsKeyDown(Keys.F3) && _oldKeyboardState.IsKeyUp(Keys.F3))
                GameplayScreen.DebugCollision = !GameplayScreen.DebugCollision;
            if (state.IsKeyDown(Keys.W))
                force += new Vector2(0, -1);
            if (state.IsKeyDown(Keys.A))
                force += new Vector2(-1, 0);
            if (state.IsKeyDown(Keys.S))
                force += new Vector2(0, 1);
            if (state.IsKeyDown(Keys.D))
                force += new Vector2(1, 0);
            if (state.IsKeyDown(Keys.Space))
                SoundManager.Play("Swish");

            if (force != Vector2.Zero)
                force.Normalize();
            CollisionBody.ApplyLinearImpulse(force * Speed);

            _oldKeyboardState = state;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SpriteTexture.Sheet, ConvertUnits.ToDisplayUnits(CollisionBody.Position), SpriteTexture.CurrentFrame, Color.White, 0f, SpriteOrigin, 2f, SpriteEffects.None, 0f);
        }

    }
}
