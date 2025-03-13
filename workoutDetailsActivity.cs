//using Android.App;
//using Android.OS;
//using Android.Widget;
//using System;

//namespace Gym_Me
//{
//    [Activity(Label = "Workout Details")]
//    public class WorkoutDetailActivity : Activity
//    {
//        private TextView workoutNameTextView;
//        private Button editWorkoutButton;
//        private Button deleteWorkoutButton;
//        private string workoutName;
//        private DatabaseHelper dbHelper;

//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);
//            SetContentView(Resource.Layout.activity_workout_detail);

//            dbHelper = new DatabaseHelper(this);
//            workoutNameTextView = FindViewById<TextView>(Resource.Id.workoutNameTextView);
//            editWorkoutButton = FindViewById<Button>(Resource.Id.editWorkoutButton);
//            deleteWorkoutButton = FindViewById<Button>(Resource.Id.deleteWorkoutButton);

//            // Get the workout name passed from WorkoutHistoryActivity
//            workoutName = Intent.GetStringExtra("WorkoutName");

//            workoutNameTextView.Text = workoutName;

//            editWorkoutButton.Click += EditWorkoutButton_Click;
//            deleteWorkoutButton.Click += DeleteWorkoutButton_Click;
//        }

//        private void EditWorkoutButton_Click(object sender, EventArgs e)
//        {
//            // Launch the activity to edit the workout
//            var intent = new Android.Content.Intent(this, typeof(EditWorkoutActivity));
//            intent.PutExtra("WorkoutName", workoutName);
//            StartActivity(intent);
//        }

//        private void DeleteWorkoutButton_Click(object sender, EventArgs e)
//        {
//            // Find the workout ID and delete it from the database
//            int workoutId = dbHelper.GetWorkoutIdByName(workoutName);
//            if (workoutId != -1)
//            {
//                dbHelper.DeleteWorkout(workoutId);
//                Toast.MakeText(this, "Workout deleted", ToastLength.Short).Show();
//                Finish(); // Close this activity and return to the list
//            }
//        }
//    }
//}
