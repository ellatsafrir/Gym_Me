using Android.Content;
using Android.Media;
using Android.OS;

namespace Gym_Me
{
    public class AudioPlaybackUtility
    {
        private MediaPlayer _mediaPlayer;
        private HandlerThread _audioThread;
        private Handler _audioHandler;
        private bool _isPlaying = false;
        private const string AudioAction = "com.example.ACTION_STOP_AUDIO";

        public AudioPlaybackUtility()
        {
            // Initialize HandlerThread
            _audioThread = new HandlerThread("AudioThread");
            _audioThread.Start();
            _audioHandler = new Handler(_audioThread.Looper);
        }

        // Start playing audio from the given resource ID
        public void StartPlaying(Context context, int soundResourceId)
        {
            if (_isPlaying)
                return; // Audio is already playing

            _isPlaying = true;

            // Run audio playing logic in the background thread
            _audioHandler.Post(() =>
            {
                try
                {
                    _mediaPlayer = new MediaPlayer();
                    _mediaPlayer.SetDataSource(context.Resources.OpenRawResourceFd(soundResourceId));
                    _mediaPlayer.Prepare();
                    _mediaPlayer.Start();

                    // Wait for the audio to complete or stop
                    _mediaPlayer.Completion += (sender, e) =>
                    {
                        _isPlaying = false;
                        _mediaPlayer.Release();
                    };
                }
                catch (System.Exception ex)
                {
                    RunOnUiThread(() => Toast.MakeText(context, "Error playing audio: " + ex.Message, ToastLength.Short).Show());
                }
            });
        }

        // Stop the audio playback
        public void StopAudio()
        {
            if (_mediaPlayer != null)
            {
                try
                {
                    if (_mediaPlayer.IsPlaying)
                    {
                        _mediaPlayer.Stop();
                    }
                    _mediaPlayer.Release();
                }
                catch (Java.Lang.IllegalStateException ex)
                {
                    Console.WriteLine("MediaPlayer is in an invalid state: " + ex.Message);
                }
                finally
                {
                    _mediaPlayer = null;
                    _isPlaying = false;
                }
            }
        }

        // Register a receiver to listen for stop broadcast and stop audio
        public void RegisterStopReceiver(Context context)
        {
            var receiver = new AudioStopReceiver(this);
            context.RegisterReceiver(receiver, new IntentFilter(AudioAction));
        }

        // Unregister the receiver when done
        public void UnregisterStopReceiver(Context context)
        {
            context.UnregisterReceiver(new AudioStopReceiver(this));
        }

        // Helper method to run code on the UI thread
        private void RunOnUiThread(Action action)
        {
            Android.App.Application.SynchronizationContext.Post(_ => action(), null);
        }
    }

    public class AudioStopReceiver : BroadcastReceiver
    {
        private readonly AudioPlaybackUtility _audioPlaybackUtility;

        // Pass the AudioPlaybackUtility instance to the constructor
        public AudioStopReceiver(AudioPlaybackUtility audioPlaybackUtility)
        {
            _audioPlaybackUtility = audioPlaybackUtility;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == "com.example.ACTION_STOP_AUDIO")
            {
                // Call StopAudio method on the AudioPlaybackUtility instance
                _audioPlaybackUtility.StopAudio();
            }
        }
    }
}
