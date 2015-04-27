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
        Zombie,
        Skeleton
    }

    public enum ObjectTypes
    {
        Fountain,
        Tree,
        Tree_2,
        Tree_3,
        Mausoleum,
        MausoleumLocked,
        DeadTree,
        Bush,
        Gravestone_1,
        Gravestone_2,
        Gravestone_3,
        Gravestone_4
    }

    public static class SpriteFactory
    {
        private static Dictionary<String, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private static GraphicsDevice _graphics;

        /// <summary>
        /// Initializes the Sprite Factory
        /// </summary>
        /// <param name="contentManager">the game's ContentManager object</param>
        /// <param name="graphics">the game's GraphicDevice object</param>
        public static void Init(ContentManager contentManager, GraphicsDevice graphics)
        {
            _textures = new Dictionary<string, Texture2D>();
            _graphics = graphics;
            string[] files = Directory.GetFiles(@"Content/Textures", "*.xnb");

            // Extract filenames
            for (int i = 0; i < files.Length; i++)
            {
                String s = files[i].Substring(files[i].LastIndexOf('\\') + 1);
                s = s.Substring(0, s.Length - 4);
                files[i] = s;
            }

            // Load textures into list
            foreach (string s in files)
                _textures.Add(s, contentManager.Load<Texture2D>(@"Textures/" + s));

            // Load these general-purpose textures in Entity class
            Entity.DebugCircleTexture = _textures["Circle"];
            Entity.Shadow = _textures["Shadow"];
        }

        /// <summary>
        /// Creates a rectangular texture of a solid color
        /// </summary>
        /// <param name="height">the height of the texture</param>
        /// <param name="width">the width of the texture</param>
        /// <param name="color">the color of the texture</param>
        /// <returns>a texture of the specified height, width, and color</returns>
        public static Texture2D GetRectangleTexture(int height, int width, Color color)
        {
            var rect = new Texture2D(_graphics, width, height);
            var data = new Color[width * height];
            for (int i = 0; i < data.Length; i++) data[i] = color;
            rect.SetData(data);
            return rect;
        }

        /// <summary>
        /// Fetches the texture with the specified file name
        /// </summary>
        /// <param name="texName">the name of the texture to retrieve</param>
        /// <returns>the specified texture</returns>
        public static Texture2D GetTexture(string texName)
        {
            return _textures[texName];
        }

        /// <summary>
        /// Creates a new Player entity and adds it to the given world
        /// </summary>
        /// <param name="world">world in which to add the new Player</param>
        /// <param name="position">initial position for the Player, in display units</param>
        /// <returns>the new Player</returns>
        public static PlayerEntity CreatePlayer(World world, Vector2 position)
        {
            var playerTexture = new AnimatedTexture2D(_textures["Dante"]);
            playerTexture.AddAnimation("LookDown", 0, 0, 23, 33, 6, 1 / 12f, false, false);
            playerTexture.AddAnimation("LookDownLeft", 0, 33, 23, 33, 6, 1 / 12f, false, false);
            playerTexture.AddAnimation("LookLeft", 0, 66, 23, 33, 6, 1 / 12f, false, false);
            playerTexture.AddAnimation("LookUpLeft", 0, 99, 23, 33, 6, 1 / 12f, false, false);
            playerTexture.AddAnimation("LookUp", 0, 132, 23, 33, 6, 1 / 12f, false, false);
            playerTexture.AddAnimation("LookDownRight", 0, 33, 23, 33, 6, 1 / 12f, true, false);
            playerTexture.AddAnimation("LookRight", 0, 66, 23, 33, 6, 1 / 12f, true, false);
            playerTexture.AddAnimation("LookUpRight", 0, 99, 23, 33, 6, 1 / 12f, true, false);
            playerTexture.SetAnimation("LookRight");

            var gauntletTexture = new AnimatedTexture2D(_textures["Gauntlet"]);

            return new PlayerEntity(world, position.Sim(), playerTexture, gauntletTexture);
        }

        /// <summary>
        /// Creates a new Enemy entity of the specified type and adds it to the given world
        /// </summary>
        /// <param name="world">world in which to add the new Enemy</param>
        /// <param name="position">initial position for the Enemy, in display units</param>
        /// <param name="type">the type of enemy to create</param>
        /// <returns>the new Enemy</returns>
        public static EnemyEntity CreateEnemy(World world, Vector2 position, EnemyTypes type)
        {
            switch (type)
            {
                case EnemyTypes.Guapo:
                    var gaupoTexture = new AnimatedTexture2D(_textures["Guapo"]);
                    gaupoTexture.AddAnimation("Fly", 0, 0, 32, 32, 5, 1/24f, false, false);
                    gaupoTexture.SetAnimation("Fly");
                    return new Guapo(world, position.Sim(), gaupoTexture);

                case EnemyTypes.Zombie:
                    var zombieTexture = new AnimatedTexture2D(_textures["Zombie"]);
                    zombieTexture.AddAnimation("Walk", 0, 0, 64, 64, 4, 1/12f, false, false);
                    zombieTexture.SetAnimation("Walk");
                    return new Zombie(world, position.Sim(), zombieTexture);

                case EnemyTypes.Skeleton:
                    var skeletonTexture = new AnimatedTexture2D(_textures["Skeleton"]);
                    skeletonTexture.AddAnimation("Move", 0, 0, 64, 64, 4, 1 / 12f, false, false);
                    skeletonTexture.SetAnimation("Move");
                    return new Skeleton(world, position.Sim(), skeletonTexture);

                default:
                    throw new ArgumentException("One or more enemy types do not exist!");
            }
        }

        /// <summary>
        /// Creates a new Static entity of the specified type and adds it to the given world
        /// </summary>
        /// <param name="world">world in which to add the new entity</param>
        /// <param name="position">initial position for the entity, in display units</param>
        /// <param name="type">the type of Static entity to create</param>
        /// <returns>the new entity</returns>
        public static StaticEntity CreateStaticEntity(World world, Vector2 position, ObjectTypes type)
        {
            switch (type)
            {
                case ObjectTypes.Fountain:
                    var fountainTexture = new AnimatedTexture2D(_textures["Fountain"]);
                    fountainTexture.AddAnimation("Flow", 0, 0, 128, 59, 3, 1/8f, false, false);
                    fountainTexture.SetAnimation("Flow");
                    return new StaticEntity(world, position.Sim(), Units.SimVector(128, 39), fountainTexture);

                case ObjectTypes.Tree:
                    var treeTexture = new AnimatedTexture2D(_textures["Tree"]);
                    return new StaticEntity(world, position.Sim(), Units.SimVector(120, 30), treeTexture);

                case ObjectTypes.Tree_2:
                    var tree2Texture = new AnimatedTexture2D(_textures["Tree_2"]);
                    return new StaticEntity(world, position.Sim(), Units.SimVector(90, 30), tree2Texture);

                case ObjectTypes.Tree_3:
                    var tree3Texture = new AnimatedTexture2D(_textures["Tree_3"]);
                    return new StaticEntity(world, position.Sim(), Units.SimVector(50, 30), tree3Texture);

                case ObjectTypes.Mausoleum:
                    var mausoleumTexture = new AnimatedTexture2D(_textures["Mausoleum"]);
                    return new StaticEntity(world, position.Sim(), Units.SimVector(50, 30), mausoleumTexture);

                case ObjectTypes.MausoleumLocked:
                    var mausoleumLTexture = new AnimatedTexture2D(_textures["MausoleumLocked"]);
                    return new StaticEntity(world, position.Sim(), Units.SimVector(50, 30), mausoleumLTexture);

                case ObjectTypes.DeadTree:
                    var deadTreeTexture = new AnimatedTexture2D(_textures["DeadTree"]);
                    return new StaticEntity(world, position.Sim(), Units.SimVector(52, 30), deadTreeTexture);

                case ObjectTypes.Bush:
                    var bushTexture = new AnimatedTexture2D(_textures["BUSH"]);
                    return new StaticEntity(world, position.Sim(), Units.SimVector(125, 30), bushTexture);

                case ObjectTypes.Gravestone_1:
                    var grave1Texture = new AnimatedTexture2D(_textures["Gravestones"]);
                    grave1Texture.AddAnimation("Flow", 0, 0, 64, 59, 1, 1 / 8f, false, true);
                    grave1Texture.SetAnimation("Flow");
                    return new StaticEntity(world, position.Sim(), Units.SimVector(48, 8), grave1Texture);

                case ObjectTypes.Gravestone_2:
                    var grave2Texture = new AnimatedTexture2D(_textures["Gravestones"]);
                    grave2Texture.AddAnimation("Flow", 64, 0, 64, 59, 1, 1 / 8f, false, true);
                    grave2Texture.SetAnimation("Flow");
                    return new StaticEntity(world, position.Sim(), Units.SimVector(48, 8), grave2Texture);

                case ObjectTypes.Gravestone_3:
                    var grave3Texture = new AnimatedTexture2D(_textures["Gravestones"]);
                    grave3Texture.AddAnimation("Flow", 128, 0, 64, 59, 1, 1 / 8f, false, true);
                    grave3Texture.SetAnimation("Flow");
                    return new StaticEntity(world, position.Sim(), Units.SimVector(48, 8), grave3Texture);

                case ObjectTypes.Gravestone_4:
                    var grave4Texture = new AnimatedTexture2D(_textures["Gravestones"]);
                    grave4Texture.AddAnimation("Flow", 192, 0, 64, 59, 1, 1 / 8f, false, true);
                    grave4Texture.SetAnimation("Flow");
                    return new StaticEntity(world, position.Sim(), Units.SimVector(48, 8), grave4Texture);

                default:
                    throw new ArgumentException("One or more object types do not exist!");
            }
        }

    }
}
