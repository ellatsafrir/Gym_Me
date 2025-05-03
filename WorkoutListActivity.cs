using Android.Content;

namespace Gym_Me;

[Activity(Label = "Workout List")]
public class WorkoutListActivity : Activity
{
    TextView workoutName;
    ListView listView;
    Button startButton;
    List<Exercise> exercises;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_workout_list);

        workoutName = FindViewById<TextView>(Resource.Id.workoutTitleText);
        listView = FindViewById<ListView>(Resource.Id.exerciseListView);
        startButton = FindViewById<Button>(Resource.Id.startWorkoutButton);

        var db = new DatabaseHelper(this);
        int workoutId = Intent.GetIntExtra("WorkoutId", -1);
        exercises = db.GetExercisesForWorkout(workoutId);
        var workout = db.GetWorkoutById(workoutId);
        workoutName.Text = workout.Name;

        listView.Adapter = new ArrayAdapter(
            this,
            Android.Resource.Layout.SimpleListItem1,
            exercises.Select(e => e.ToString())
                     .ToList()
         );

        startButton.Click += (s, e) =>
        {
            var intent = new Intent(this, typeof(WorkoutTimerActivity));
            intent.PutExtra("WorkoutId", workoutId);
            intent.PutExtra("ExerciseIndex", 0);
            StartActivity(intent);
        };
    }
}
