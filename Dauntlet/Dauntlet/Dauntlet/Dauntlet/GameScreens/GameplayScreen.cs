using System;
using System.Linq;
using Dauntlet.Entities;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.GameScreens
{
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteBatch _spriteBatch;
        private static Matrix _view;

        private float _stepTime;

        // ===================================

        public static PlayerEntity Player;
        public Cue BgMusic;
        public World World { get; set; }
        public static bool DebugCollision { get; set; }
        public Vector2 DisplayRoomCenter { get { return new Vector2(TileEngine.CurrentRoom.PixelWidth/2f, TileEngine.CurrentRoom.PixelHeight/2f);} }
        public Vector2 SimRoomCenter { get { return ConvertUnits.ToSimUnits(DisplayRoomCenter); } }
        public override Screen ScreenType { get { return Screen.GameplayScreen;} }

        // ==================================

        public GameplayScreen(Dauntlet game) : base(game) { }

        public override void LoadContent()
        {
            // Lazy load things
            if (_content == null) _content = new ContentManager(MainGame.Services, "Content");
            if (_spriteBatch == null) _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize things
            CameraManager.Init(GraphicsDevice);
            SpriteFactory.Init(_content, GraphicsDevice);
            TileEngine.LoadContent(this, _content);
            HUD.Init();
            ConvertUnits.SetDisplayUnitToSimUnitRatio(TileEngine.TileSize); // 1 meter = 1 tile

            World = TileEngine.CurrentRoom.World;
            Player = SpriteFactory.CreatePlayer(World, DisplayRoomCenter + new Vector2(40, -40));

            BgMusic = Dauntlet.SoundBank.GetCue("DauntletNoCombat");
            BgMusic.Play();

            IsScreenLoaded = true;
        }

        public override void UnloadContent()
        {
            IsScreenLoaded = false;
            _content.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            _stepTime = gameTime.ElapsedGameTime.Milliseconds;
            // Update the world
            World.Step(_stepTime/1000f);
            Player.Update(gameTime);
            foreach (var entity in TileEngine.CurrentRoom.Entities.Where(entity => !entity.Dead))
                entity.Update(gameTime);
            TileEngine.CurrentRoom.Entities.AddRange(TileEngine.CurrentRoom.AddQueue);
            TileEngine.CurrentRoom.AddQueue.Clear();
            foreach (Entity e in TileEngine.CurrentRoom.RemoveQueue) TileEngine.CurrentRoom.Entities.Remove(e);
            TileEngine.CurrentRoom.RemoveQueue.Clear();
            
            // Update camera
            _view = CameraManager.MoveCamera(Player.DisplayPosition);
            
            // Update HUD
            HUD.Update(gameTime);

            // Handle input
            if (MainGame.Input.IsMovement() && !Player.IsPunching)
                Player.Move(MainGame.Input.CurrentKeyboardState, MainGame.Input.CurrentGamePadState);
            if (MainGame.Input.IsRotate() && !Player.IsPunching)
                Player.Rotate(MainGame.Input.CurrentGamePadState);
            if (MainGame.Input.IsPauseGame())
                ((MenuScreen)MainGame.GetScreen(Screen.PauseScreen)).OverlayScreen(this);
            if (MainGame.Input.IsToggleDebug())
                DebugCollision = !DebugCollision;
            if (MainGame.Input.IsAttack() && !Player.IsPunching)
            {
                Player.Punch(gameTime);
                Dauntlet.SoundBank.PlayCue("Swish_1");
                //SoundManager.Play("Swish");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0, 141, 158));

            // First pass: Draw room
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, _view);
            TileEngine.DrawRoom(_spriteBatch, gameTime);
            _spriteBatch.End();

            // Second pass: Draw debug highlights (optional)
            if (DebugCollision)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _view);
                TileEngine.DrawDebug(_spriteBatch, GraphicsDevice);
                _spriteBatch.End();
            }

            // Third pass: Draw entities
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, _view);
            Player.Draw(gameTime, _spriteBatch);
            foreach (var entity in TileEngine.CurrentRoom.Entities.Where(entity => !entity.Dead))
                entity.Draw(gameTime, _spriteBatch);
            TileEngine.DrawWallCaps(_spriteBatch);
            _spriteBatch.End();

            // Fourth pass: HUD
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            if (DebugCollision)
            {
                _spriteBatch.DrawString(MainGame.Font, String.Format("MS: {0}", _stepTime.ToString("##0.00")),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 50), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("X: {0}", Player.SimPosition.X.ToString("0.00")),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 100), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("Y: {0}", Player.SimPosition.Y.ToString("0.00")),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 150), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("EXP: {0}", Player.Exp),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 200), Color.White);
            }
            HUD.Draw(_spriteBatch);
            _spriteBatch.End();
        }
    }
}
