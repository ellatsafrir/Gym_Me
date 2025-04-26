
//using Gym_Me;

//[Activity(Label = "Edit Workout")]
//public class EditWorkoutActivity : Activity
//{
//    private DatabaseHelper _databaseHelper;

//    protected override void OnCreate(Bundle savedInstanceState)
//    {
//        base.OnCreate(savedInstanceState);
//        SetContentView(Resource.Layout.EditWorkoutActivity);

//        var workoutNameEditText = FindViewById<EditText>(Resource.Id.editWorkoutName);
//        var setsRecyclerView = FindViewById<RecyclerView>(Resource.Id.setsRecyclerView);
//        var saveButton = FindViewById<Button>(Resource.Id.saveChangesBtn);

//        _databaseHelper = new DatabaseHelper(); // Your Database Helper instance

//        // Get the workout ID from the intent
//        int workoutId = Intent.GetIntExtra("workoutId", -1);

//        if (workoutId != -1)
//        {
//            var workout = _databaseHelper.GetWorkout(workoutId);
//            workoutNameEditText.Text = workout.Name;

//            // Populate the RecyclerView with the sets of this workout
//            var setsAdapter = new SetsAdapter(workout.Sets);
//            setsRecyclerView.SetAdapter(setsAdapter);

//            saveButton.Click += (sender, e) =>
//            {
//                // Update the workout name
//                workout.Name = workoutNameEditText.Text;

//                // Save changes to the database
//                _databaseHelper.UpdateWorkout(workout);

//                // Get updated sets and save them (You might have another method for this)
//                var updatedSets = setsAdapter.GetUpdatedSets();
//                _databaseHelper.UpdateSets(updatedSets);

//                Toast.MakeText(this, "Workout updated!", ToastLength.Short).Show();
//                Finish();  // Close the activity
//            };
//        }
//    }
//}
