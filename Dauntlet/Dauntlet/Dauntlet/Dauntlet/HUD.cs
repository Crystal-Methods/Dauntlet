using System;
using Dauntlet.GameScreens;
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
            _levelUpText.AddAnimation("LevelUp", 0, 0, 204, 87, 10, 1/32f, false, false);
            _levelUpText.SetAnimation("LevelUp");

            _hsa = new Vector2(-(float) Math.Cos(22.38), (float) Math.Sin(22.38));
            _hsa.Normalize();
            _hsa *= 40;
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

            // Draw the EXP bar
            var expBarLength = (int)Math.Round(_expbar.Width* GameplayScreen.Player.SmoothExp/GameplayScreen.Player.ExpToNextLevel);
            var sourceRect = new Rectangle(0, 0, expBarLength, _expbar.Height);
            spriteBatch.Draw(_expbar, new Vector2(133, 61), Color.Black);
            spriteBatch.Draw(_expbar, new Vector2(133, 61), sourceRect, Color.White);

            // Draw the lifebar
            spriteBatch.Draw(_lifebar, new Vector2(30, 40), Color.White);

            // Draw the flame
            spriteBatch.Draw(_flame.Sheet, new Vector2(83, 151), _flame.CurrentFrame, Color.White, 0f, new Vector2(132, 194), 1f, SpriteEffects.None, 0f);

            // Draw the health stocks
            for (int i = 0; i < GameplayScreen.Player.HitPoints; i++)
                spriteBatch.Draw(_healthStock.Sheet, new Vector2(100, 70) + (_hsa * i), _healthStock.CurrentFrame, Color.White, 0f, new Vector2(10, 10), 1f, SpriteEffects.None, 0f);
            
            //This batch exists for example purposes until I can get it to play once based on the conditional below
            spriteBatch.Draw(_levelUpText.Sheet, new Vector2(250, 30), _levelUpText.CurrentFrame, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
    
            //Draw Level Up
            if (GameplayScreen.Player.Exp == 0 && GameplayScreen.Player.Level > 1)
            {
                    //Display animation once and delete last frame after a couple seconds                       
            }         
        }

    }
}
