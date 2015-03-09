using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public class PauseScreen : GameScreen
    {
        private const string menuTitle = "PAUSED";

        private Texture2D _darkness;
        private SpriteFont font;
        private ContentManager content;
        private SpriteBatch spriteBatch;

        public PauseScreen(Dauntlet game) : base(game)
        {
        }

        public override void LoadContent()
        {
            content = MainGame.Content;
            _darkness = new Texture2D(MainGame.Graphics, MainGame.Graphics.Viewport.Width, MainGame.Graphics.Viewport.Height);
            var data = new Color[MainGame.Graphics.Viewport.Width * MainGame.Graphics.Viewport.Height];
            for (int i = 0; i < data.Length; i++) data[i] = new Color(0, 0, 0, 0.1f);
            _darkness.SetData(data);
            font = content.Load<SpriteFont>("loadingFont");
        }

        public override void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gametime)
        {
            spriteBatch.Begin();
            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(MainGame.Graphics.Viewport.Width / 2f, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            float titleScale = 1.25f;
            spriteBatch.DrawString(font, menuTitle, titlePosition, Color.White, 0f, titleOrigin, titleScale, SpriteEffects.None, 0f);

            spriteBatch.End();
        }
    }
}
