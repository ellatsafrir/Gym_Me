using SQLite;

namespace Gym_Me
{
    [Table("ExerciseSet")]
    public class ExerciseSet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int WorkoutId { get; set; } // Foreign key to Workout
        public int ExerciseId { get; set; } // Foreign key to Exercise

        public int Repetitions { get; set; }
        public double Weight { get; set; } // in kilograms
        public int RestTime { get; set; } // in seconds

        // Optional: Ignored in DB but useful for app logic
        [Ignore]
        public Exercise Exercise { get; set; }
    }
}
