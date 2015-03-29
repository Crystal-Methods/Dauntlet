//CrystalMethods
//Section 2
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace simpleSounds
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //adding sound effect variable
        SoundEffect soundEffect;

        //adding xact variables
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue w, a, s, d;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            //loading/playing sound effects
            soundEffect = Content.Load<SoundEffect>(@"Audio\start");
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
            soundEffectInstance.Play();

            //initializing xact sounds
            audioEngine = new AudioEngine(@"Content\Audio\Sounds.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");

            //playing instructions, unmodified from a home recording
            soundBank.PlayCue("Instructions");

            //this is homer saying doh 32 times with the pitch increased
            w = soundBank.GetCue("W");
            
            //this is the I feel good song that has had its volume lowered
            a = soundBank.GetCue("A");
            
            //This is the scooby doo theme song that pans around
            s = soundBank.GetCue("S");
            
            //this is an "oh yeah" sound with a high pass filter
            d = soundBank.GetCue("D");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            //checking to see if the sounds have played/are playing and then playing the sounds to prevent crashes
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.W))
                if (!w.IsPlaying && !w.IsStopped)
                    w.Play();
            if (state.IsKeyDown(Keys.A))
                if (!a.IsPlaying&&!a.IsStopped)
                    a.Play();
            if (state.IsKeyDown(Keys.S))
                if (!s.IsPlaying && !s.IsStopped)
                    s.Play();
            if (state.IsKeyDown(Keys.D))
                if (!d.IsPlaying && !d.IsStopped)
                    d.Play();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
