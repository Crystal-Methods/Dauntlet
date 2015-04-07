using System;
using Microsoft.Xna.Framework.Audio;

namespace Dauntlet.GameScreens
{
    public class PauseScreen : MenuScreen
    {
        private const string MenuTitle = "PAUSED";
        public override Screen ScreenType { get { return Screen.PauseScreen; } }

        // ===============================

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

        void QuitGameMenuEntrySelected(object sender, EventArgs eventArgs)
        {
            ((GameplayScreen)LowerScreen).BgMusic.Stop(AudioStopOptions.Immediate);
            MainGame.ChangeScreen(Screen.TitleScreen);
        }

    }
}
