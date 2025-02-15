using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Gym_Me
{
    [Activity(Label = "Workout History")]
    public class WorkoutHistoryActivity : Activity
    {
        private ListView workoutHistoryListView;
        private List<Workout> workoutHistoryList;
        private DatabaseHelper dbHelper;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_workout_history);

            workoutHistoryListView = FindViewById<ListView>(Resource.Id.workoutListView);
            dbHelper = new DatabaseHelper(this);
            // Inside your Activity or Fragment's OnCreate or OnViewCreated method
            if (workoutHistoryListView == null)
            {
                Log.Error("WorkoutHistory", "ListView is null");
                return;
            }

            LoadWorkoutHistory();

            Button backButton = FindViewById<Button>(Resource.Id.backButton);
            backButton.Click += (sender, e) =>
            {
                Finish(); // Go back to the previous activity
            };
        }

        private void LoadWorkoutHistory()
        {
            var allWorkouts = dbHelper.GetAllWorkouts(); // Fetch all workout names from the database
            var workoutListWithDetails = new List<Workout>();

            foreach (var workoutName in allWorkouts)
            {
                var exercises = dbHelper.GetExercisesForWorkout(workoutName); // Get exercises for each workout
                workoutListWithDetails.Add(new Workout
                {
                    //Name = workoutName,
                    //Exercises = exercises
                });
            }

            // Set up a custom adapter for displaying workouts
            var adapter = new WorkoutHistoryAdapter(this, workoutListWithDetails);
            workoutHistoryListView.Adapter = adapter;
        }

    }
}
