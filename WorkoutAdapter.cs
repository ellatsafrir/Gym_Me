using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Gym_Me
{
    public class WorkoutAdapter : BaseAdapter<Workout>
    {
        private List<Workout> workouts;
        private Context context;
        private DatabaseHelper dbHelper;

        public WorkoutAdapter(Context context, List<Workout> workouts, DatabaseHelper dbHelper)
        {
            this.context = context;
            this.workouts = workouts;
            this.dbHelper = dbHelper;
        }

        public override Workout this[int position] => workouts[position];

        public override int Count => workouts.Count;

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                view = LayoutInflater.From(context).Inflate(Resource.Layout.workout_item, parent, false);
            }

            var workout = workouts[position];

            // Bind views
            TextView workoutNameTextView = view.FindViewById<TextView>(Resource.Id.workoutNameTextView);
            LinearLayout exercisesLayout = view.FindViewById<LinearLayout>(Resource.Id.exercisesLayout);
            TextView exercisesTextView = view.FindViewById<TextView>(Resource.Id.exercisesTextView);

            workoutNameTextView.Text = workout.Id;
            exercisesTextView.Text = string.Join(", ", workout.Exercises);

            // Set visibility toggle
            exercisesLayout.Visibility = Android.Views.ViewStates.Gone;

            view.Click -= ToggleVisibility;
            view.Click += ToggleVisibility;

            void ToggleVisibility(object sender, EventArgs e)
            {
                exercisesLayout.Visibility = exercisesLayout.Visibility == Android.Views.ViewStates.Gone
                    ? Android.Views.ViewStates.Visible
                    : Android.Views.ViewStates.Gone;
            }

            // Long-click for popup menu
            view.LongClick -= ShowPopupMenu;
            view.LongClick += ShowPopupMenu;

            void ShowPopupMenu(object sender, View.LongClickEventArgs e)
            {
                ShowEditPopup(view, workout);
            }

            return view;
        }

        private void ShowEditPopup(View view, Workout workout)
        {
            PopupMenu menu = new PopupMenu(context, view);
            menu.Inflate(Resource.Menu.edit_popup_menu); // Ensure this menu XML has edit and delete options

            menu.MenuItemClick += (sender, e) =>
            {
                if (e.Item.ItemId == Resource.Id.menu_edit)
                {
                    ShowEditDialog(workout);
                }
                else if (e.Item.ItemId == Resource.Id.menu_delete)
                {
                    ConfirmDelete(workout);
                }
            };

            menu.Show();
        }

        private void ConfirmDelete(Workout workout)
        {
            // Confirm deletion dialog
            new AlertDialog.Builder(context)
                .SetTitle("Delete Workout")
                .SetMessage($"Are you sure you want to delete {workout.Id}?")
                .SetPositiveButton("Yes", (sender, e) =>
                {
                    DeleteWorkoutFromDatabase(workout);
                })
                .SetNegativeButton("No", (sender, e) => { /* Do nothing */ })
                .Create()
                .Show();
        }

        private void DeleteWorkoutFromDatabase(Workout workout)
        {
            bool isDeleted = dbHelper.DeleteWorkout(workout.Id); // Ensure this method exists in DatabaseHelper

            if (isDeleted)
            {
                workouts.Remove(workout); // Remove workout from the list
                NotifyDataSetChanged(); // Refresh the adapter to update the ListView
                Toast.MakeText(context, $"{workout.Id} deleted.", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(context, "Failed to delete workout.", ToastLength.Short).Show();
            }
        }

        private void ShowEditDialog(Workout workout)
        {
            var dialogView = LayoutInflater.From(context).Inflate(Resource.Layout.dialog_edit_workout, null);

            EditText workoutNameEditText = dialogView.FindViewById<EditText>(Resource.Id.workoutNameEditText);
            EditText exercisesEditText = dialogView.FindViewById<EditText>(Resource.Id.exercisesEditText);

            workoutNameEditText.Text = workout.Id;
            exercisesEditText.Text = string.Join(", ", workout.Exercises);

            new AlertDialog.Builder(context)
                .SetTitle("Edit Workout")
                .SetView(dialogView)
                .SetPositiveButton("Save", (sender, e) =>
                {
                    string updatedWorkoutName = workoutNameEditText.Text.Trim();
                    string updatedExercises = exercisesEditText.Text.Trim();

                    if (string.IsNullOrEmpty(updatedWorkoutName) || string.IsNullOrEmpty(updatedExercises))
                    {
                        Toast.MakeText(context, "Please provide valid inputs.", ToastLength.Short).Show();
                        return;
                    }

                    workout.Id = updatedWorkoutName;
                    //workout.Exercises = new List<string>(updatedExercises.Split(new[] { ", " }, StringSplitOptions.None));
                    //UpdateWorkoutInDatabase(workout);
                })
                .SetNegativeButton("Cancel", (sender, e) => { /* Do nothing */ })
                .Create()
                .Show();
           
        }

        //private void UpdateWorkoutInDatabase(Workout workout)
        //{
        //    bool isUpdated = dbHelper.UpdateWorkout(workout);

        //    if (isUpdated)
        //    {
        //        NotifyDataSetChanged(); // Refresh the ListView
        //        Toast.MakeText(context, "Workout updated successfully.", ToastLength.Short).Show();
              
        //    }
        //    else
        //    {
        //        Toast.MakeText(context, "Failed to update workout.", ToastLength.Short).Show();
        //    }
        //}



        // Reload the workouts list from the database
        private void ReloadWorkouts()
        {
            
        //   va view = LayoutInflater.From(context).Inflate(Resource.Layout.workout_item, parent, false);
            // Fetch updated workouts from the database
            var updatedWorkoutNames = dbHelper.GetWorkoutsForDate(DateTime.Now.Date);
            workouts.Clear();

            foreach (var workoutName in updatedWorkoutNames)
            {
                var exercises = dbHelper.GetExercisesForWorkout(workoutName);
                workouts.Add(new Workout
                {
                    //Name = workoutName,
                    //Exercises = exercises
                });
            }
        }
    }
}
