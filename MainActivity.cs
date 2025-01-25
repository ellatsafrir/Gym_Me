using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Gym_Me
{
    [Activity(Label = "MainActivity", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ListView workoutListView;
        private List<Workout> workoutList;
        private DatabaseHelper dbHelper;
        private Button history;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Check if the user is logged in
            if (!IsUserLoggedIn())
            {
                Intent intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);
                Finish();
                return;
            }

            // Initialize UI elements
            history = FindViewById<Button>(Resource.Id.historyPage);
            workoutListView = FindViewById<ListView>(Resource.Id.workoutListView);
            var addWorkoutButton = FindViewById<Button>(Resource.Id.addWorkoutButton);

            // Initialize database helper
            dbHelper = new DatabaseHelper(this);

            // Load today's workouts
            LoadTodayWorkouts();

            // Set up Add Workout button click event
            addWorkoutButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(AddWorkoutActivity));
                StartActivityForResult(intent, 1); // Request code 1
            };

            history.Click+= (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(WorkoutHistoryActivity));
                StartActivityForResult(intent, 1); // Request code 1
            };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1 && resultCode == Result.Ok)
            {
                // Refresh the workout list after adding or editing a workout
                LoadTodayWorkouts();
            }
        }

        private void LoadTodayWorkouts()
        {
            try
            {
                DateTime today = DateTime.Now.Date;

                // Fetch today's workouts
                var workoutNames = dbHelper.GetWorkoutsForDate(today);
                if (workoutNames == null || workoutNames.Count == 0)
                {
                    Toast.MakeText(this, "No workouts scheduled for today.", ToastLength.Short).Show();
                    workoutList = new List<Workout>();
                }
                else
                {
                    workoutList = new List<Workout>();

                    foreach (var workoutName in workoutNames)
                    {
                        // Fetch exercises for each workout
                        var exercises = dbHelper.GetExercisesForWorkout(workoutName) ?? new List<string>();
                        workoutList.Add(new Workout
                        {
                            Name = workoutName,
                            Exercises = exercises
                        });
                    }
                }

                // Bind the workout list to the adapter
                var adapter = new WorkoutAdapter(this, workoutList, dbHelper);
                workoutListView.Adapter = adapter;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Failed to load workouts: " + ex.Message, ToastLength.Long).Show();
            }
        }

        private bool IsUserLoggedIn()
        {
            var preferences = GetSharedPreferences("GymMePreferences", FileCreationMode.Private);
            string savedEmail = preferences.GetString("userEmail", null);
            return !string.IsNullOrEmpty(savedEmail);
        }
        //public  void Recreated() {

        //    //Intent intent = new Intent(this, typeof(MainActivity));
        //    //Finish(); // Close the current activity
        //    //StartActivity(intent); 
        //    LoadTodayWorkouts();
        //}
    }
   
}
