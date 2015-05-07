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
        public Vector2 DisplayRoomCenter { get { return new Vector2(TileEngine.TileEngine.CurrentRoom.DisplayWidth/2f,
            TileEngine.TileEngine.CurrentRoom.DisplayHeight/2f);} }
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
            TileEngine.TileEngine.LoadContent(this, _content);
            HUD.Init();
            ConvertUnits.SetDisplayUnitToSimUnitRatio(TileEngine.TileEngine.TileSize); // 1 meter = 1 tile

            World = TileEngine.TileEngine.CurrentRoom.World;
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
            World.Step(_stepTime / 2000f);
            Player.Update(gameTime);
            foreach (var entity in TileEngine.TileEngine.CurrentRoom.Entities.Where(entity => !entity.Dead))
                entity.Update(gameTime);
            TileEngine.TileEngine.CurrentRoom.Entities.AddRange(TileEngine.TileEngine.CurrentRoom.AddQueue);
            TileEngine.TileEngine.CurrentRoom.AddQueue.Clear();
            foreach (Entity e in TileEngine.TileEngine.CurrentRoom.RemoveQueue) TileEngine.TileEngine.CurrentRoom.Entities.Remove(e);
            TileEngine.TileEngine.CurrentRoom.RemoveQueue.Clear();

            //Update the world again
            World.Step(_stepTime / 2000f);
            Player.Update(gameTime);
            foreach (var entity in TileEngine.TileEngine.CurrentRoom.Entities.Where(entity => !entity.Dead))
                entity.Update(gameTime);
            TileEngine.TileEngine.CurrentRoom.Entities.AddRange(TileEngine.TileEngine.CurrentRoom.AddQueue);
            TileEngine.TileEngine.CurrentRoom.AddQueue.Clear();
            foreach (Entity e in TileEngine.TileEngine.CurrentRoom.RemoveQueue) TileEngine.TileEngine.CurrentRoom.Entities.Remove(e);
            TileEngine.TileEngine.CurrentRoom.RemoveQueue.Clear();
            
            // Update camera
            _view = CameraManager.MoveCamera(Player.DisplayPosition);

            // Handle input
            if (MainGame.Input.IsMovement() && !Player.IsPunching)
                Player.Move(MainGame.Input.CurrentKeyboardState, MainGame.Input.CurrentGamePadState);
            else
                Player.CurrentSpeed = Vector2.Zero;
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

            //Handle death
            if (Player.Dead && DeathScreen.DeathSwitch)
                ((MenuScreen)MainGame.GetScreen(Screen.DeathScreen)).OverlayDeathScreen(this);
            
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0, 0, 0));

            // First pass: Draw room
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, _view);

            TileEngine.TileEngine.DrawRoom(GraphicsDevice, _spriteBatch, gameTime);

            if (DebugCollision)
                TileEngine.TileEngine.DrawDebug(_spriteBatch, GraphicsDevice);

            Player.Draw(gameTime, _spriteBatch);

            foreach (var entity in TileEngine.TileEngine.CurrentRoom.Entities.Where(entity => !entity.Dead))
                entity.Draw(gameTime, _spriteBatch);

            TileEngine.TileEngine.DrawWallCaps(_spriteBatch);

            _spriteBatch.End();

            // Fourth pass: HUD
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
            if (DebugCollision)
            {
                _spriteBatch.DrawString(MainGame.Font, String.Format("MS: {0}", _stepTime.ToString("##0.00")),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 50), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("X: {0}", Player.Position.X.ToString("0.00")),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 100), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("Y: {0}", Player.Position.Y.ToString("0.00")),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 150), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("Lvl: {0}", Player.Level),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 200), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("EXP: {0}", Player.Exp),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 250), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("2Nxt: {0}", Player.ExpToNextLevel),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 300), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("Room: {0}", TileEngine.TileEngine.CurrentRoomName ),
                    new Vector2(GraphicsDevice.Viewport.Width - 200, 350), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("TEX: {0}", (int)(Convert.ToDouble(Player.Position.X) * 64)),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 400), Color.White);
                _spriteBatch.DrawString(MainGame.Font, String.Format("TEY: {0}", (int)(Convert.ToDouble(Player.Position.Y) * 64)),
                    new Vector2(GraphicsDevice.Viewport.Width - 100, 450), Color.White);
            }
            _spriteBatch.DrawString(MainGame.Font, String.Format("XP Level: {0}", Player.Level),
                    new Vector2(42, 222), Color.Black);
            _spriteBatch.DrawString(MainGame.Font, String.Format("XP Level: {0}", Player.Level),
                    new Vector2(40,220), Color.White);

            if (Player._bonus)
            {
                _spriteBatch.DrawString(MainGame.Font, String.Format("Bonus: {0}", (5 - (Player._levelUpTime / 1000)).ToString("0.00")),
                    new Vector2(683, 600), Color.White);
            }

            HUD.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
        }
    }
}
