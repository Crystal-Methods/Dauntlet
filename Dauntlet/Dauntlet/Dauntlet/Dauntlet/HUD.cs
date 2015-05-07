using System;
using Dauntlet.GameScreens;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    // ReSharper disable once InconsistentNaming
    public static class HUD
    {
        private static Texture2D _lifebar;
        private static Texture2D _expbar;
        private static AnimatedTexture2D _flame;
        private static AnimatedTexture2D _healthStock;
        private static AnimatedTexture2D _levelUpText;
        private static AnimatedTexture2D _key;
        private static bool isLevelUp;
        public static bool hasKey;
        private static float levelUpTimer;
        private static Vector2 _hsa; // Displacement between health stocks on the healthbar
        /// <summary>
        /// Initializes the HUD
        /// </summary>
        public static void Init()
        {
            _lifebar = SpriteFactory.GetTexture("Lifebar");

            _expbar = SpriteFactory.GetTexture("EXPbar");

            _flame = new AnimatedTexture2D(SpriteFactory.GetTexture("DauntletFire"));
            _flame.AddAnimation("Flicker1", 0, 0, 256, 256, 7, 1/10f, false, false);
            _flame.SetAnimation("Flicker1");

            _healthStock = new AnimatedTexture2D(SpriteFactory.GetTexture("HealthStock"));
            _healthStock.AddAnimation("healthStock",0,0,64,64,10,1/24f,false, false);
            _healthStock.SetAnimation("healthStock");

            _levelUpText = new AnimatedTexture2D(SpriteFactory.GetTexture("LevelUpText"));
            _levelUpText.AddAnimation("LevelUp", 0, 0, 204, 87, 10, 1/16f, false, true);
            _levelUpText.SetAnimation("LevelUp");

            _key = new AnimatedTexture2D(SpriteFactory.GetTexture("Key"));
            _key.AddAnimation("Key", 0, 0, 64, 64, 1, 1 / 12f, false, true);
            _key.SetAnimation("Key");

            _hsa = new Vector2(-(float) Math.Cos(22.38), (float) Math.Sin(22.38));
            _hsa.Normalize();
            _hsa *= 40;

            hasKey = false;
        }

        public static void LevelledUp()
        {
            isLevelUp = true;
            levelUpTimer = 0f;
            _levelUpText.Reset();
            GameplayScreen.Player.Speed += 0.125f;
        }


        public static void GotKey()
        {
            if(!hasKey)
                hasKey = true;
        }


        /// <summary>
        /// Draws the HUD
        /// </summary>
        /// <param name="gameTime">the game's GameTime object</param>
        /// <param name="spriteBatch">the game's SpriteBatch object</param>
        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _flame.StepAnimation(gameTime);
            _healthStock.StepAnimation(gameTime);
            _levelUpText.StepAnimation(gameTime); //Exists for example purposes
            _key.StepAnimation(gameTime);

            // Draw the EXP bar
            var expBarLength = (int)Math.Round(_expbar.Width* GameplayScreen.Player.SmoothExp/GameplayScreen.Player.ExpToNextLevel);
            var sourceRect = new Rectangle(0, 0, expBarLength, _expbar.Height);
            spriteBatch.Draw(_expbar, new Vector2(133, 61), Color.Black);
            spriteBatch.Draw(_expbar, new Vector2(133, 61), sourceRect, Color.White);

            // Draw the lifebar
            spriteBatch.Draw(_lifebar, new Vector2(30, 40), Color.White);

            // Draw the flame
            spriteBatch.Draw(_flame.Sheet, new Vector2(83, 151), _flame.CurrentFrame, Color.White, 0f, new Vector2(132, 194), 0.20f * GameplayScreen.Player.HitPoints, SpriteEffects.None, 0f);

            // Draw the health stocks
            for (int i = 0; i < GameplayScreen.Player.HitPoints; i++)
                spriteBatch.Draw(_healthStock.Sheet, new Vector2(100, 70) + (_hsa * i), _healthStock.CurrentFrame, Color.White, 0f, new Vector2(10, 10), 1f, SpriteEffects.None, 0f);

            //Draw Level Up
            if (isLevelUp)
            {
                 //Display animation once and delete last frame after a couple seconds                       
                 levelUpTimer += gameTime.ElapsedGameTime.Milliseconds / 2f;
                 if (levelUpTimer > 2000)
                     isLevelUp = false;
                 _levelUpText.StepAnimation(gameTime); //Exists for example purposes
                 spriteBatch.Draw(_levelUpText.Sheet, new Vector2(250, 30), _levelUpText.CurrentFrame, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }
            if (hasKey)
            {
                spriteBatch.Draw(_key.Sheet, new Vector2(40, 260), _key.CurrentFrame, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }
        }

    }
}
