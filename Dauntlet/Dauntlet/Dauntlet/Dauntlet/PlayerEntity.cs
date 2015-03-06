using System;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//used for testing
using System.Windows.Forms;

namespace Dauntlet
{
    public class PlayerEntity
    {
        private const float Speed = 30f; // Speed of the player; CHANGES DEPENDING ON RADIUS!!
        private const float Radius = 15f; // Radius of player's bounding circle

        // ---------------------------------

        private readonly Vector2 _playerOrigin;
        private readonly Texture2D _playerSprite;
        private Body _playerBody;
        private bool _isTeleporting;
        private KeyboardState _oldKeyboardState;

        public Vector2 SimPosition { get { return _playerBody.Position; } }
        public Vector2 DisplayPosition { get { return ConvertUnits.ToDisplayUnits(_playerBody.Position); } }
        public Body PlayerBody
        {
            get { return _playerBody; }
            set { _playerBody = value; }
        }

        // --------------------------------

        public PlayerEntity(World world, Vector2 roomCenter, Texture2D playerSprite)
        {
            _playerSprite = playerSprite;
            _playerOrigin = new Vector2(_playerSprite.Width / 2f, _playerSprite.Height / 2f);
            Vector2 circlePosition = ConvertUnits.ToSimUnits(roomCenter) + new Vector2(0, -1f);

            // Create player body
            _playerBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(Radius), 0.7f, circlePosition);
            _playerBody.BodyType = BodyType.Dynamic;
            _playerBody.FixedRotation = true;
            _playerBody.Restitution = 0.3f;
            _playerBody.Friction = 0.5f;
            _playerBody.LinearDamping = 50f;
            _playerBody.AngularDamping = 100f;

            _playerBody.OnCollision += _playerBody_OnCollision;
        }

        bool _playerBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            // Detects teleportation
            if (fixtureA.Body.GetType() == _playerBody.GetType() & fixtureB.CollisionCategories == Category.Cat10 && !_isTeleporting)
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
            newBody.Restitution = _playerBody.Restitution;
            newBody.Friction = _playerBody.Friction;
            newBody.LinearDamping = _playerBody.LinearDamping;
            newBody.AngularDamping = _playerBody.AngularDamping;
            newBody.Rotation = _playerBody.Rotation;

            //Sound Test
            SoundManager.PlaySong("NoCombat");
            

            // Kill old body and set new one
            _playerBody.Dispose();
            _playerBody = newBody;
            _playerBody.OnCollision += _playerBody_OnCollision;
        }

        public void Update(GameTime gameTime)
        {
            _isTeleporting = false;
            HandleKeyboard();
            HandleGamePad();
        }

        private void HandleGamePad()
        {
            GamePadState padState = GamePad.GetState(0, GamePadDeadZone.Circular);

            if (padState.IsConnected)
            {
                //if (padState.Buttons.Back == ButtonState.Pressed)
                //    Exit();
                Vector2 force = padState.ThumbSticks.Right;
                force.Y *= -1;
                _playerBody.ApplyLinearImpulse(force * Speed);

                if (padState.ThumbSticks.Right.Length() > 0.2)
                    _playerBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Right.Y, padState.ThumbSticks.Right.X);

                if (padState.ThumbSticks.Left.Length() > 0.2)
                    _playerBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Left.Y, padState.ThumbSticks.Left.X);
            }
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            Vector2 force = Vector2.Zero;
            //Keys are really long because i was testing something that conflicted with the word Keys
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3) && _oldKeyboardState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.F3))
                Game1.DebugCollision = !Game1.DebugCollision;
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                force += new Vector2(0, -1);
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                force += new Vector2(-1, 0);
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                force += new Vector2(0, 1);
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                force += new Vector2(1, 0);
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
                SoundManager.Play("Swish");

            if (force != Vector2.Zero)
                force.Normalize();
            _playerBody.ApplyLinearImpulse(force * Speed);

            _oldKeyboardState = state;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_playerSprite, ConvertUnits.ToDisplayUnits(_playerBody.Position), null, Color.White, _playerBody.Rotation, _playerOrigin, 2*Radius/50f, SpriteEffects.None, 0f);
        }

    }
}
