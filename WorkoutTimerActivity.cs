
using Android.Content;
using Android.OS;
using Newtonsoft.Json;


namespace Gym_Me
{
    [Activity(Label = "Workout Timer")]
    public class WorkoutTimerActivity : Activity
    {
        TextView exerciseNameText, timerText, setCounterText, weightText, repsText, rest;
        Button nextSetButton, pauseButton, skipSetButton, startTimerButton;

        bool isPaused = false;
        long remainingTime;
        Vibrator buzzer;
        AudioPlaybackUtility audioPlaybackUtility;
        

        // In?memory list to hold workout logs
        List<WorkoutLog> workoutLogs = new List<WorkoutLog>();
        WorkoutLog currentWorkoutLog;

        List<Exercise> exercises;
        int workoutId;
        int currentSet = 1;
        int totalSets = 3;
        int exerciseIndex = 0;
        CountDownTimer countDown;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_workout_timer);

            // Create instance of audio playback Thread
            audioPlaybackUtility = new AudioPlaybackUtility();
            // find views
            exerciseNameText = FindViewById<TextView>(Resource.Id.exerciseNameText);
            timerText = FindViewById<TextView>(Resource.Id.timerText);
            setCounterText = FindViewById<TextView>(Resource.Id.setCounterText);
            weightText = FindViewById<TextView>(Resource.Id.weightText);
            repsText = FindViewById<TextView>(Resource.Id.repsText);
            nextSetButton = FindViewById<Button>(Resource.Id.nextSetButton);
            pauseButton = FindViewById<Button>(Resource.Id.pauseButton);
            skipSetButton = FindViewById<Button>(Resource.Id.skipSetButton);
            startTimerButton = FindViewById<Button>(Resource.Id.startTimerButton);
            buzzer = (Vibrator)GetSystemService(VibratorService);
            rest = FindViewById<TextView>(Resource.Id.restText);

            // retrieve intent data
            workoutId = Intent.GetIntExtra("WorkoutId", -1);
            exerciseIndex = Intent.GetIntExtra("ExerciseIndex", 0);

            // prepare log
            currentWorkoutLog = new WorkoutLog
            {
                WorkoutId = workoutId,
                Sets = new List<WorkoutSetLog>(),
                Timestamp = DateTime.Now
            };

            // load exercises
            exercises = new DatabaseHelper(this).GetExercisesForWorkout(workoutId);

            // hook button events
            startTimerButton.Click += (s, e) => StartTimer();
            pauseButton.Click += (s, e) => TogglePause();
            skipSetButton.Click += (s, e) => SkipExercise();
            nextSetButton.Click += (s, e) => MoveToNextSet();  // if you want manual skip-to-next

            // initial UI
            LoadExercise();
        }

        void LoadExercise()
        {
            audioPlaybackUtility.StopAudio();

            if (exerciseIndex >= exercises.Count)
            {
                FinishWorkout();
                return;
            }

            var ex = exercises[exerciseIndex];
            var data = ExcersizeList.Instance.GetExcersizeData(ex.ExcersizeId);

            exerciseNameText.Text = data?.Name ?? "Exercise";
            weightText.Text = ex.Weight.ToString();
            repsText.Text = ex.Reps.ToString();
            totalSets = ex.Sets;
            remainingTime = (long)(ex.SetTime * 1000);
            setCounterText.Text = $"Set {currentSet} of {totalSets}";
            timerText.Text = "00:00";
            startTimerButton.Enabled = true;
        }

        void StartTimer()
        {
            audioPlaybackUtility.StopAudio();
            audioPlaybackUtility.StartPlaying(this, Resource.Raw.zambolino_go_on);
            rest.Text = "";
            // disable start to prevent re?entry
            startTimerButton.Enabled = false;

            countDown = new CountDownTimer(remainingTime, 1000)
            {
                Tick = millis => {
                    remainingTime = millis;
                    int sec = (int)(millis / 1000) % 60;
                    int min = (int)(millis / 1000) / 60;
                    timerText.Text = $"{min:00}:{sec:00}";
                },
                Finish = () => {
                    buzzer?.Vibrate(VibrationEffect.CreateOneShot(300, VibrationEffect.DefaultAmplitude));
                    MoveToNextSet();
                }
            };
            countDown.Start();
        }

        void TogglePause()
        {
            if (!isPaused)
            {
                countDown?.Cancel();
                isPaused = true;
                pauseButton.Text = "Resume";
            }
            else
            {
                isPaused = false;
                pauseButton.Text = "Pause";
                StartTimer();
            }
        }

        void SkipExercise()
        {
            // Stop any running timer
            countDown?.Cancel();

            // If there is another exercise, advance to it
            if (exerciseIndex < exercises.Count - 1)
            {
                // Move to next exercise
                exerciseIndex++;
                currentSet = 1;                             // reset set count
                var ex = exercises[exerciseIndex];
                totalSets = ex.Sets;                        // update total sets
                remainingTime = (long)(ex.SetTime * 1000);  // update work time

                // Refresh UI
                LoadExercise();

                // Enable Start so user can begin timing the first set of the new exercise
                startTimerButton.Enabled = true;
            }
            else
            {
                // No more exercises ? finish the workout
                FinishWorkout();
            }
        }

        void MoveToNextSet()
        {
            countDown?.Cancel();

            // log work set
            currentWorkoutLog.Sets.Add(new WorkoutSetLog
            {
                Id = currentSet,
                ExerciseId = exercises[exerciseIndex].ExcersizeId,
                Reps = exercises[exerciseIndex].Reps,
                Time = new WorkoutTimeLog
                {
                    SetTime = exercises[exerciseIndex].SetTime - (remainingTime / 1000),
                    RestTime = exercises[exerciseIndex].RestTime
                }
            });

            // advance counters
            if (++currentSet > totalSets)
            {
                currentSet = 1;
                exerciseIndex++;
                if (exerciseIndex >= exercises.Count)
                {
                    FinishWorkout(); // <- You need to implement this method
                    return;
                }
            }

            // reset work timer
            remainingTime = (long)(exercises[exerciseIndex].SetTime * 1000);
            setCounterText.Text = $"Set {currentSet} of {totalSets}";
            LoadExercise();

            // then run rest countdown
            StartRestTimer();
        }

        void StartRestTimer()
        {
            audioPlaybackUtility.StopAudio();
            audioPlaybackUtility.StartPlaying(this, Resource.Raw.moavii_belong);
            long restMs = (long)(exercises[exerciseIndex].RestTime * 1000);
            startTimerButton.Enabled = false;
            rest.Text = "rest";

            countDown = new CountDownTimer(restMs, 1000)
            {
                Tick = millis => {
                    int sec = (int)(millis / 1000) % 60;
                    int min = (int)(millis / 1000) / 60;
                    timerText.Text = $"{min:00}:{sec:00}";
                },
                Finish = () => {
                    buzzer?.Vibrate(VibrationEffect.CreateOneShot(300, VibrationEffect.DefaultAmplitude));
                    startTimerButton.Enabled = true;
                    timerText.Text = "00:00";
                }
            };
            countDown.Start();
        }

        void FinishWorkout()
        {
            audioPlaybackUtility.StopAudio();
            workoutLogs.Add(currentWorkoutLog);
            var intent = new Intent(this, typeof(WorkoutSummaryActivity));
            intent.PutExtra("WorkoutId", workoutId);
            intent.PutExtra("WorkoutLogs", JsonConvert.SerializeObject(workoutLogs));
            StartActivity(intent);
            Finish();
        }

        // helper timer
        public class CountDownTimer : Android.OS.CountDownTimer
        {
            public Action<long> Tick;
            public Action Finish;
            public CountDownTimer(long millisInFuture, long interval)
                : base(millisInFuture, interval) { }
            public override void OnTick(long msec) => Tick?.Invoke(msec);
            public override void OnFinish() => Finish?.Invoke();
        }
    }
}
