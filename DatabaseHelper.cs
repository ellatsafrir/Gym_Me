using Android.Content;
using Android.Database.Sqlite;
using Android.Util;
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
        private const int DatabaseVersion = 13;

      
        public DatabaseHelper(Context context)
            : base(context, DatabaseName, null, DatabaseVersion)
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                CreateDB();
            }
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            Log.Debug("DatabaseHelper", "Creating tables...");
            CreateDB();
        }

        public void CreateDB()
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                connection.CreateTable<User>();
                connection.CreateTable<Workout>();
                connection.CreateTable<Exercise>();
                connection.CreateTable<WorkoutLogTable>();
                connection.CreateTable<WorkoutSetLogTable>();
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
                connection.DropTable<WorkoutLog>();
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

                foreach (var workout in workouts)
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
            using (var connection = new SQLiteConnection(dbPath))
            {
                try
                {
                    var user = new User
                    {
                        Email = email,
                        Password = password,
                        Name = name
                    };
                    connection.Insert(user);
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error("DatabaseHelper", $"AddUser failed: {ex.Message}");
                    return false;
                }
            }
        }

        public bool IsEmailRegistered(string email)
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                return connection.Table<User>()
                    .Any(u => u.Email == email);
            }
        }

        public List<Workout> GetAllWorkoutsWithDetails()
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                var workouts = connection.Table<Workout>()
                    .OrderByDescending(w => w.Date)
                    .ToList();

                foreach (var workout in workouts)
                {
                    workout.Exercises = GetExercisesForWorkout(workout.Id);
                }

                return workouts;
            }
        }

        public bool AuthenticateUser(string email, string password)
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                return connection.Table<User>()
                    .Any(u => u.Email == email && u.Password == password);
            }
        }

        public int GetWorkoutIdByName(string workoutName)
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                var workout = connection.Table<Workout>()
                    .FirstOrDefault(w => w.Name == workoutName);

                return workout?.Id ?? -1; // Return the WorkoutId or -1 if not found
            }
        }

        // Workout Management
        public long SaveWorkout(Workout workout)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                db.Insert(workout);
            }
            return workout.Id;
        }

        

        //public void SaveExerciseSet(int workoutId, int exerciseId, int repetitions, double weight, double restTime)
        public long SaveExerciseSet(Exercise excersize)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                return db.Insert(excersize);
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
                using (var connection = new SQLiteConnection(dbPath))
                {
                    var workout = connection.Table<Workout>().FirstOrDefault(w => w.Id == workoutId);
                    if (workout != null)
                    {
                        connection.Delete(workout);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("DatabaseHelper", ex.Message);
            }
            return false;
        }


        public List<Workout> GetWorkoutsForDate(DateTime date)
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                string dateString = date.ToString("yyyy-MM-dd");  // format DateTime to yyyy-MM-dd
                return connection.Table<Workout>()
                    .Where(w => w.Date == date.Date)
                    .ToList();
            }
        }


        public Exercise GetExerciseById(int exerciseId)
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                return connection.Table<Exercise>()
                    .FirstOrDefault(e => e.Id == exerciseId);
            }
        }

        public bool ClearDatabase()
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                try
                {
                    connection.DropTable<User>();
                    connection.DropTable<Workout>();
                    connection.DropTable<Exercise>();

                    return true; // ✅ Success
                }
                catch (Exception ex)
                {
                    Log.Error("DatabaseHelper", $"Error clearing database: {ex.Message}");
                    return false; // ❌ Something went wrong
                }
            }
        }

        public Workout GetWorkoutById(int workoutId)
        {
            using (var connection = new SQLiteConnection(dbPath))
            {
                var workout = connection.Table<Workout>()
                    .FirstOrDefault(w => w.Id == workoutId);

                if (workout != null)
                {
                    workout.Date = DateTime.Parse(workout.Date.ToString()); // Ensure date is properly formatted
                    return workout;
                }

                return null; // Return null if no workout found
            }
        }

        public int InsertWorkoutLog(WorkoutLog log)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                // Save WorkoutLog metadata
                var workoutLogRow = new WorkoutLogTable
                {
                    WorkoutId = log.WorkoutId,
                    Timestamp = log.Timestamp
                };

                db.Insert(workoutLogRow); // This populates workoutLogRow.Id

                foreach (var set in log.Sets)
                {
                    var setLogRow = new WorkoutSetLogTable
                    {
                        WorkoutLogId = workoutLogRow.Id,
                        ExerciseId = set.ExerciseId,
                        Reps = set.Reps,
                        SetTime = set.Time.SetTime,
                        RestTime = set.Time.RestTime
                    };

                    db.Insert(setLogRow);
                }

                return workoutLogRow.Id; // Return the generated ID
            }
        }
        public List<WorkoutLog> GetLogsForWorkout(int workoutId)
        {
            using (var db = new SQLiteConnection(dbPath))
            {
                var workoutLogs = db.Table<WorkoutLogTable>()
                    .Where(x => x.WorkoutId == workoutId)
                    .ToList();

                var result = new List<WorkoutLog>();

                foreach (var log in workoutLogs)
                {
                    var sets = db.Table<WorkoutSetLogTable>() 
                        .Where(x => x.WorkoutLogId == log.Id)
                        .Select(s => new WorkoutSetLog
                        {
                            Id = s.Id,
                            ExerciseId = s.ExerciseId,
                            Reps = s.Reps,
                            Time = new WorkoutTimeLog
                            {
                                SetTime = s.SetTime,
                                RestTime = s.RestTime
                            }
                        }).ToList();

                    result.Add(new WorkoutLog
                    {
                        Id = log.Id,
                        WorkoutId = log.WorkoutId,
                        Timestamp = log.Timestamp,
                        Sets = sets
                    });
                }

                return result;
            }
        }

    }
}