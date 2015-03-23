using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet.GameScreens
{
    public class PauseScreen : MenuScreen
    {
        private const string MenuTitle = "PAUSED";

        private Texture2D _darkness;

        public PauseScreen(Dauntlet game) : base(game, MenuTitle)
        {
            // Create our menu entries.
            var resumeGameMenuEntry = new MenuItem("Resume Game");
            var quitGameMenuEntry = new MenuItem("Quit Game");

            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        public override void LoadContent()
        {
            _darkness = new Texture2D(MainGame.Graphics, MainGame.Graphics.Viewport.Width, MainGame.Graphics.Viewport.Height);
            var data = new Color[MainGame.Graphics.Viewport.Width * MainGame.Graphics.Viewport.Height];
            for (int i = 0; i < data.Length; i++) data[i] = new Color(0, 0, 0, 0.1f);
            _darkness.SetData(data);
        }

        public override void UnloadContent()
        {
            throw new NotImplementedException();
        }

        void QuitGameMenuEntrySelected(object sender, EventArgs eventArgs)
        {
            //LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }

    }
}
