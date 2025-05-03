using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Gym_Me
{
    [Activity(Label = "Workout Summary")]
    public class WorkoutSummaryActivity : Activity
    {
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

            // Use the custom adapter
            //var adapter = new WorkoutSetAdapter(this, allSets);
            //listView.Adapter = adapter;


            // Retrieve the workout logs passed through the intent
            var json = Intent.GetStringExtra("WorkoutLogs");
            List<WorkoutLog> workoutLogs = JsonConvert.DeserializeObject<List<WorkoutLog>>(json);

            if (workoutLogs == null || workoutLogs.Count == 0)
            {
                var noLogsText = new TextView(this) { Text = "No logs available for this workout." };
                container.AddView(noLogsText);
                return;
            }
            //List<string> logText = new List<string>();
            //logText.Add($"[{workoutLog.Id}] WorkoutId:{workoutLog.WorkoutId}");
            //foreach (var set in workoutLog.Sets)
            //{
            //    logText.Add(set.ToString());
            //}

            var adapter = new WorkoutLogAdapter(this, workoutLogs);
            listView.Adapter = adapter;

            //// Group and display the logs
            //foreach (var group in logs.GroupBy(l => l.Sets.FirstOrDefault()?.ExerciseId))
            //{
            //    var name = ExcersizeList.Instance.GetExcersizeData(group.Key?.ToString())?.Name ?? "Exercise";

            //    // Build the log text, combining set data
            //    var logText = $"{name}\n" + string.Join("\n", group.SelectMany(log => log.Sets)
            //        .Select(set => $"Set {set.Id}: {set.Reps} Reps | Time: {set.Time.SetTime}s | Rest: {set.Time.RestTime}s"));

            //    // Create and add a TextView for this log
            //    var logView = new TextView(this)
            //    {
            //        Text = logText
            //    };

            //    container.AddView(logView);
            //}

            // Handle the save button click event
            FindViewById<Button>(Resource.Id.saveSummaryButton).Click += (s, e) => Finish();
        }
    }
}
