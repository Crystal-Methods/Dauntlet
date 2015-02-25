using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest
{
    internal class SpriteFactory
    {
        private readonly Game _game; // Instance of the Game object for reasons

        // This keeps track of all the textures and gives each one a unique name
        private readonly Dictionary<String, Texture2D> _textures = new Dictionary<string, Texture2D>();

        public SpriteFactory(Game game)
        {
            _game = game;
        }

        // Loads all the textures and puts them in the Dictionary
        public void LoadContent(ContentManager contentManager)
        {
            var texture = contentManager.Load<Texture2D>(@"Textures/Boo");
            _textures.Add(@"Boo", texture);

            texture = contentManager.Load<Texture2D>(@"Textures/DryBones");
            _textures.Add(@"DryBones", texture);
        }

        // Calling this will create a Player sprite.  You probably don't want to create more than one at a time!
        public PlayerEntity CreatePlayer(Vector2 newPosition, Vector2 newSpeed, string newTextureName)
        {
            return new PlayerEntity(_game, newPosition, newSpeed, _textures[newTextureName], newTextureName, 100, 70);
        }

        // Calling this will create an Enemy
        public EnemyEntity CreateEnemy(Vector2 newPosition, Vector2 newSpeed, string newTextureName)
        {
            return new EnemyEntity(_game, newPosition, newSpeed, _textures[newTextureName], newTextureName, 100, 70);
        }
    }
}
