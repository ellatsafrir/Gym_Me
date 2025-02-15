using Android.Health.Connect.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym_Me
{
    [Serializable]
    public abstract class SetBase
    {
        public ExcersizeData Excersize { get; set; }
        public string Type { get; set; }
        public int Reps {  get; set; }
        public double Weight { get; set; } 
        public double SetTime {  get; set; }
        public double RestTime {  get; set; }
    }

}
