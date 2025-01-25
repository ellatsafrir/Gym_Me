using Android.App;
using Android.Views;
using Android.Widget;
using Gym_Me;
using System.Collections.Generic;

public class GymAdapter : BaseAdapter<GymData>
{
    private readonly List<GymData> _excersizes;
    private readonly Activity _context;

    public GymAdapter(Activity context, List<GymData> excersizes) : base()
    {
        _context = context;
        _excersizes = excersizes;
    }

    public override GymData this[int position] => _excersizes[position];

    public override int Count => _excersizes.Count;

    public override long GetItemId(int position) => position;

    public override View GetView(int position, View convertView, ViewGroup parent)
    {
        var gymData = _excersizes[position];
        var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.item_gym, parent, false);

        var nameTextView = view.FindViewById<TextView>(Resource.Id.nameTextView);
        var descriptionTextView = view.FindViewById<TextView>(Resource.Id.descriptionTextView);

        nameTextView.Text = gymData.Name;
        descriptionTextView.Text = gymData.Description;

        return view;
    }
}
