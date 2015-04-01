using System;
using System.Collections.Generic;
using System.IO;
using Dauntlet.Entities;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public enum EnemyTypes
    {
        Guapo,
        Zombie
    }

    public enum ObjectTypes
    {
        Fountain,
        Tree
    }

    public static class SpriteFactory
    {
        private static Dictionary<String, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private static GraphicsDevice _graphics;

        public static void Init(ContentManager contentManager, GraphicsDevice graphics)
        {
            if (_textures.Count > 0)
                _textures = new Dictionary<string, Texture2D>();
            _graphics = graphics;
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
                _textures.Add(s, texture);
            }

            // Load these general-purpose textures
            Entity.DebugCircleTexture = _textures["Circle"];
            Entity.Shadow = _textures["Shadow"];
        }

        public static Texture2D GetRectangleTexture(int height, int width, Color color)
        {
            var rect = new Texture2D(_graphics, width, height);
            var data = new Color[width * height];
            for (int i = 0; i < data.Length; i++) data[i] = color;
            rect.SetData(data);
            return rect;
        }

        public static Texture2D GetTexture(string texName)
        {
            return _textures[texName];
        }

        public static PlayerEntity CreatePlayer(World world, Vector2 position)
        {
            return new PlayerEntity(world, position, _textures["Dante"], _textures["Gauntlet"]);
        }

        public static EnemyEntity CreateEnemy(World world, Vector2 position, EnemyTypes type)
        {
            if (type == EnemyTypes.Guapo)
                return new Guapo(world, position, _textures["Guapo"]);
            else if (type == EnemyTypes.Zombie)
                return new Zombie(world, position, _textures["Zombie"]);
            else
            throw new ArgumentException("One or more enemy types do not exist!");
        }

        public static StaticEntity CreateStaticEntity(World world, Vector2 position, ObjectTypes type)
        {
            if (type == ObjectTypes.Fountain)
            {
                var se = new StaticEntity(world, position, new Vector2(128, 39),
                    _textures["Fountain"]);
                se.SetAnimation(0, 0, 128, 59, 3, 1 / 8f, false);
                return se;
            }
            if (type == ObjectTypes.Tree)
            {
                return new StaticEntity(world, new Vector2(577.5f, 351f), new Vector2(125, 30), _textures["Tree"]);
            }
            throw new ArgumentException("One or more object types do not exist!");
        }
    }
}
