using System;
using System.Collections.Generic;
using SQLite;

namespace Gym_Me
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }                 // Primary Key

        [Unique, NotNull]
        public string Email { get; set; }           // User Email (unique)

        [NotNull]
        public string Password { get; set; }        // User Password (store hashed for security)

        [NotNull]
        public string Name { get; set; }            // User's Name

        // Optional: Ignored in DB but useful for app logic
        [Ignore]
        public List<Workout> Workouts { get; set; } = new List<Workout>();
    }
}
