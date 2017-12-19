/*
 * Copyright (C) 2004-2017 AMain.com, Inc.
 * All Rights Reserved
 */

using System.Threading.Tasks;
using Android.App;
using Android.Media;

namespace AndroidSound
{
    /// <summary>
    /// class to gather up all the common functions used by all client applications
    /// </summary>
    public class PlaySoundMediaPlayer
    {
        private readonly MediaPlayer _barCodeSuccess;

        /// <summary>
        /// Constructor to create all the sounds we play
        /// </summary>
        public PlaySoundMediaPlayer()
        {
            LoadSound(ref _barCodeSuccess, "success.wav");
        }

        /// <summary>
        /// Loads a sound from the assets into a buffer and prepares it for playing.
        /// </summary>
        /// <param name="player">Media player for the sound loaded as an Asset</param>
        /// <param name="assetName">Name of the asset file to load</param>
        private static void LoadSound(
            ref MediaPlayer player,
            string assetName)
        {
            try {
                // Create the media player
                player = new MediaPlayer();
                player.SetAudioStreamType(Stream.Music);

                // Load the sound buffer from the assets folder if not already loaded
                using (var fd = Application.Context.Assets.OpenFd(assetName)) {
                    player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                    player.Prepare();
                }
            } catch {
                player?.Dispose();
                player = null;
            }
        }

        /// <summary>
        /// Plays a sound from the a prepared media player
        /// </summary>
        /// <param name="player">Media player to play</param>
        private static void PlayIt(
            MediaPlayer player)
        {
            if (player != null) {
                Task.Run(() => {
                    try {
                        // Stop if currently playing and rewind to the start
                        if (player.IsPlaying) {
                            player.Stop();
                            player.SeekTo(0);
                        }

                        // Now play the sound
                        player.Start();
                    } catch {
                    }
                });
            }
        }

        /// <summary>
        /// Function to play the barcode scan success sound
        /// </summary>
        public void BarCodeSuccess()
        {
            PlayIt(_barCodeSuccess);
        }
    }
}
