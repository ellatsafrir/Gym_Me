using SQLite;


namespace Gym_Me
{

    [Table("WorkoutLog")]
    public class WorkoutLog
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int SetNumber { get; set; }
        public double Weight { get; set; }
        public int Reps { get; set; }
        public bool Skipped { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
