using System;
using System.Collections.Generic;
using Android.Content;
using Android.Database.Sqlite;
using Android.Telecom;
using Android.Util;
using Java.Sql;
using SQLite;

namespace Gym_Me
{
    public class DatabaseHelper : SQLiteOpenHelper
    {
        private const string DatabaseName = "GymMe.db";
        //string dbPath = Path.Combine(
        //    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
        //    DatabaseName
        //);
        string dbPath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            DatabaseName
        );
        // 
        private const int DatabaseVersion = 8;

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
        private const string ColumnSetTime = "SetTime";
        private const string ColumnRestTime = "RestTime";

        public DatabaseHelper(Context context)
            : base(context, DatabaseName, null, DatabaseVersion)
        {}

        public override void OnCreate(SQLiteDatabase db)
        {
            Log.Debug("DatabaseHelper", "Creating tables...");
            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.CreateTable<User>();
                connection.CreateTable<Workout>();
                connection.CreateTable<Exercise>();
            }
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            // Drop the old tables if they exist
            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.DropTable<User>();
                connection.DropTable<Workout>();
                connection.DropTable<Exercise>();
            }

            // Recreate the tables with the updated schema
            OnCreate(db);
        }
        public List<string> GetAllWorkouts()
        {
            var workoutsDisp = new List<string>();

            using (var connection = new SQLiteConnection(dbPath))
            {
                var workouts = connection.Table<Workout>()
                    .OrderByDescending(w => w.Date)
                    .ToList();

                foreach(var workout in workouts)
                {
                    var exercises = GetExercisesForWorkout(workout.Id);
                    string exerciseDetails = "";
                    foreach (var exercise in exercises)
                    {
                        //exerciseDetails += exerciseSet.ExcersizeId + "\n";
                        exerciseDetails += exercise.ToString() + "\n";
                    }
                    // Combine name, date, and exercises
                    string workoutText = $"{workout.Name} - {workout.Date}\nExercises: {exerciseDetails}";
                    workoutsDisp.Add(workoutText);
                }
            }
            return workoutsDisp;
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
        public long SaveWorkout(Workout workout)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                return db.Insert(workout);
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

        //public void SaveExerciseSet(int workoutId, int exerciseId, int repetitions, double weight, double restTime)
        public long SaveExerciseSet(Exercise excersize)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                return db.Insert(excersize);
            }
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
        public List<Exercise> GetExercisesForWorkout(int workoutId)  // TODO: Maybe change Excersize:SetBase to ExersizeSet
        {
            List<Exercise> exercises;
            using (var connection = new SQLiteConnection(dbPath))
            {
                exercises = connection.Table<Exercise>()
                    .Where(e => e.WorkoutId == workoutId)
                    .ToList();
            }
            return exercises;
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

        public bool ClearDatabase()
        {
            using (var db = WritableDatabase)
            {
                db.BeginTransaction();
                try
                {
                    db.Delete(TableExerciseSets, null, null);
                    db.Delete(TableExercises, null, null);
                    db.Delete(TableWorkouts, null, null);
                    db.Delete(TableUsers, null, null);

                    db.SetTransactionSuccessful();
                    return true; // ✅ Success
                }
                catch (Exception ex)
                {
                    Log.Error("DatabaseHelper", $"Error clearing database: {ex.Message}");
                    return false; // ❌ Something went wrong
                }
                finally
                {
                    db.EndTransaction();
                }
            }
        }

        public Workout GetWorkoutById(int workoutId)
        {
            using (var db = ReadableDatabase)
            {
                string query = $"SELECT * FROM {TableWorkouts} WHERE {ColumnWorkoutId} = ?";
                var cursor = db.RawQuery(query, new string[] { workoutId.ToString() });

                if (cursor.MoveToNext())
                {
                    return new Workout
                    {
                        Id = cursor.GetInt(cursor.GetColumnIndex(ColumnWorkoutId)),
                        Name = cursor.GetString(cursor.GetColumnIndex(ColumnWorkoutName)),
                        Date = DateTime.Parse(cursor.GetString(cursor.GetColumnIndex(ColumnWorkoutDate)))
                    };
                }
                return null; // Return null if no workout found with the given ID
            }
        }


    }
}
