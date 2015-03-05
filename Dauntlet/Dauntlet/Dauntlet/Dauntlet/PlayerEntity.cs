using System;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dauntlet
{
    public class PlayerEntity
    {
        private Body _playerBody;

        private Vector2 _playerOrigin;
        private Texture2D _playerSprite;

        private KeyboardState _oldKeyboardState;

        public Body PlayerBody
        {
            get { return _playerBody; }
            set { _playerBody = value; }
        }

        public Vector2 SimPosition { get { return _playerBody.Position; } }
        public Vector2 DisplayPosition { get { return ConvertUnits.ToDisplayUnits(_playerBody.Position); } }

        public PlayerEntity(World world, Vector2 screenCenter, Texture2D playerSprite)
        {
            _playerSprite = playerSprite;
            _playerOrigin = new Vector2(_playerSprite.Width / 2f, _playerSprite.Height / 2f);
            Vector2 circlePosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, -1f);

            // Create the circle fixture
            _playerBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(50 / 2f), 0.5f, circlePosition);
            _playerBody.BodyType = BodyType.Dynamic;
            _playerBody.FixedRotation = true;

            // Give it some bounce and friction
            _playerBody.Restitution = 0.3f;
            _playerBody.Friction = 0.5f;
            _playerBody.LinearDamping = 50f;
            _playerBody.AngularDamping = 100f;
        }

        public void ChangeRoom(World world, bool isNs, float newPosValue)
        {
            Vector2 newPos = _playerBody.Position;
            if (isNs)
                newPos.Y = newPosValue;
            else
                newPos.X = newPosValue;
            // Create the circle fixture
            Body newBody = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(50 / 2f), 0.5f, newPos);
            newBody.BodyType = BodyType.Dynamic;
            newBody.FixedRotation = true;

            // Give it some bounce and friction
            newBody.Restitution = _playerBody.Restitution;
            newBody.Friction = _playerBody.Friction;
            newBody.LinearDamping = _playerBody.LinearDamping;
            newBody.AngularDamping = _playerBody.AngularDamping;
            newBody.Rotation = _playerBody.Rotation;

            _playerBody.Dispose();
            _playerBody = newBody;
        }

        public void Update(GameTime gameTime)
        {
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
                Vector2 force = padState.ThumbSticks.Right * 70;
                force.Y *= -1;
                _playerBody.ApplyLinearImpulse(force);

                if (padState.ThumbSticks.Right.Length() > 0.2)
                    _playerBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Right.Y, padState.ThumbSticks.Right.X);

                if (padState.ThumbSticks.Left.Length() > 0.2)
                    _playerBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Left.Y, padState.ThumbSticks.Left.X);
            }
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.F3) && _oldKeyboardState.IsKeyUp(Keys.F3))
                Game1.DebugCollision = !Game1.DebugCollision;
            if (state.IsKeyDown(Keys.W))
                _playerBody.ApplyLinearImpulse(new Vector2(0, -30));
            if (state.IsKeyDown(Keys.A))
                _playerBody.ApplyLinearImpulse(new Vector2(-30, 0));
            if (state.IsKeyDown(Keys.S))
                _playerBody.ApplyLinearImpulse(new Vector2(0, 30));
            if (state.IsKeyDown(Keys.D))
                _playerBody.ApplyLinearImpulse(new Vector2(30, 0));

            _oldKeyboardState = state;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_playerSprite, ConvertUnits.ToDisplayUnits(_playerBody.Position), null, Color.White, _playerBody.Rotation, _playerOrigin, 1f, SpriteEffects.None, 0f);
        }

        
    }
}
