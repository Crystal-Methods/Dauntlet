using System;
using Microsoft.Xna.Framework.Audio;

namespace Dauntlet.GameScreens
{
    public class DeathScreen:MenuScreen
    {
        //change this to false if you don't wish to die
        private static bool deathSwitch = true;

        private const string MenuTitle = "YOU DIED";
        public override Screen ScreenType { get { return Screen.DeathScreen; } } 

        // =======================================

        public DeathScreen(Dauntlet game)
            : base(game, MenuTitle)
        {
            //create only option
            var quitGameMenuEntry = new MenuItem("Quit Game");

            //Hook up menu even handlers
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            //Add entries to the menu.
            MenuEntries.Add(quitGameMenuEntry);

        }

        void QuitGameMenuEntrySelected(object sender, EventArgs eventArgs)
        {
            ((GameplayScreen)LowerScreen).BgMusic.Stop(AudioStopOptions.Immediate);
            MainGame.ChangeScreen(Screen.TitleScreen);
        }

        public static bool DeathSwitch { get { return deathSwitch; } }
    }
}
