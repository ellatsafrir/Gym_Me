using SQLite;

/*  הקלאס הוא גם טבלה בדאטאבייס מכיוון שככה אין טעויות בקישור בין בדאטאבייס לקלאסים.
    הכוונה שבהתחלה הכל היה עם סטרינגים משמע היה צורך שהשמות והפרופרטיס הכתובים בדאטאבייס ובקלאסים    
    שהיו זהים אך היו תקלות וטעיות לכן זוהי שיטה הנוחה יותר לשימוש.

    דאטאבייס אינו בנוי כמו objectorianted.
    לכן כדי לשמור ולהוציא מידע משם צריך מחלקות כי האפליקציה שלי בנוייה כ objectorianted.
 */
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
        // קשור למחלקה ולא לטבלה לכן רשום ignore
        [Ignore]
        public List<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
