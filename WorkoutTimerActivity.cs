using Android.Content;

namespace Gym_Me;

[Activity(Label = "Workout Timer")]
public class WorkoutTimerActivity : Activity
{
    TextView exerciseNameText, timerText, setCounterText, weightText, repsText;
    Button nextSetButton;
    List<Exercise> exercises;
    int workoutId, currentSet = 1, totalSets = 3, exerciseIndex = 0;
    long timeLeft = 45000;
    CountDownTimer countDown;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_workout_timer);

        exerciseNameText = FindViewById<TextView>(Resource.Id.exerciseNameText);
        timerText = FindViewById<TextView>(Resource.Id.timerText);
        setCounterText = FindViewById<TextView>(Resource.Id.setCounterText);
        weightText = FindViewById<TextView>(Resource.Id.weightText);
        repsText = FindViewById<TextView>(Resource.Id.repsText);
        nextSetButton = FindViewById<Button>(Resource.Id.nextSetButton);

        workoutId = Intent.GetIntExtra("WorkoutId", -1);
        exerciseIndex = Intent.GetIntExtra("ExerciseIndex", 0);

        exercises = new DatabaseHelper(this).GetExercisesForWorkout(workoutId);
        LoadExercise();

        nextSetButton.Click += (s, e) => MoveToNextSet();
    }

    void LoadExercise()
    {
        if (exerciseIndex >= exercises.Count)
        {
            FinishWorkout();
            return;
        }

        var ex = exercises[exerciseIndex];
        var data = ExcersizeList.Instance.GetExcersizeData(ex.ExcersizeId);

        exerciseNameText.Text = data?.Name ?? "Exercise";
        weightText.Text = ex.Weight.ToString();
        repsText.Text = ex.Reps.ToString();
        setCounterText.Text = $"Set {currentSet} of {totalSets}";

        StartTimer();
    }

    void StartTimer()
    {
        countDown = new CountDownTimer(timeLeft, 1000)
        {
            Tick = millis => timerText.Text = $"00:{millis / 1000:00}",
            Finish = () => MoveToNextSet()
        };
        countDown.Start();
    }

    void MoveToNextSet()
    {
        countDown?.Cancel();

        new DatabaseHelper(this).InsertWorkoutLog(new WorkoutLog
        {
            WorkoutId = workoutId,
            ExerciseId = exercises[exerciseIndex].ExcersizeId,
            SetNumber = currentSet,
            Weight = exercises[exerciseIndex].Weight,
            Reps = exercises[exerciseIndex].Reps,
            Skipped = false,
            Timestamp = DateTime.Now
        });

        if (++currentSet > totalSets)
        {
            currentSet = 1;
            exerciseIndex++;
        }

        LoadExercise();
    }

    void FinishWorkout()
    {
        var intent = new Intent(this, typeof(WorkoutSummaryActivity));
        intent.PutExtra("WorkoutId", workoutId);
        StartActivity(intent);
        Finish();
    }

    public class CountDownTimer : Android.OS.CountDownTimer
    {
        public Action<long> Tick;
        public Action Finish;

        public CountDownTimer(long millisInFuture, long interval) : base(millisInFuture, interval) { }

        public override void OnTick(long millisUntilFinished) => Tick?.Invoke(millisUntilFinished);
        public override void OnFinish() => Finish?.Invoke();
    }
}
