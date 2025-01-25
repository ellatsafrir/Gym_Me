using Android.Health.Connect.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Me
{
    public abstract class SetBase
    {
        public ExcersizeData Excersize { get; set; }
        public string Type { get; set; }
        public int Reps {  get; set; }
        public float Weight { get; set; } 
        public float SetTime {  get; set; }
        public float RestTime {  get; set; }
    }

}
