using Android.Content;
using Android.Views;
using Android.Widget;
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
        var exercisesLayout = view.FindViewById<LinearLayout>(Resource.Id.exercisesLayout);
        var exercisesTextView = view.FindViewById<TextView>(Resource.Id.exercisesTextView);

        workoutNameTextView.Text = workout.Name;
        workoutDateTextView.Text = workout.Date.ToString("yyyy-MM-dd");

        // Show/hide exercises on click
        view.Click += (sender, e) =>
        {
            if (exercisesLayout.Visibility == Android.Views.ViewStates.Gone)
            {
                exercisesLayout.Visibility = Android.Views.ViewStates.Visible;
                exercisesTextView.Text = string.Join("\n", workout.Exercises.Select(exercise =>

                    $"\"Exercise: {exercise}"));
            }
            else
            {
                exercisesLayout.Visibility = Android.Views.ViewStates.Gone;
            }
        };


        // Long press for popout menu
        view.LongClick += (sender, e) =>
        {
            PopupMenu popup = new PopupMenu(_context, view);
            popup.MenuInflater.Inflate(Resource.Menu.edit_popup_menu, popup.Menu);

            popup.MenuItemClick += (s, args) =>
            {
                if (args.Item.ItemId == Resource.Id.menu_edit)
                {
                    // Start edit workout activity
                    var intent = new Intent(_context, typeof(AddWorkoutActivity));
                    intent.PutExtra("WorkoutId", workout.Id);
                    _context.StartActivity(intent);
                }
                else if (args.Item.ItemId == Resource.Id.menu_delete)
                {
                    // Confirm and delete workout
                    AlertDialog.Builder alert = new AlertDialog.Builder(_context);
                    alert.SetTitle("Delete Workout");
                    alert.SetMessage("Are you sure you want to delete this workout?");
                    alert.SetPositiveButton("Delete", (senderAlert, argsAlert) =>
                    {
                        _dbHelper.DeleteWorkout(workout.Id);
                        _workouts.RemoveAt(position);
                        NotifyDataSetChanged();
                    });
                    alert.SetNegativeButton("Cancel", (senderAlert, argsAlert) => { });
                    alert.Show();
                }
            };

            popup.Show();
        };



        return view;
    }

}


