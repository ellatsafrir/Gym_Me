using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Me
{
    public abstract class ExerciseBase
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string TargetMuscles { get; set; }
    }

}
