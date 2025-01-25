using Android.Widget;

namespace Gym_Me;

[Activity(Label = "SeeActivity", MainLauncher = true)]
public class SeeActivity : Activity
{
    private ListView _listView;
    private GymAdapter _adapter;
    private ExcersizeList _excersizeList;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.See);

        _listView = FindViewById<ListView>(Resource.Id.seelistView);
        _excersizeList = new ExcersizeList();

        // Load exercises from CSV
        _excersizeList.LoadCsvFromAssets(this);

        // Set the adapter to the ListView
        _adapter = new GymAdapter(this, _excersizeList.excersizes);
        _listView.Adapter = _adapter;

        // Handle item click (if you want to do something when an item is clicked)
        _listView.ItemClick += (sender, e) =>
        {
            var clickedItem = _excersizeList.excersizes[e.Position];
            Toast.MakeText(this, $"Clicked: {clickedItem.Name}", ToastLength.Short).Show();
        };
    }
}