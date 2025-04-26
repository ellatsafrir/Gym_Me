//using Android.Views;
//using Gym_Me;

//public class SetsAdapter : RecyclerView.Adapter
//{
//    private List<ExerciseSet> _sets;

//    public SetsAdapter(List<ExerciseSet> sets)
//    {
//        _sets = sets;
//    }

//    public override int ItemCount => _sets.Count;

//    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
//    {
//        var set = _sets[position];
//        var setViewHolder = holder as SetViewHolder;

//        setViewHolder.SetDescription.Text = set.Description;
//        setViewHolder.RepsText.Text = set.Reps.ToString();
//        setViewHolder.WeightText.Text = set.Weight.ToString();

//        setViewHolder.EditButton.Click += (sender, e) =>
//        {
//            // Open an edit dialog or a new activity to edit the set
//            EditSet(position);
//        };

//        setViewHolder.DeleteButton.Click += (sender, e) =>
//        {
//            // Delete the set from the list and notify the adapter
//            _sets.RemoveAt(position);
//            NotifyItemRemoved(position);
//        };
//    }

//    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
//    {
//        View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SetItem, parent, false);
//        return new SetViewHolder(itemView);
//    }

//    // Get updated sets after edits
//    public List<ExerciseSet> GetUpdatedSets() => _sets;

//    // Handle the editing logic (this could pop up a dialog or open another activity)
//    private void EditSet(int position)
//    {
//        var set = _sets[position];
//        // Open dialog or activity to edit this set
//    }
//}

//public class SetViewHolder : RecyclerView.ViewHolder
//{
//    public TextView SetDescription { get; set; }
//    public TextView RepsText { get; set; }
//    public TextView WeightText { get; set; }
//    public Button EditButton { get; set; }
//    public Button DeleteButton { get; set; }

//    public SetViewHolder(View itemView) : base(itemView)
//    {
//        SetDescription = itemView.FindViewById<TextView>(Resource.Id.setDescription);
//        RepsText = itemView.FindViewById<TextView>(Resource.Id.repsText);
//        WeightText = itemView.FindViewById<TextView>(Resource.Id.weightText);
//        EditButton = itemView.FindViewById<Button>(Resource.Id.editSetButton);
//        DeleteButton = itemView.FindViewById<Button>(Resource.Id.deleteSetButton);
//    }
//}
