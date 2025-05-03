using System;
using System.Collections.Generic;

namespace Gym_Me
{
    [Serializable] // Make the class serializable
    public record WorkoutTimeLog
    {
        public double SetTime { get; set; }
        public double RestTime { get; set; }
    }

    [Serializable] // Make the class serializable
    public record WorkoutSetLog
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public WorkoutTimeLog Time { get; set; }
        public int Reps { get; set; }
    }

    [Serializable] // Make the class serializable
    public record WorkoutLog
    {
        public int Id { get; set; }
        public int WorkoutId { get; set; }
        public List<WorkoutSetLog> Sets { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
