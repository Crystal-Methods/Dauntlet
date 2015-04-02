using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Dauntlet.GameScreens
{
    public class InputState
    {
        public KeyboardState CurrentKeyboardState;
        public GamePadState CurrentGamePadState;
        public KeyboardState LastKeyboardState;
        public GamePadState LastGamePadState;
        public bool GamePadWasConnected;

        // =========================================

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

        public bool IsQuitGame()
        {
            return IsNewKeyPress(Keys.Escape) || IsNewButtonPress(Buttons.Back);
        }

        public bool IsMovement()
        {
            return CurrentKeyboardState.IsKeyDown(Keys.W) || CurrentKeyboardState.IsKeyDown(Keys.A) ||
                   CurrentKeyboardState.IsKeyDown(Keys.S) ||
                   CurrentKeyboardState.IsKeyDown(Keys.D) || CurrentGamePadState.ThumbSticks.Left != Vector2.Zero;
        }

        public bool IsRotate()
        {
            return CurrentGamePadState.ThumbSticks.Right != Vector2.Zero;
        }

        public bool IsToggleDebug()
        {
            return IsNewKeyPress(Keys.F3) || IsNewButtonPress(Buttons.RightShoulder);
        }

        public bool IsAttack()
        {
            return IsNewKeyPress(Keys.Space) || IsNewButtonPress(Buttons.RightTrigger);
        }

    }
}
