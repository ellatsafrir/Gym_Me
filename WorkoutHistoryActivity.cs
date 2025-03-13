using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gym_Me;

namespace Gym_Me
{
    [Activity(Label = "Workout History")]
    public class WorkoutHistoryActivity : Activity
    {
        private ListView workoutsListView;
        private List<string> workoutsList;
        private DatabaseHelper dbHelper;
        private ArrayAdapter<string> workoutsAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_workout_history);

            // Initialize views and database helper
            workoutsListView = FindViewById<ListView>(Resource.Id.workoutsListView);
            dbHelper = new DatabaseHelper(this);

            // Get all workouts from the database
            workoutsList = dbHelper.GetAllWorkouts();

            // Set up the adapter for the ListView
            workoutsAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, workoutsList);
            workoutsListView.Adapter = workoutsAdapter;

            // Set item click listener to show workout details or edit
            workoutsListView.ItemClick += WorkoutsListView_ItemClick;
        }

        // Item click event to handle workout selection
        private void WorkoutsListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string selectedWorkoutName = workoutsList[e.Position];

            // You can launch a new activity to show the workout details or edit it
            var intent = new Android.Content.Intent(this, typeof(WorkoutHistoryAdapter));
            intent.PutExtra("WorkoutName", selectedWorkoutName);
            StartActivity(intent);
        }
    }
}
