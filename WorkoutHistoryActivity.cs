using Android.Content;
using Gym_Me;

[Activity(Label = "Workout History")]
public class WorkoutHistoryActivity : Activity
{
    private ListView workoutsListView;
    private List<Workout> workouts;
    private DatabaseHelper dbHelper;
    private WorkoutHistoryAdapter adapter;
    private Button back;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_workout_history);

        workoutsListView = FindViewById<ListView>(Resource.Id.workoutsListView);
        back = FindViewById<Button>(Resource.Id.backButton2);
        dbHelper = new DatabaseHelper(this);

        back.Click += (s, e) =>
        {
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        };

        LoadWorkouts();

        //workoutsListView.ItemClick += WorkoutsListView_ItemClick;
        workoutsListView.ItemLongClick += WorkoutsListView_ItemLongClick;

    }

    private void LoadWorkouts()
    {
        workouts = dbHelper.GetAllWorkoutsWithDetails(); // Assume this returns List<Workout> with exercises populated
        adapter = new WorkoutHistoryAdapter(this, workouts);
        workoutsListView.Adapter = adapter;
    }


    private void WorkoutsListView_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
    {
        var selectedWorkout = workouts[e.Position];

        PopupMenu popup = new PopupMenu(this, workoutsListView.GetChildAt(e.Position));
        popup.MenuInflater.Inflate(Resource.Menu.edit_popup_menu, popup.Menu);

        popup.MenuItemClick += (s, args) =>
        {
            if (args.Item.ItemId == Resource.Id.menu_edit)
            {
                var editIntent = new Intent(this, typeof(AddWorkoutActivity));
                editIntent.PutExtra("WorkoutId", selectedWorkout.Id);
                StartActivity(editIntent);
            }
            else if (args.Item.ItemId == Resource.Id.menu_delete)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Delete Workout");
                alert.SetMessage("Are you sure you want to delete this workout?");
                alert.SetPositiveButton("Delete", (senderAlert, argsAlert) =>
                {
                    dbHelper.DeleteWorkout(selectedWorkout.Id);
                    workouts.RemoveAt(e.Position);
                    adapter.NotifyDataSetChanged();
                    Toast.MakeText(this, "Workout deleted", ToastLength.Short).Show();
                });
                alert.SetNegativeButton("Cancel", (senderAlert, argsAlert) => { });
                alert.Show();
            }
        };
        popup.Show();
    }

    
}



