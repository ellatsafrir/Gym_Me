﻿using SQLite;


namespace Gym_Me
{
    [Table("Workouts")]
    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime Date { get; set; }

        // Not stored directly in the table, but useful for querying sets
        [Ignore]
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
