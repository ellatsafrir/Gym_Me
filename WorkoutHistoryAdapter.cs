﻿
using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace Gym_Me
{
    public class WorkoutHistoryAdapter : BaseAdapter<Workout>
    {
        private readonly Context context;
        private readonly List<Workout> workouts;
        private readonly DatabaseHelper dbHelper;

        public WorkoutHistoryAdapter(Context context, List<Workout> workouts)
        {
            this.context = context;
            this.workouts = workouts;
            dbHelper = new DatabaseHelper(context);
        }

        public override Workout this[int position] => workouts[position];

        public override int Count => workouts.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView ?? LayoutInflater.From(context).Inflate(Resource.Layout.workout_history_item, parent, false);

            var workout = workouts[position];
            var workoutNameTextView = view.FindViewById<TextView>(Resource.Id.workoutNameTextView);
            var workoutDateTextView = view.FindViewById<TextView>(Resource.Id.workoutDateTextView);
            var exercisesTextView = view.FindViewById<TextView>(Resource.Id.exercisesTextView);

            workoutNameTextView.Text = workout.Name;
            workoutDateTextView.Text = $"Date: {workout.Date.ToString("MMMM dd, yyyy")}";

          
            exercisesTextView.Text = string.Join("\n", workout.Exercises.Select(exercise =>

                    $"Exercise: {exercise}"));

            return view;
        }

      
    }
}
