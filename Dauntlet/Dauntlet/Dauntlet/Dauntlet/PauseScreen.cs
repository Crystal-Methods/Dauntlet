using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public class PauseScreen : GameScreen
    {
        private Texture2D _darkness;

        public PauseScreen(Dauntlet game) : base(game)
        {
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

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gametime)
        {
            throw new NotImplementedException();
        }
    }
}
