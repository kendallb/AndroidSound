using System;
using Android.App;
using Android.Widget;
using Android.OS;

namespace AndroidSound
{
    [Activity(Label = "AndroidSound", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private PlaySoundMediaPlayer _playSoundMediaPlayer;
        private PlaySoundAudioTrack _playSoundAudioTrack;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Hook up the buttons
            var mediaPlayerButton = FindViewById<Button>(Resource.Id.buttonMediaPlayer);
            mediaPlayerButton.Click += MediaPlayerButtonOnClick;
            var audioTrackButton = FindViewById<Button>(Resource.Id.buttonAudioTrack);
            audioTrackButton.Click += AudioTrackButtonOnClick;

            // Create the sound players
            _playSoundMediaPlayer = new PlaySoundMediaPlayer();
            _playSoundAudioTrack = new PlaySoundAudioTrack();
        }

        private void MediaPlayerButtonOnClick(object sender, EventArgs eventArgs)
        {
            _playSoundMediaPlayer.BarCodeSuccess();
        }

        private void AudioTrackButtonOnClick(object sender, EventArgs eventArgs)
        {
            _playSoundAudioTrack.BarCodeSuccess();
        }
    }
}
