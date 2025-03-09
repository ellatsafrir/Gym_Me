using Android.Views;
using Gym_Me;

public class WorkoutAdapter : BaseAdapter<Workout>
{
    private List<Workout> _workouts;
    private Activity _context;
    private DatabaseHelper _dbHelper;

    public WorkoutAdapter(Activity context, List<Workout> workouts, DatabaseHelper dbHelper)
    {
        _context = context;
        _workouts = workouts;
        _dbHelper = dbHelper;
    }

    public override Workout this[int position] => _workouts[position];

    public override int Count => _workouts.Count;

    public override long GetItemId(int position) => position;

    public override View GetView(int position, View convertView, ViewGroup parent)
    {
        var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.workout_item, parent, false);

        var workout = _workouts[position];
        var workoutNameTextView = view.FindViewById<TextView>(Resource.Id.workoutNameTextView);
        var workoutDateTextView = view.FindViewById<TextView>(Resource.Id.workoutDateTextView);
        var exercisesLayout = view.FindViewById<LinearLayout>(Resource.Id.exercisesLayout); // The layout to show exercises
        var exercisesTextView = view.FindViewById<TextView>(Resource.Id.exercisesTextView); // The TextView for exercises

        workoutNameTextView.Text = workout.Name;
        workoutDateTextView.Text = workout.Date.ToString("yyyy-MM-dd");

        // Handle item click to show/hide exercises
        view.Click += (sender, e) =>
        {
            if (exercisesLayout.Visibility == Android.Views.ViewStates.Gone)
            {
                exercisesLayout.Visibility = Android.Views.ViewStates.Visible;
                exercisesTextView.Text = string.Join("\n", workout.Exercises.Select(exercise =>
                    $"Reps: {exercise.Repetitions}, Weight: {exercise.Weight}, Rest: {exercise.RestTime}s"));

            }
            else
            {
                exercisesLayout.Visibility = Android.Views.ViewStates.Gone;
            }
        };

        return view;
    }

}
