using Android.Content;
using Newtonsoft.Json;


namespace Gym_Me
{
    [Activity(Label = "AddWorkoutActivity")]
    public class AddWorkoutActivity : Activity
    {
        private EditText workoutNameEditText;
        private Button addSet;
        private Button selectDateButton;
        private Button saveWorkoutButton;
        ListView setsListView;
        private DateTime selectedDate;
        private DatabaseHelper _databaseHelper;
        private bool isEditing = false;
        private string originalWorkoutName = "";
        private List<Exercise> setDetailsList = new List<Exercise>();
        private ArrayAdapter<string> adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_workout);

            // Initialize UI elements
            workoutNameEditText = FindViewById<EditText>(Resource.Id.workoutNameEditText);
            addSet = FindViewById<Button>(Resource.Id.addSetButton);
            selectDateButton = FindViewById<Button>(Resource.Id.selectDateButton);
            saveWorkoutButton = FindViewById<Button>(Resource.Id.saveWorkoutButton);
            setsListView = FindViewById<ListView>(Resource.Id.setsListView);

            _databaseHelper = new DatabaseHelper(this);
            adapter = new ArrayAdapter<string>(
                this, 
                Android.Resource.Layout.SimpleListItem1,
                setDetailsList.Select(set => set.ToString()).ToList()
            );

            setsListView.Adapter = adapter;

            if (Intent.HasExtra("isEditing") && Intent.GetBooleanExtra("isEditing", false))
            {
                isEditing = true;
                originalWorkoutName = Intent.GetStringExtra("workoutName");
                string exercises = Intent.GetStringExtra("exercises");
                workoutNameEditText.Text = originalWorkoutName;
            }

            selectDateButton.Click += (sender, e) => ShowDatePickerDialog();

            //saveWorkoutButton.Click += (sender, e) => 

            addSet.Click += (s, e) =>
            {
                var intent = new Intent(this, typeof(AddSetActivity));
                StartActivityForResult(intent, 1); // Request code 1
            };

            saveWorkoutButton.Click += (sender, e) =>
            {
                SaveWorkout();
            };

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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1 && resultCode == Result.Ok && data != null)
            {
                // Get the JSON string of the Exercise object
                string exerciseJson = data.GetStringExtra("Excersize");

                if (!string.IsNullOrEmpty(exerciseJson))
                {
                    try
                    {
                        // Deserialize the JSON string to an Exercise object
                        Exercise exercise = JsonConvert.DeserializeObject<Exercise>(exerciseJson);

                        // Format the set details using the Exercise object
                        string setDetails = $"Exercise: {exercise}";
                        Toast.MakeText(this, setDetails, ToastLength.Short).Show();

                        // Add the new set to the list
                        setDetailsList.Add(exercise);

                        // Refresh the ListView by adding to the adapter and notifying the change
                        RunOnUiThread(() =>
                        {
                            adapter.Clear();
                            adapter.AddAll(setDetailsList.Select(ex => ex.ToString()).ToList());
                            adapter.NotifyDataSetChanged(); // Notify that the data has changed
                            setsListView.RefreshDrawableState(); // Optional: extra refresh if needed
                        });

                        Console.WriteLine($"Updated List: {string.Join(", ", setDetailsList)}");
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "Failed to deserialize exercise data", ToastLength.Short).Show();
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }
        private void SaveWorkout()
        {
            string workoutName = workoutNameEditText.Text.Trim();

            // Verify and protect against invalid workout settings.
            if (string.IsNullOrEmpty(workoutName))
            {
                Toast.MakeText(this, "Please enter a workout name.", ToastLength.Short).Show();
                return;
            }

            if (selectedDate == DateTime.MinValue)
            {
                Toast.MakeText(this, "Please select a date.", ToastLength.Short).Show();
                return;
            }

            // Create Workout object
            Workout workout = new Workout
            {
                Name = workoutName,
                Date = selectedDate
            };
            // Insert workout into database
            long workoutId = _databaseHelper.SaveWorkout(workout);
            if (workoutId == -1)
            {
                Toast.MakeText(this, "Failed to save workout.", ToastLength.Short).Show();
                return;
            }

            // Save ExerciseSets for this workout
            foreach (var excersize in setDetailsList)
            {
                // Save ExerciseSet
                excersize.WorkoutId = (int)workoutId;
                if (_databaseHelper.SaveExerciseSet(excersize) < 0)
                {
                    Toast.MakeText(this, "Failed to save exersize sets", ToastLength.Short).Show();
                }
            }

            Toast.MakeText(this, "Workout saved successfully!", ToastLength.Short).Show();
            SetResult(Result.Ok);
            Finish();
        }


    }
}
