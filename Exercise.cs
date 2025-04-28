using SQLite;

namespace Gym_Me
{
    [Table("Exercise")]
    public class Exercise : SetBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Description { get; set; } = "";

        public int WorkoutId { get; set; } // Foreign key to Workout

        public override string ToString()
        {
            return $"Name: {ExcersizeList.Instance.GetExcersizeData(ExcersizeId).Name}\nReps: {Reps}, Weight: {Weight}, SetTime: {SetTime}, RestTime: {RestTime}";
        }
    }
}
