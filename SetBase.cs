﻿
namespace Gym_Me
{
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
