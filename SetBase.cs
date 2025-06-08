
namespace Gym_Me
{
    /*Serializable ממיר לסטרינג ואחר כך ישנה את האופציה להמיר חזרה לעצם או למחלקה בשלמותו. 
    בגלל שאני מעבירה מידע דרך intent וintent מעביר רק string
    לכן יש צורך להשתמש בזה*/
    [Serializable]
    public abstract class SetBase
    {
        public int ExcersizeId {  get; set; }
        public string Type { get; set; } = "Regular";
        public int Sets { get; set; }
        public int Reps {  get; set; }
        public double Weight { get; set; } 
        public double SetTime {  get; set; }
        public double RestTime {  get; set; }
    }

}
