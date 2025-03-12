using Android.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using System;
using System.Collections.Generic;
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
        var exercisesTextView = view.FindViewById<TextView>(Resource.Id.exercisesTextView);

        workoutNameTextView.Text = workout.Name;
        workoutDateTextView.Text = workout.Date.ToString("yyyy-MM-dd");

        view.LongClick += (sender, e) =>
        {
            ShowPopupMenu(view, workout);
        };

        return view;
    }

    private void ShowPopupMenu(View view, Workout workout)
    {
        PopupMenu menu = new PopupMenu(_context, view);
        menu.Inflate(Resource.Menu.edit_popup_menu);

        menu.MenuItemClick += (s, e) =>
        {
            if (e.Item.ItemId == Resource.Id.menu_edit)
            {
                Intent intent = new Intent(_context, typeof(AddWorkoutActivity));
                intent.PutExtra("WorkoutId", workout.Id);
                _context.StartActivity(intent);
            }
            else if (e.Item.ItemId == Resource.Id.menu_delete)
            {
                ShowDeleteConfirmation(workout);
            }
        };

        menu.Show();
    }

    private void ShowDeleteConfirmation(Workout workout)
    {
        AlertDialog.Builder builder = new AlertDialog.Builder(_context);
        builder.SetTitle("Delete Workout");
        builder.SetMessage("Are you sure you want to delete this workout?");
        builder.SetPositiveButton("Delete", (sender, e) =>
        {
            if (_dbHelper.DeleteWorkout(workout.Id))
            {
                Toast.MakeText(_context, "Workout deleted", ToastLength.Short).Show();
                _workouts.Remove(workout);
                NotifyDataSetChanged();
            }
            else
            {
                Toast.MakeText(_context, "Failed to delete workout", ToastLength.Short).Show();
            }
        });
        builder.SetNegativeButton("Cancel", (sender, e) => { });
        builder.Show();
    }
}