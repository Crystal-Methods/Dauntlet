using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Dauntlet
{
    public static class SoundManager
    {
        
        static readonly Dictionary<String, SoundEffectInstance> SfxList = new Dictionary<string, SoundEffectInstance>();
        static readonly Dictionary<String, Song> SongList = new Dictionary<string, Song>();
        private static Song _currentSong;
        private static string _currentSongName;

        public static void LoadContent(ContentManager contentManager)
        {
            // Initialize songs
            Song temp;

            temp = contentManager.Load<Song>(@"Sounds/Songs/DauntletMainTheme");
            SongList.Add(@"MainTheme", temp);

            temp = contentManager.Load<Song>(@"Sounds/Songs/DauntletNoCombat");
            SongList.Add(@"NoCombat", temp);

            temp = contentManager.Load<Song>(@"Sounds/Songs/SkeletonSwing");
            SongList.Add(@"SkeletonSwing", temp);

            //Initialize SFX
            SoundEffect effect;

            effect = contentManager.Load<SoundEffect>(@"Sounds/SoundFX/Hop");
            SfxList.Add("Hop", effect.CreateInstance());

            effect = contentManager.Load<SoundEffect>(@"Sounds/SoundFX/Swish_1");
            SfxList.Add("Swish", effect.CreateInstance());

        }

        public static SoundEffectInstance GetInstance(string name)
        {
            return SfxList[name];
        }

        public static void Play(string name)
        {
            SfxList[name].Play();
        }

        public static void PlaySong(string name)
        {
            _currentSong = SongList[name];
            _currentSongName = name;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_currentSong);
        }

        public static void VolumeChange(float newVolume)
        {
            MediaPlayer.Volume = newVolume;
        }

        public static TimeSpan GetTimeSpanStart()
        {
            return MediaPlayer.PlayPosition;
        }

        public static TimeSpan GetTimeSpanEnd()
        {
            return _currentSong.Duration;
        }

        public static string GetCurrentSongName()
        {
            return _currentSongName;
        }

    }
}

