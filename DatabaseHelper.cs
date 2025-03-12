using System;
using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using Android.Util;

namespace Gym_Me
{
    public class DatabaseHelper : SQLiteOpenHelper
    {
        private const string DatabaseName = "GymMe.db";
        private const int DatabaseVersion = 2;

        // Table and Column names
        private const string TableUsers = "Users";
        private const string ColumnUserId = "Id";
        private const string ColumnEmail = "Email";
        private const string ColumnPassword = "Password";
        private const string ColumnName = "Name";

        private const string TableWorkouts = "Workouts";
        private const string ColumnWorkoutId = "Id";
        private const string ColumnWorkoutName = "Name";
        private const string ColumnWorkoutDate = "Date";

        private const string TableExercises = "Exercises";
        private const string ColumnExerciseId = "Id";
        private const string ColumnExerciseName = "Name";

        private const string TableExerciseSets = "ExerciseSets";
        private const string ColumnExerciseSetId = "Id";
        private const string ColumnSetWorkoutId = "WorkoutId";
        private const string ColumnSetExerciseId = "ExerciseId";
        private const string ColumnRepetitions = "Repetitions";
        private const string ColumnWeight = "Weight";
        private const string ColumnRestTime = "RestTime";

        public DatabaseHelper(Context context)
            : base(context, DatabaseName, null, DatabaseVersion)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            Log.Debug("DatabaseHelper", "Creating tables...");
            string createUsersTable = $@"
                CREATE TABLE {TableUsers} (
                    {ColumnUserId} INTEGER PRIMARY KEY AUTOINCREMENT,
                    {ColumnEmail} TEXT NOT NULL UNIQUE,
                    {ColumnPassword} TEXT NOT NULL,
                    {ColumnName} TEXT NOT NULL
                );";

            string createWorkoutsTable = $@"
                CREATE TABLE {TableWorkouts} (
                    {ColumnWorkoutId} INTEGER PRIMARY KEY AUTOINCREMENT,
                    {ColumnWorkoutName} TEXT NOT NULL,
                    {ColumnWorkoutDate} TEXT NOT NULL
                );";

            string createExercisesTable = $@"
                CREATE TABLE {TableExercises} (
                    {ColumnExerciseId} INTEGER PRIMARY KEY AUTOINCREMENT,
                    {ColumnExerciseName} TEXT NOT NULL
                );";

            string createExerciseSetsTable = $@"
                CREATE TABLE {TableExerciseSets} (
                    {ColumnExerciseSetId} INTEGER PRIMARY KEY AUTOINCREMENT,
                    {ColumnSetWorkoutId} INTEGER NOT NULL,
                    {ColumnSetExerciseId} INTEGER NOT NULL,
                    {ColumnRepetitions} INTEGER,
                    {ColumnWeight} REAL,
                    {ColumnRestTime} INTEGER,
                    FOREIGN KEY ({ColumnSetWorkoutId}) REFERENCES {TableWorkouts}({ColumnWorkoutId}) ON DELETE CASCADE,
                    FOREIGN KEY ({ColumnSetExerciseId}) REFERENCES {TableExercises}({ColumnExerciseId}) ON DELETE CASCADE
                );";

            db.ExecSQL(createUsersTable);
            db.ExecSQL(createWorkoutsTable);
            db.ExecSQL(createExercisesTable);
            db.ExecSQL(createExerciseSetsTable);
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            // Drop the old tables if they exist
            db.ExecSQL($"DROP TABLE IF EXISTS {TableUsers}");
            db.ExecSQL($"DROP TABLE IF EXISTS {TableWorkouts}");
            db.ExecSQL($"DROP TABLE IF EXISTS {TableExercises}");
            db.ExecSQL($"DROP TABLE IF EXISTS {TableExerciseSets}");

            // Recreate the tables with the updated schema
            OnCreate(db);
        }


        // User Management
        public bool AddUser(string email, string password, string name)
        {
            using (var db = WritableDatabase)
            {
                try
                {
                    var contentValues = new ContentValues();
                    contentValues.Put(ColumnEmail, email);
                    contentValues.Put(ColumnPassword, password);
                    contentValues.Put(ColumnName, name);

                    db.InsertOrThrow(TableUsers, null, contentValues);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsEmailRegistered(string email)
        {
            using (var db = ReadableDatabase)
            {
                // Query to check if the email already exists in the Users table
                string query = $"SELECT COUNT(*) FROM {TableUsers} WHERE {ColumnEmail} = ?";

                using (var cursor = db.RawQuery(query, new string[] { email }))
                {
                    // If cursor moves to the next row and the count is greater than 0, the email is already registered
                    cursor.MoveToNext();
                    return cursor.GetInt(0) > 0;
                }
            }
        }

        public bool AuthenticateUser(string email, string password)
        {
            using (var db = ReadableDatabase)
            {
                string query = $@"
                    SELECT * FROM {TableUsers}
                    WHERE {ColumnEmail} = ? AND {ColumnPassword} = ?";

                using (var cursor = db.RawQuery(query, new string[] { email, password }))
                {
                    return cursor.Count > 0;
                }
            }
        }

        public int GetWorkoutIdByName(string workoutName)
        {
            using (var db = ReadableDatabase)
            {
                string query = $"SELECT {ColumnWorkoutId} FROM {TableWorkouts} WHERE {ColumnWorkoutName} = ?";
                using (var cursor = db.RawQuery(query, new string[] { workoutName }))
                {
                    if (cursor.MoveToNext())
                    {
                        return cursor.GetInt(0); // Return the WorkoutId
                    }
                }
            }
            return -1; // Return -1 if workout not found
        }


        // Workout Management
        public long SaveWorkout(string workoutName, DateTime date)
        {
            using (var db = WritableDatabase)
            {
                var contentValues = new ContentValues();
                contentValues.Put(ColumnWorkoutName, workoutName);
                contentValues.Put(ColumnWorkoutDate, date.ToString("yyyy-MM-dd"));

                return db.Insert(TableWorkouts, null, contentValues);
            }
        }

        public long SaveExercise(string exerciseName)
        {
            using (var db = WritableDatabase)
            {
                var contentValues = new ContentValues();
                contentValues.Put(ColumnExerciseName, exerciseName);

                return db.Insert(TableExercises, null, contentValues);
            }
        }

        public void SaveExerciseSet(int workoutId, int exerciseId, int repetitions, double weight, int restTime)
        {
            using (var db = WritableDatabase)
            {
                var contentValues = new ContentValues();
                contentValues.Put(ColumnSetWorkoutId, workoutId);
                contentValues.Put(ColumnSetExerciseId, exerciseId);
                contentValues.Put(ColumnRepetitions, repetitions);
                contentValues.Put(ColumnWeight, weight);
                contentValues.Put(ColumnRestTime, restTime);

                db.Insert(TableExerciseSets, null, contentValues);
            }
        }

        public List<string> GetAllWorkouts()
        {
            var workouts = new List<string>();
            using (var db = ReadableDatabase)
            {
                string query = $"SELECT {ColumnWorkoutName} FROM {TableWorkouts}";

                using (var cursor = db.RawQuery(query, null))
                {
                    while (cursor.MoveToNext())
                    {
                        workouts.Add(cursor.GetString(0)); // Get workout name
                    }
                }
            }
            return workouts;
        }


        public int GetExerciseIdByName(string exerciseName)
        {
            using (var db = ReadableDatabase)
            {
                string query = $"SELECT {ColumnExerciseId} FROM {TableExercises} WHERE {ColumnExerciseName} = ?";
                using (var cursor = db.RawQuery(query, new string[] { exerciseName }))
                {
                    if (cursor.MoveToNext())
                    {
                        return cursor.GetInt(0);  // Return the ExerciseId
                    }
                    return -1; // Exercise not found
                }
            }
        }

        public List<ExerciseSet> GetExercisesForWorkout(int workoutId)
        {
            using (var db = ReadableDatabase)
            {
                string query = "SELECT * FROM ExerciseSets WHERE WorkoutId = ?";
                var cursor = db.RawQuery(query, new string[] { workoutId.ToString() });

                var exercises = new List<ExerciseSet>();
                while (cursor.MoveToNext())
                {
                    var exerciseSet = new ExerciseSet
                    {
                        Id = cursor.GetInt(cursor.GetColumnIndex(ColumnExerciseSetId)),
                        WorkoutId = cursor.GetInt(cursor.GetColumnIndex(ColumnSetWorkoutId)),
                        ExerciseId = cursor.GetInt(cursor.GetColumnIndex(ColumnSetExerciseId)),
                        Repetitions = cursor.GetInt(cursor.GetColumnIndex(ColumnRepetitions)),
                        Weight = cursor.GetDouble(cursor.GetColumnIndex(ColumnWeight)),
                        RestTime = cursor.GetInt(cursor.GetColumnIndex(ColumnRestTime))
                    };
                    exercises.Add(exerciseSet);
                }
                return exercises;
            }
        }




        public bool DeleteWorkout(int workoutId)
        {
            try
            {
                using (var db = WritableDatabase)
                {
                    int rowsAffected = db.Delete(TableWorkouts, $"{ColumnWorkoutId} = ?", new string[] { workoutId.ToString() });
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error("DatabaseHelper", ex.Message);
                return false;
            }
        }

        public List<Workout> GetWorkoutsForDate(DateTime date)
        {
            using (var db = ReadableDatabase)
            {
                string query = "SELECT * FROM Workouts WHERE Date = ?";
                var cursor = db.RawQuery(query, new string[] { date.ToString("yyyy-MM-dd") });

                var workouts = new List<Workout>();
                while (cursor.MoveToNext())
                {
                    var workout = new Workout
                    {
                        Id = cursor.GetInt(cursor.GetColumnIndex("Id")),
                        Name = cursor.GetString(cursor.GetColumnIndex("Name")),
                        Date = DateTime.Parse(cursor.GetString(cursor.GetColumnIndex("Date")))
                    };
                    workouts.Add(workout);
                }
                return workouts;
            }
        }
        public Exercise GetExerciseById(int exerciseId)
        {
            using (var db = ReadableDatabase)
            {
                string query = $"SELECT * FROM {TableExercises} WHERE {ColumnExerciseId} = ?";
                using (var cursor = db.RawQuery(query, new string[] { exerciseId.ToString() }))
                {
                    if (cursor.MoveToNext())
                    {
                        return new Exercise
                        {
                            Id = cursor.GetInt(cursor.GetColumnIndex(ColumnExerciseId)),
                            Description = cursor.GetString(cursor.GetColumnIndex(ColumnExerciseName))
                        };
                    }
                }
            }
            return null; // Return null if no exercise found
        }


    }
}
