using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Gym_Me
{
    [Activity(Label = "AddWorkoutActivity")]
    public class AddWorkoutActivity : Activity
    {
        private EditText workoutNameEditText;
        private EditText exercisesEditText;
        private Button selectDateButton;
        private Button saveWorkoutButton;
        private DateTime selectedDate;
        private DatabaseHelper _databaseHelper;
        private bool isEditing = false;
        private string originalWorkoutName = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_workout);

            // Initialize UI elements
            workoutNameEditText = FindViewById<EditText>(Resource.Id.workoutNameEditText);
            exercisesEditText = FindViewById<EditText>(Resource.Id.exercisesEditText);
            selectDateButton = FindViewById<Button>(Resource.Id.selectDateButton);
            saveWorkoutButton = FindViewById<Button>(Resource.Id.saveWorkoutButton);

            _databaseHelper = new DatabaseHelper(this);

            if (Intent.HasExtra("isEditing") && Intent.GetBooleanExtra("isEditing", false))
            {
                isEditing = true;
                originalWorkoutName = Intent.GetStringExtra("workoutName");
                string exercises = Intent.GetStringExtra("exercises");
                workoutNameEditText.Text = originalWorkoutName;
                exercisesEditText.Text = exercises;
            }

            selectDateButton.Click += (sender, e) => ShowDatePickerDialog();

            saveWorkoutButton.Click += (sender, e) => SaveWorkout();
        }

        private void ShowDatePickerDialog()
        {
            DatePickerDialog datePickerDialog = new DatePickerDialog(this, (sender, e) =>
            {
                selectedDate = new DateTime(e.Year, e.Month + 1, e.DayOfMonth); // Months are 0-based
                selectDateButton.Text = selectedDate.ToString("yyyy-MM-dd");
            }, DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day);

            datePickerDialog.Show();
        }

        private void SaveWorkout()
        {
            string workoutName = workoutNameEditText.Text.Trim();
            string exercisesInput = exercisesEditText.Text.Trim();

            if (string.IsNullOrEmpty(workoutName) || string.IsNullOrEmpty(exercisesInput))
            {
                Toast.MakeText(this, "Please enter workout name and exercises.", ToastLength.Short).Show();
                return;
            }

            if (selectedDate == DateTime.MinValue)
            {
                Toast.MakeText(this, "Please select a date.", ToastLength.Short).Show();
                return;
            }

            var exercises = new List<string>(exercisesInput.Split(','));

            for (int i = 0; i < exercises.Count; i++)
            {
                exercises[i] = exercises[i].Trim();
            }

            try
            {
                if (isEditing)
                {
                    _databaseHelper.UpdateWorkout(originalWorkoutName, workoutName, exercises, selectedDate);
                }
                else
                {
                    _databaseHelper.SaveWorkout(workoutName, exercises, selectedDate, _databaseHelper.WritableDatabase);
                }

                Intent resultIntent = new Intent();
                SetResult(Result.Ok, resultIntent);
                Toast.MakeText(this, "Workout saved successfully!", ToastLength.Short).Show();
                Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, $"Error saving workout: {ex.Message}", ToastLength.Short).Show();
            }
        }
    }
}
