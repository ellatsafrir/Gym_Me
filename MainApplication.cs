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
            base.OnCreate();

            // 🔥 Put your singleton or startup logic here
            ExcersizeList.Instance.LoadCsvFromAssets(this);
        }

    }
}
