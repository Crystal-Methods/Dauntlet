using System;
using System.Collections.Generic;
using System.IO;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionTest.NewSpriteSystem
{
    internal static class SpriteManager
    {
        
        // This keeps track of all the textures and gives each one a unique name
        private static readonly Dictionary<String, Texture2D> Textures = new Dictionary<string, Texture2D>();

        // Loads all the textures and puts them in the Dictionary
        public static void Init(ContentManager contentManager)
        {
            string[] files = Directory.GetFiles(@"Content/Textures", "*.xnb");

            for (int i = 0; i < files.Length; i++)
            {
                String s = files[i].Substring(files[i].LastIndexOf('\\') + 1);
                s = s.Substring(0, s.Length - 4);
                files[i] = s;
            }

            foreach (string s in files)
            {
                var texture = contentManager.Load<Texture2D>(@"Textures/" + s);
                Textures.Add(s, texture);
            }
        }

         //Calling this will create a Player sprite.  You probably don't want to create more than one at a time!
        public static PlayerEntity CreatePlayer(World world, string newTextureName, Vector2 position)
        {
            return new PlayerEntity(world, Textures["MarioWalk"], 25f, 10f, position);
        }

        // Calling this will create an Enemy
        public static EnemyEntity CreateEnemy(World world, string newTextureName, Vector2 position)
        {
            return new EnemyEntity(world, Textures["DryBones"], 25f, 10f, position);
        }

        internal static Texture2D BoundingCircle { get { return Textures["BoundingCircle"]; } }

    }
}
