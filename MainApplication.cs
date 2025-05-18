using Android.Runtime;
using Android.App;
using System;

namespace Gym_Me
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer) { }
        public override void OnCreate()
        {
            // Check Database exist if not then create them

            base.OnCreate();

            // 🔥 Put your singleton or startup logic here
            ExcersizeList.Instance.LoadCsvFromAssets(this);
        }

    }
}
