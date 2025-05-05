
using Android.Content;
using Newtonsoft.Json;


namespace Gym_Me
{
    [Activity(Label = "Workout Summary")]
    public class WorkoutSummaryActivity : Activity
    {
        Button save;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_workout_summary);

            // Retrieve workoutId passed from the previous activity
            var workoutId = Intent.GetIntExtra("WorkoutId", -1);  // Default to -1 if not passed

            // Check if the workoutId is valid
            if (workoutId == -1)
            {
                // Handle error or show message that workoutId is missing
                return;
            }

            var container = FindViewById<LinearLayout>(Resource.Id.logContainer);
            var listView = FindViewById<ListView>(Resource.Id.workoutSetListView);
            save = FindViewById<Button>(Resource.Id.saveSummaryButton);


            

            // Retrieve the workout logs passed through the intent
            var json = Intent.GetStringExtra("WorkoutLogs");
            List<WorkoutLog> workoutLogs = JsonConvert.DeserializeObject<List<WorkoutLog>>(json);

            if (workoutLogs == null || workoutLogs.Count == 0)
            {
                var noLogsText = new TextView(this) { Text = "No logs available for this workout." };
                container.AddView(noLogsText);
                return;
            }
           

            var adapter = new WorkoutLogAdapter(this, workoutLogs);
            listView.Adapter = adapter;

           

            // Handle the save button click event
            save.Click += (s, e) =>
            {
                // Log to ensure the button click event is triggered
                Android.Util.Log.Debug("WorkoutSummaryActivity", "Save button clicked");

                if (workoutLogs != null && workoutLogs.Count > 0)
                {
                    var dbHelper = new DatabaseHelper(this);

                    foreach (var log in workoutLogs)
                    {
                        dbHelper.InsertWorkoutLog(log);  // Insert into DB
                    }

                    Toast.MakeText(this, "Workout saved!", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "No workout logs to save.", ToastLength.Short).Show();
                }

                // Return to MainActivity
                var intent = new Intent(this, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
                StartActivity(intent);
                Finish();
            };

        }
    }
}
