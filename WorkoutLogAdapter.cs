using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace Gym_Me
{
    public class WorkoutLogAdapter : BaseAdapter<WorkoutLog>
    {
        private readonly Activity context;
        private readonly List<WorkoutLog> items;

        public WorkoutLogAdapter(Activity context, List<WorkoutLog> items)
        {
            this.context = context;
            this.items = items;
        }

        public override WorkoutLog this[int position] => items[position];

        public override int Count => items.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.workout_log_item, parent, false);

            // Get the workout log
            var workoutLog = items[position];

            // Bind the Timestamp and WorkoutId to the respective TextViews
            var timestampTextView = view.FindViewById<TextView>(Resource.Id.timestampTextView);
            var workoutIdTextView = view.FindViewById<TextView>(Resource.Id.workoutIdTextView);

            timestampTextView.Text = workoutLog.Timestamp.ToString("G"); // Format the timestamp
            workoutIdTextView.Text = $"Workout ID: {workoutLog.WorkoutId}";

            // Handle Sets
            var setsContainer = view.FindViewById<LinearLayout>(Resource.Id.setsContainer);
            setsContainer.RemoveAllViews(); // Clear any existing views

            foreach (var set in workoutLog.Sets)
            {
                var setView = context.LayoutInflater.Inflate(Resource.Layout.workout_set_item, null);

                var repsTextView = setView.FindViewById<TextView>(Resource.Id.repsTextView);
                var setTimeTextView = setView.FindViewById<TextView>(Resource.Id.setTimeTextView);
                var restTimeTextView = setView.FindViewById<TextView>(Resource.Id.restTimeTextView);

                repsTextView.Text = $"Reps: {set.Reps}";
                setTimeTextView.Text = $"Set Time: {set.Time.SetTime} seconds";
                restTimeTextView.Text = $"Rest Time: {set.Time.RestTime} seconds";

                setsContainer.AddView(setView); // Add the set view to the container
            }

            return view;
        }
    }
}
