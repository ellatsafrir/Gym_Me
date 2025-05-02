namespace Gym_Me;

[Activity(Label = "Workout Summary")]
public class WorkoutSummaryActivity : Activity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_workout_summary);

        var container = FindViewById<LinearLayout>(Resource.Id.logContainer);
        var workoutId = Intent.GetIntExtra("WorkoutId", -1);
        var logs = new DatabaseHelper(this).GetLogsForWorkout(workoutId);

        foreach (var group in logs.GroupBy(l => l.ExerciseId))
        {
            var name = ExcersizeList.Instance.GetExcersizeData(group.Key)?.Name ?? "Exercise";
            var logView = new TextView(this)
            {
                Text = $"{name}\n" + string.Join("\n", group.Select(x => $"{x.Weight} kg x {x.Reps}"))
            };
            container.AddView(logView);
        }

        FindViewById<Button>(Resource.Id.saveSummaryButton).Click += (s, e) => Finish();
    }
}
