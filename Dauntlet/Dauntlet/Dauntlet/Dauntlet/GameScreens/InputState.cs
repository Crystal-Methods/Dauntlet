using Microsoft.Xna.Framework.Input;

namespace Dauntlet.GameScreens
{
    public class InputState
    {

        public KeyboardState CurrentKeyboardState;
        public GamePadState CurrentGamePadState;

        public KeyboardState LastKeyboardState;
        public GamePadState LastGamePadState;

        public bool GamePadWasConnected;

        public InputState()
        {
            CurrentKeyboardState = new KeyboardState();
            CurrentGamePadState = new GamePadState();

            LastKeyboardState = new KeyboardState();
            LastGamePadState = new GamePadState();
        }

        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            LastGamePadState = CurrentGamePadState;

            CurrentKeyboardState = Keyboard.GetState();
            CurrentGamePadState = GamePad.GetState(0, GamePadDeadZone.Circular);

            // Keep track of whether a gamepad has ever been
            // connected, so we can detect if it is unplugged.
            GamePadWasConnected = CurrentGamePadState.IsConnected;
        }

        public bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) &&
                    LastKeyboardState.IsKeyUp(key));
        }

        public bool IsNewButtonPress(Buttons button)
        {
            return (CurrentGamePadState.IsButtonDown(button) &&
                    LastGamePadState.IsButtonUp(button));
        }

        public bool IsMenuSelect()
        {
            return IsNewKeyPress(Keys.Space) || IsNewKeyPress(Keys.Enter) ||
                   IsNewButtonPress(Buttons.A) || IsNewButtonPress(Buttons.Start);
        }

         public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape) || IsNewButtonPress(Buttons.B) ||
                   IsNewButtonPress(Buttons.Back);
        }

        public bool IsMenuUp()
        {
            return IsNewKeyPress(Keys.Up) || IsNewButtonPress(Buttons.DPadUp) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp);
        }

        public bool IsMenuDown()
        {
            return IsNewKeyPress(Keys.Down) || IsNewButtonPress(Buttons.DPadDown) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown);
        }

        public bool IsPauseGame()
        {
            return IsNewKeyPress(Keys.Escape) || IsNewButtonPress(Buttons.Back) ||
                   IsNewButtonPress(Buttons.Start);
        }

    }
}
