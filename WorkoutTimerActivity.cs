using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Gym_Me
{
    [Activity(Label = "Workout Timer")]
    public class WorkoutTimerActivity : Activity
    {
        TextView exerciseNameText, timerText, setCounterText, weightText, repsText;
        Button nextSetButton, pauseButton, skipSetButton, startTimerButton;

        bool isPaused = false;
        long remainingTime = 45000;
        Vibrator buzzer;

        // In-memory list to hold workout logs
        List<WorkoutLog> workoutLogs = new List<WorkoutLog>();  // Holds all logs for the workout session
        WorkoutLog currentWorkoutLog;

        List<Exercise> exercises;
        int workoutId;
        int currentSet = 1;
        int totalSets = 3;
        int exerciseIndex = 0;
        long timeLeft = 45000;
        CountDownTimer countDown;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_workout_timer);

            exerciseNameText = FindViewById<TextView>(Resource.Id.exerciseNameText);
            timerText = FindViewById<TextView>(Resource.Id.timerText);
            setCounterText = FindViewById<TextView>(Resource.Id.setCounterText);
            weightText = FindViewById<TextView>(Resource.Id.weightText);
            repsText = FindViewById<TextView>(Resource.Id.repsText);
            nextSetButton = FindViewById<Button>(Resource.Id.nextSetButton);
            pauseButton = FindViewById<Button>(Resource.Id.pauseButton);
            skipSetButton = FindViewById<Button>(Resource.Id.skipSetButton);
            buzzer = (Vibrator)GetSystemService(VibratorService);
            startTimerButton = FindViewById<Button>(Resource.Id.startTimerButton);

            // Initialize workout log
            currentWorkoutLog = new WorkoutLog
            {
                WorkoutId = workoutId,
                Sets = new List<WorkoutSetLog>(),  // Empty list to hold individual sets
                Timestamp = DateTime.Now  // Store the current timestamp of the workout
            };

            startTimerButton.Click += (s, e) => StartTimer();
            pauseButton.Click += (s, e) => TogglePause();
            skipSetButton.Click += (s, e) => SkipCurrentSet();

            workoutId = Intent.GetIntExtra("WorkoutId", -1);
            exerciseIndex = Intent.GetIntExtra("ExerciseIndex", 0);

            exercises = new DatabaseHelper(this).GetExercisesForWorkout(workoutId);
            LoadExercise();

            nextSetButton.Click += (s, e) => MoveToNextSet();
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
                StartTimer();  // resumes with remainingTime
            }
        }

        void LoadExercise()
        {
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
            setCounterText.Text = $"Set {currentSet} of {totalSets}";
            remainingTime = 45000; // Reset timer to 45 seconds
        }

        void StartTimer()
        {
            // Disable the Start button to prevent multiple presses
            startTimerButton.Enabled = false;

            // Create and start the countdown timer
            countDown = new CountDownTimer(remainingTime, 1000)
            {
                Tick = millis =>
                {
                    remainingTime = millis;
                    timerText.Text = $"00:{millis / 1000:00}";
                },
                Finish = () =>
                {
                    remainingTime = 45000;  // Reset for the next set
                    buzzer?.Vibrate(VibrationEffect.CreateOneShot(300, VibrationEffect.DefaultAmplitude));
                    MoveToNextSet();
                }
            };

            countDown.Start();
        }

        void SkipCurrentSet()
        {
            countDown?.Cancel();
            remainingTime = 45000; // Reset time for next set

            buzzer?.Vibrate(VibrationEffect.CreateOneShot(200, VibrationEffect.DefaultAmplitude));

            // Store skipped set log
            currentWorkoutLog.Sets.Add(new WorkoutSetLog
            {
                Id = currentSet,
                ExerciseId = exercises[exerciseIndex].ExcersizeId,
                Reps = exercises[exerciseIndex].Reps,
                Time = new WorkoutTimeLog
                {
                    SetTime = 0,  // No time for skipped set
                    RestTime = 0
                }
            });

            // Move to the next set
            if (++currentSet > totalSets)
            {
                currentSet = 1;
                exerciseIndex++;
            }

            LoadExercise();
        }

        void MoveToNextSet()
        {
            countDown?.Cancel();

            // Store the completed set log
            currentWorkoutLog.Sets.Add(new WorkoutSetLog
            {
                Id = currentSet,
                ExerciseId = exercises[exerciseIndex].ExcersizeId,
                Reps = exercises[exerciseIndex].Reps,
                Time = new WorkoutTimeLog
                {
                    SetTime = 45 - (remainingTime / 1000),  // Time spent on this set
                    RestTime = 0  // We'll handle rest time after the set
                }
            });

            // Move to next set or exercise
            if (++currentSet > totalSets)
            {
                currentSet = 1;
                exerciseIndex++;
            }

            // Reset timer for next set
            remainingTime = 45000;

            // Update UI with new set number
            setCounterText.Text = $"Set {currentSet} of {totalSets}";

            // Restart the timer
            StartTimer();

            // Load the new exercise
            LoadExercise();
        }

        void FinishWorkout()
        {
            // Add the current workout log to the list of logs (if needed)
            workoutLogs.Add(currentWorkoutLog);  // Add the current log to the list of logs

            // Create an Intent to navigate to the summary activity
            var intent = new Intent(this, typeof(WorkoutSummaryActivity));

            // Pass the workoutId to the next activity
            intent.PutExtra("WorkoutId", workoutId);

            // Pass the workout logs as an array to the next activity
            intent.PutExtra("WorkoutLogs", JsonConvert.SerializeObject(workoutLogs));  // Pass the workoutLogs as an array

            // Start the WorkoutSummaryActivity
            StartActivity(intent);

            // Optionally finish the current activity (optional)
            Finish();
        }

        // Timer class to manage the countdown
        public class CountDownTimer : Android.OS.CountDownTimer
        {
            public Action<long> Tick;
            public Action Finish;

            public CountDownTimer(long millisInFuture, long interval) : base(millisInFuture, interval) { }

            public override void OnTick(long millisUntilFinished) => Tick?.Invoke(millisUntilFinished);
            public override void OnFinish() => Finish?.Invoke();
        }
    }
}
