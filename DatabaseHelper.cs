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
        private const int DatabaseVersion = 1;

        // Table and Column names
        private const string TableUsers = "Users";
        private const string ColumnId = "Id";
        private const string ColumnEmail = "Email";
        private const string ColumnPassword = "Password";
        private const string ColumnName = "Name";
        private const string TableWorkouts = "Workouts";
        private const string ColumnWorkoutId = "Id";
        private const string ColumnWorkoutName = "Name";
        private const string ColumnWorkoutDate = "Date";
        private const string TableExercises = "Exercises";
        private const string ColumnExerciseId = "Id";
        private const string ColumnWorkoutIdForExercise = "WorkoutId";
        private const string ColumnExerciseName = "Name";

        public DatabaseHelper(Context context)
            : base(context, DatabaseName, null, DatabaseVersion)
        {
        }

        // Called when the database is created
        public override void OnCreate(SQLiteDatabase db)
        {
            string createUsersTable = $@"
                CREATE TABLE {TableUsers} (
                    {ColumnId} INTEGER PRIMARY KEY AUTOINCREMENT,
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
                    {ColumnWorkoutIdForExercise} INTEGER NOT NULL,
                    {ColumnExerciseName} TEXT NOT NULL,
                    FOREIGN KEY ({ColumnWorkoutIdForExercise}) REFERENCES {TableWorkouts}({ColumnWorkoutId})
                );";

            db.ExecSQL(createUsersTable);
            db.ExecSQL(createWorkoutsTable);
            db.ExecSQL(createExercisesTable);
        }

        // Called when the database is upgraded
        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            db.ExecSQL($"DROP TABLE IF EXISTS {TableUsers}");
            db.ExecSQL($"DROP TABLE IF EXISTS {TableWorkouts}");
            db.ExecSQL($"DROP TABLE IF EXISTS {TableExercises}");
            OnCreate(db);
        }

        // Add a new user
        public bool AddUser(string email, string password)
        {
            using (var db = WritableDatabase)
            {
                try
                {
                    var contentValues = new ContentValues();
                    contentValues.Put(ColumnEmail, email);
                    contentValues.Put(ColumnPassword, password);

                    db.InsertOrThrow(TableUsers, null, contentValues);
                    return true;
                }
                catch (Exception)
                {
                    return false; // Handle exceptions (e.g., duplicate email)
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

        

        //public void UpdateWorkout(string originalWorkoutName, string newWorkoutName, List<string> exercises, DateTime date)
        //{
        //    using (var db = this.WritableDatabase)
        //    {
        //        // Update the workout in the database
        //        ContentValues values = new ContentValues();
        //        values.Put("workout_name", newWorkoutName);
        //        values.Put("date", date.ToString("yyyy-MM-dd"));

        //        // Update exercises
        //        DeleteExercisesForWorkout(originalWorkoutName, db); // Delete existing exercises
        //        foreach (var exercise in exercises)
        //        {
        //            SaveExercise(newWorkoutName, exercise, db); // Save new exercises
        //        }

        //        db.Update("workouts", values, "workout_name = ?", new string[] { originalWorkoutName });
        //    }
        //}

        //public bool UpdateWorkout(Workout workout)
        //{
        //    using (var db = WritableDatabase)
        //    {
        //        db.BeginTransaction();  // Begin a transaction
        //        try
        //        {
        //            // Step 1: Delete the existing workout and its exercises
        //            db.ExecSQL($"DELETE FROM {TableExercises} WHERE {ColumnWorkoutIdForExercise} = ?", new Java.Lang.Object[] { new Java.Lang.String(workout.Id.ToString()) });
        //            db.ExecSQL($"DELETE FROM {TableWorkouts} WHERE {ColumnWorkoutId} = ?", new Java.Lang.Object[] { new Java.Lang.String(workout.Id.ToString()) });

        //            // Step 2: Insert the updated workout and its exercises
        //            SaveWorkout(workout.Id, workout.Exercises, workout.Date, db); // No need to pass the database as a parameter if it's already available in the method

        //            db.SetTransactionSuccessful(); // Commit the transaction if everything is successful
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
                  
        //            throw ex; // Re-throw the exception after rolling back
        //        }
        //        finally
        //        {
        //            db.EndTransaction(); // Ensure the transaction is ended, whether success or failure
        //        }
        //    }
        //}

        // Inside DatabaseHelper.cs
        public void DeleteExercisesForWorkout(string workoutName, SQLiteDatabase db)
        {
            // Convert the string array to Java.Lang.Object array
            Java.Lang.Object[] args = new Java.Lang.Object[] { new Java.Lang.String(workoutName) };

            // Execute the SQL delete query
            db.ExecSQL("DELETE FROM exercises WHERE workout_name = ?", args);
        }


        // Inside DatabaseHelper.cs
        //public void SaveExercise(string workoutName, string exercise, SQLiteDatabase db)
        //{
        //    var workoutId = GetWorkoutIdByName(workoutName, db);
        //    if (workoutId == -1) return; // No such workout

        //    ContentValues values = new ContentValues();
        //    values.Put(ColumnWorkoutIdForExercise, workoutId);
        //    values.Put(ColumnExerciseName, exercise);

        //    db.Insert(TableExercises, null, values);  // Inserting exercise into "Exercises" table
        //}

        private long GetWorkoutIdByName(string workoutName, SQLiteDatabase db)
        {
            string query = $@"
            SELECT {ColumnWorkoutId}
            FROM {TableWorkouts}
            WHERE {ColumnWorkoutName} = ?";

            using (var cursor = db.RawQuery(query, new string[] { workoutName }))
            {
                if (cursor.Count > 0)
                {
                    cursor.MoveToFirst();
                    return cursor.GetLong(0); // Return the Workout ID
                }
            }
            return -1; // No workout found
        }



        public void SaveWorkout(string workoutName, List<string> exercises, DateTime selectedDate, SQLiteDatabase db)
        {
            try
            {
             //   db.BeginTransaction();
                // Insert Workout
                var contentValues = new ContentValues();
                contentValues.Put(ColumnWorkoutName, workoutName);
                contentValues.Put(ColumnWorkoutDate, selectedDate.ToString("yyyy-MM-dd")); // Use selectedDate here

                long workoutId = db.Insert(TableWorkouts, null, contentValues); // Insert the workout and get its ID

                // Insert Exercises
                foreach (var exercise in exercises)
                {
                    var exerciseValues = new ContentValues();
                    exerciseValues.Put(ColumnWorkoutIdForExercise, workoutId);
                    exerciseValues.Put(ColumnExerciseName, exercise);
                    db.Insert(TableExercises, null, exerciseValues); // Insert each exercise
                }

            }
            catch (Exception ex)
            {
              
                throw ex; // Rollback and throw if something goes wrong
            }
          
        }

        public List<string> GetAllWorkouts()
        {
            var workouts = new List<string>();
            using (var db = ReadableDatabase)
            {
                string query = $@"
            SELECT {ColumnWorkoutName}
            FROM {TableWorkouts}";

                using (var cursor = db.RawQuery(query, new string[] { }))
                {
                    while (cursor.MoveToNext())
                    {
                        workouts.Add(cursor.GetString(0)); // Add workout name to the list
                    }
                }
            }
            return workouts;
        }


        // Check if an email already exists
        public bool IsEmailRegistered(string email)
        {
            using (var db = ReadableDatabase)
            {
                string query = $@"
                    SELECT * FROM {TableUsers}
                    WHERE {ColumnEmail} = ?";

                using (var cursor = db.RawQuery(query, new string[] { email }))
                {
                    return cursor.Count > 0;
                }
            }
        }

        public List<string> GetWorkoutsForDate(DateTime date)
        {
            var workouts = new List<string>();
            using (var db = ReadableDatabase)
            {
                string query = $@"
                    SELECT {ColumnWorkoutName}
                    FROM {TableWorkouts}
                    WHERE {ColumnWorkoutDate} = ?";

                string dateString = date.ToString("yyyy-MM-dd"); // Format date for SQLite
                using (var cursor = db.RawQuery(query, new string[] { dateString }))
                {
                    while (cursor.MoveToNext())
                    {
                        workouts.Add(cursor.GetString(0)); // Add workout name to the list
                    }
                }
            }
            return workouts;
        }

        public List<string> GetExercisesForWorkout(string workoutName)
        {
            var exercises = new List<string>();
            using (var db = ReadableDatabase)
            {
                string query = $@"
                SELECT  {TableExercises}.{ColumnExerciseName}
                FROM {TableExercises}
                JOIN {TableWorkouts} ON {TableWorkouts}.{ColumnWorkoutId} = {TableExercises}.{ColumnWorkoutIdForExercise}
                WHERE {TableWorkouts}.{ColumnWorkoutName} = ?";


                using (var cursor = db.RawQuery(query, new string[] { workoutName }))
                {
                    if (cursor != null)
                    {
                        while (cursor.MoveToNext())
                        {
                            exercises.Add(cursor.GetString(0)); // Add exercise name to the list
                        }
                    }
                    else
                    {
                        exercises.Add("dummy");

                    }
                }
            }
            return exercises;
        }
        public bool DeleteWorkout(string workoutName)
        {
            try
            {
                using (var db = WritableDatabase)
                {
                    // Delete the workout and its exercises from the database
                    int rowsAffected = db.Delete("Workouts", "name = ?", new string[] { workoutName });
                    db.Delete("Exercises", "workout_name = ?", new string[] { workoutName });  // Assuming "Exercises" table has a column "workout_name"
                    return rowsAffected > 0; // Return true if rows were affected
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Log.Error("DatabaseHelper", ex.Message);
                return false;
            }
        }
    }
}
