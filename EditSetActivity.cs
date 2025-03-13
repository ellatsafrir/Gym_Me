//using Android.Content;

//[Activity(Label = "Edit Set")]
//public class EditSetActivity : Activity
//{
//    private EditText repsEditText, weightEditText, restEditText, timeEditText;
//    private Button saveButton;
//    private string originalSetDetails;

//    protected override void OnCreate(Bundle savedInstanceState)
//    {
//        base.OnCreate(savedInstanceState);
//        SetContentView(Resource.Layout.activity_edit_set); // Your EditSet layout

//        repsEditText = FindViewById<EditText>(Resource.Id.repsEditText);
//        weightEditText = FindViewById<EditText>(Resource.Id.weightEditText);
//        restEditText = FindViewById<EditText>(Resource.Id.restEditText);
//        timeEditText = FindViewById<EditText>(Resource.Id.timeEditText);
//        saveButton = FindViewById<Button>(Resource.Id.saveButton);

//        originalSetDetails = Intent.GetStringExtra("setDetails");

//        // Pre-fill the fields with the existing set details
//        var setData = originalSetDetails.Split(',');
//        repsEditText.Text = setData[1].Split(':')[1].Trim();
//        weightEditText.Text = setData[2].Split(':')[1].Trim();
//        restEditText.Text = setData[3].Split(':')[1].Trim().Replace("s", "");
//        timeEditText.Text = setData[4].Split(':')[1].Trim().Replace("s", "");

//        saveButton.Click += (sender, e) =>
//        {
//            // Save the updated set details
//            string updatedSetDetails = $"Exercise: {setData[0].Split(':')[1].Trim()}, Reps: {repsEditText.Text}, Weight: {weightEditText.Text}, Time: {timeEditText.Text}s, Rest: {restEditText.Text}s";

//            // Send the updated set details back to AddWorkoutActivity
//            Intent resultIntent = new Intent();
//            resultIntent.PutExtra("updatedSetDetails", updatedSetDetails);
//            SetResult(Result.Ok, resultIntent);

//            // Close the EditSetActivity
//            Finish();
//        };
//    }
//}
