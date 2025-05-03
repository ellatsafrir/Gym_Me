using SQLite;

namespace Gym_Me
{
    [Table("WorkoutLogs")]
    public class WorkoutLogTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int WorkoutId { get; set; }

        public DateTime Timestamp { get; set; }
    }

    [Table("WorkoutSetLogs")]
    public class WorkoutSetLogTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int WorkoutLogId { get; set; } // Foreign key

        public int ExerciseId { get; set; }

        public double SetTime { get; set; }

        public double RestTime { get; set; }

        public int Reps { get; set; }
    }
}
