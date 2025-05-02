using Android.Content;

namespace Gym_Me;

[Activity(Label = "Workout List")]
public class WorkoutListActivity : Activity
{
    ListView listView;
    Button startButton;
    List<Exercise> exercises;
    int workoutId = 1; // Or get from intent

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_workout_list);

        listView = FindViewById<ListView>(Resource.Id.exerciseListView);
        startButton = FindViewById<Button>(Resource.Id.startWorkoutButton);

        var db = new DatabaseHelper(this);
        exercises = db.GetExercisesForWorkout(workoutId);

        listView.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
            exercises.Select(e => ExcersizeList.Instance.GetExcersizeData(e.ExcersizeId).Name).ToList());

        startButton.Click += (s, e) =>
        {
            var intent = new Intent(this, typeof(WorkoutTimerActivity));
            intent.PutExtra("WorkoutId", workoutId);
            intent.PutExtra("ExerciseIndex", 0);
            StartActivity(intent);
        };
    }
}
