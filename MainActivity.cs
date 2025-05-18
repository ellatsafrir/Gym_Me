
using Android.Content;
using Android.Util;


namespace Gym_Me
{
    [Activity(Label = "MainActivity", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ListView workoutListView;
        private List<Workout> workoutList;
        private DatabaseHelper dbHelper;
        private Button history;
        private Button infoMuscles;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            if (!IsUserLoggedIn())
            {
                StartActivity(new Intent(this, typeof(LoginActivity)));
                Finish();
                return;
            }

            history = FindViewById<Button>(Resource.Id.historyPage);
            workoutListView = FindViewById<ListView>(Resource.Id.workoutListView);
            infoMuscles = FindViewById<Button>(Resource.Id.seeMusclesInfo);
            var addWorkoutButton = FindViewById<Button>(Resource.Id.addWorkoutButton);
            dbHelper = new DatabaseHelper(this);
            LoadTodayWorkouts();

            addWorkoutButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(AddWorkoutActivity));
                StartActivityForResult(intent, 1);
            };

            history.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(WorkoutHistoryActivity));
                StartActivityForResult(intent, 1);
            };

            workoutListView.ItemLongClick += (sender, e) =>
            {
                ShowPopupMenu(e.Position);
            };

            infoMuscles.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(SeeActivity));
                StartActivityForResult(intent, 1);
            };
        }

        private void ShowPopupMenu(int position)
        {
            var popupMenu = new PopupMenu(this, workoutListView.GetChildAt(position));
            popupMenu.MenuInflater.Inflate(Resource.Menu.edit_popup_menu, popupMenu.Menu);

            popupMenu.MenuItemClick += (s, args) =>
            {
                var selectedWorkout = workoutList[position];
                 if (args.Item.ItemId == Resource.Id.menu_delete)
                {
                    DeleteWorkout(selectedWorkout);
                }
                else if (args.Item.ItemId == Resource.Id.menu_play)
                {
                    // Open the workout details activity
                    var intent = new Intent(this, typeof(WorkoutListActivity));

                    // Optional: You can pass the workout details to the new activity
                    intent.PutExtra("workoutTitle", selectedWorkout.Name);  // Assuming the workout has a Title property
                    intent.PutExtra("workoutId", selectedWorkout.Id);        // Assuming you have an ID for each workout

                    StartActivity(intent);
                }
            };
            popupMenu.Show();
        }

        private void EditWorkout(Workout workout)
        {
            Intent intent = new Intent(this, typeof(AddWorkoutActivity));
            intent.PutExtra("WorkoutId", workout.Id);
            StartActivityForResult(intent, 1);
        }

        private void DeleteWorkout(Workout workout)
        {
            dbHelper.DeleteWorkout(workout.Id);
            LoadTodayWorkouts();
            Toast.MakeText(this, "Workout deleted", ToastLength.Short).Show();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1 && resultCode == Result.Ok)
            {
                LoadTodayWorkouts();
            }
        }

        private void LoadTodayWorkouts()
        {
            try
            {
                DateTime today = DateTime.Now.Date;

                // Fetch today's workouts
                var workouts = dbHelper.GetWorkoutsForDate(today);
                if (workouts == null || workouts.Count == 0)
                {
                    Toast.MakeText(this, "No workouts scheduled for today.", ToastLength.Short).Show();
                    workoutList = new List<Workout>();
                }
                else
                {
                    workoutList = new List<Workout>();
                    foreach (var workout in workouts)
                    {
                        // Fetch exercises for each workout
                        var exercises = dbHelper.GetExercisesForWorkout(workout.Id) ?? new List<Exercise>();
                        workout.Exercises = exercises;  // Set the exercises in the workout

                        // Log the exercises for debugging
                        foreach (var exercise in exercises)
                        {
                            Log.Debug("ExerciseSets", $"ExerciseSet Id: {exercise.Id}, WorkoutId: {exercise.WorkoutId}, ExerciseId: {exercise.ExcersizeId}, Reps: {exercise.Reps}, Weight: {exercise.Weight}, RestTime: {exercise.RestTime}");
                        }

                        workoutList.Add(workout);
                    }
                }

                // Bind the workout list to the adapter
                var adapter = new WorkoutAdapter(this, workoutList, dbHelper);
                workoutListView.Adapter = adapter;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Failed to load workouts: " + ex.Message, ToastLength.Long).Show();
            }
        }

        private bool IsUserLoggedIn()
        {
            var preferences = GetSharedPreferences("GymMePreferences", FileCreationMode.Private);
            return !string.IsNullOrEmpty(preferences.GetString("userEmail", null));
        }
    }
}