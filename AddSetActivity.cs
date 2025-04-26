using Android.App;
using Android.OS;
using Android.Widget;
using Gym_Me;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Lights;
using Newtonsoft.Json;

namespace Gym_Me
{
    [Activity(Label = "AddSetActivity" )]
    public class AddSetActivity : Activity
    {
        private ExcersizeList excersizeList;
        private LinearLayout exercisesListContainer;
        private string selectedExerciseName = null;
        Button back;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_add_set); // Your Add Set XML layout

            // Load exercises from CSV
            excersizeList = ExcersizeList.Instance;
            excersizeList.LoadCsvFromAssets(this);

            // References to UI elements
            exercisesListContainer = FindViewById<LinearLayout>(Resource.Id.exercisesListContainer);
            var repsEditText = FindViewById<EditText>(Resource.Id.repsEditText);
            var weightEditText = FindViewById<EditText>(Resource.Id.weightEditText);
            var timeEditText = FindViewById<EditText>(Resource.Id.timeSetEditText);
            var restEditText = FindViewById<EditText>(Resource.Id.restEditText);                       
            var saveSetButton = FindViewById<Button>(Resource.Id.saveSetButton);
            back = FindViewById<Button>(Resource.Id.backButton);

            back.Click += (s, e) =>
            {
                var intent = new Intent(this, typeof(AddWorkoutActivity));
                StartActivityForResult(intent, 1); // Request code 1
            };

            // Populate exercise list dynamically
            PopulateExerciseList();
            saveSetButton.Click += (s, e) =>
            {
                // Check I filled all fields
                if (string.IsNullOrEmpty(selectedExerciseName))
                {
                    Toast.MakeText(this, "Please select an exercise!", ToastLength.Short).Show();
                    return;
                }

                Exercise excersize;
                try
                {
                    excersize = new Exercise
                    {
                        Excersize = excersizeList.GetExcersizeData(selectedExerciseName), // TODO: link to actual excersize
                        Type = "Type",
                        Reps = Convert.ToInt32(repsEditText.Text),
                        Weight = Convert.ToDouble(weightEditText.Text),
                        SetTime = Convert.ToDouble(timeEditText.Text),
                        RestTime = Convert.ToDouble(restEditText.Text)
                    };
                }
                catch (Exception)
                {
                    Toast.MakeText(this, "Please fill in all fields!", ToastLength.Short).Show();
                    return;
                }
                
                // Send data back to AddWorkoutActivity
                Intent resultIntent = new Intent();
                resultIntent.PutExtra("Excersize", JsonConvert.SerializeObject(excersize));
                SetResult(Result.Ok, resultIntent);


                // Close this activity
                Finish();
            };
        }

        private void PopulateExerciseList()
        {
            foreach (var exercise in excersizeList.excersizes)
            {
                // Create a TextView for each exercise
                var textView = new TextView(this)
                {
                    Text = exercise.Name,
                    TextSize = 16,
                };
                textView.SetPadding(8, 8, 8, 8); // Use SetPadding instead of Padding
                textView.SetBackgroundResource(Android.Resource.Drawable.ListSelectorBackground);

                // Handle click event to select exercise
                textView.Click += (s, e) =>
                {
                    // Deselect all other exercises
                    for (int i = 0; i < exercisesListContainer.ChildCount; i++)
                    {
                        var child = exercisesListContainer.GetChildAt(i);
                        if (child is TextView tv)
                        {
                            tv.SetBackgroundColor(Android.Graphics.Color.Transparent);
                        }
                    }

                    // Highlight the selected exercise
                    textView.SetBackgroundColor(Android.Graphics.Color.LightGray);

                    // Store the selected exercise name
                    selectedExerciseName = exercise.Name;
                };

                // Add the TextView to the container
                exercisesListContainer.AddView(textView);
            }
        }
    }
}
