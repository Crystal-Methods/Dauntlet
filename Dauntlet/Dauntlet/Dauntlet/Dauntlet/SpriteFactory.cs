using System;
using System.Collections.Generic;
using System.IO;
using Dauntlet.Entities;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Dauntlet
{
    public enum EnemyTypes
    {
        Guapo
    }

    public enum ObjectTypes
    {
        Fountain,
        Tree
    }

    public static class SpriteFactory
    {
        private static readonly Dictionary<String, Texture2D> Textures = new Dictionary<string, Texture2D>();

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

            // Load these general-purpose textures
            Entity.DebugCircleTexture = Textures["Circle"];
            Entity.Shadow = Textures["Shadow"];
        }

        public static PlayerEntity CreatePlayer(World world, Vector2 position)
        {
            return new PlayerEntity(world, position, Textures["Dante"]);
        }

        public static EnemyEntity CreateEnemy(World world, Vector2 position, EnemyTypes type)
        {
            if (type == EnemyTypes.Guapo)
                return new Guapo(world, position, Textures["Guapo"]);
            throw new ArgumentException("One or more enemy types do not exist!");
        }

        public static StaticEntity CreateStaticEntity(World world, Vector2 position, ObjectTypes type)
        {
            if (type == ObjectTypes.Fountain)
            {
                var se = new StaticEntity(world, position, ConvertUnits.ToSimUnits(new Vector2(128, 39)),
                    Textures["Fountain"]);
                se.SetAnimation(0, 0, 128, 59, 3, 1 / 12f, false);
                return se;
            }
            if (type == ObjectTypes.Tree)
            {
                return new StaticEntity(world, new Vector2(577.5f, 351f), ConvertUnits.ToSimUnits(new Vector2(125, 30)), Textures["Tree"]);
            }
            throw new ArgumentException("One or more object types do not exist!");
        }
    }
}
