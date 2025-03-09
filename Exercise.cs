using SQLite;

namespace Gym_Me
{
    [Table("Exercise")]
    public class Exercise : SetBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
