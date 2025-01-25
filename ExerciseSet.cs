using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Me
{
    public class ExerciseSet
    {
        public int Id { get; set; }
        public Exercise Exercise { get; set; }
        public int Repetitions { get; set; }
        public double Weight { get; set; } // in kilograms
        public int RestTime { get; set; } // in seconds
    }

}
