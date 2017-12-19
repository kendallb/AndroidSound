/*
 * Copyright (C) 2004-2017 AMain.com, Inc.
 * All Rights Reserved
 */

using Android.App;

namespace AndroidSound
{
    public class PlaySoundAudioTrack
    {
        private readonly WaveFile _barCodeSuccess;

        /// <summary>
        /// Constructor to create all the sounds we play
        /// </summary>
        public PlaySoundAudioTrack()
        {
            LoadSound(ref _barCodeSuccess, "success.wav");
        }

        /// <summary>
        /// Loads a sound from the assets into a buffer and prepares it for playing.
        /// </summary>
        /// <param name="waveFile">Wave file player for the sound loaded as an Asset</param>
        /// <param name="assetName">Name of the asset file to load</param>
        private static void LoadSound(
            ref WaveFile waveFile,
            string assetName)
        {
            try {
                using (var stream = Application.Context.Assets.Open(assetName)) {
                    waveFile = new WaveFile(stream);
                }
            } catch {
            }
        }

        /// <summary>
        /// Function to play the barcode scan success sound
        /// </summary>
        public void BarCodeSuccess()
        {
            _barCodeSuccess?.Play();
        }
    }
}
