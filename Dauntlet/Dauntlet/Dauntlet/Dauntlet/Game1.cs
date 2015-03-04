using System;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Dauntlet
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private World _world;

        private Body _playerBody;

        private Vector2 _playerOrigin;
        private Vector2 _screenCenter;

        private Texture2D _playerSprite;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _world = new World(Vector2.Zero);
        }

        protected override void LoadContent()
        {
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _playerSprite = Content.Load<Texture2D>("Circle");
            _playerOrigin = new Vector2(_playerSprite.Width / 2f, _playerSprite.Height / 2f);

            ConvertUnits.SetDisplayUnitToSimUnitRatio(50f);

            Vector2 circlePosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(0, -1.5f);

            // Create the circle fixture
            _playerBody = BodyFactory.CreateCircle(_world, ConvertUnits.ToSimUnits(96 / 2f), 1f, circlePosition);
            _playerBody.BodyType = BodyType.Dynamic;

            // Give it some bounce and friction
            _playerBody.Restitution = 0.3f;
            _playerBody.Friction = 0.5f;
            _playerBody.LinearDamping = 50f;
        }

        protected override void Update(GameTime gameTime)
        {
            HandleKeyboard();

            HandleGamePad();

            //We update the world
            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);
        }

        private void HandleGamePad()
        {
            GamePadState padState = GamePad.GetState(0, GamePadDeadZone.Circular);

            if (padState.IsConnected)
            {
                if (padState.Buttons.Back == ButtonState.Pressed)
                    Exit();
                Vector2 force = padState.ThumbSticks.Right * 70;
                force.Y *= -1;
                _playerBody.ApplyLinearImpulse(force);

                if(padState.ThumbSticks.Right.Length() > 0.2)
                    _playerBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Right.Y, padState.ThumbSticks.Right.X);

                if (padState.ThumbSticks.Left.Length() > 0.2)
                    _playerBody.Rotation = -(float)Math.Atan2(padState.ThumbSticks.Left.Y, padState.ThumbSticks.Left.X);
            }
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.W))
                _playerBody.ApplyLinearImpulse(new Vector2(0, -10));
            if (state.IsKeyDown(Keys.A))
                _playerBody.ApplyLinearImpulse(new Vector2(-10, 0));
            if (state.IsKeyDown(Keys.S))
                _playerBody.ApplyLinearImpulse(new Vector2(0, 10));
            if (state.IsKeyDown(Keys.D))
                _playerBody.ApplyLinearImpulse(new Vector2(10, 0));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);
            _spriteBatch.Draw(_playerSprite, ConvertUnits.ToDisplayUnits(_playerBody.Position), null, Color.White, _playerBody.Rotation, _playerOrigin, 1f, SpriteEffects.None, 0f);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
