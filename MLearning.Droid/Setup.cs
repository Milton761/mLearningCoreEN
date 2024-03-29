using Android.Content;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.ViewModels;
using MLearning.Core.File;
using MLearning.Droid.AsyncStorage;

namespace MLearning.Droid
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new Core.App();
        }
		
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }
        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();
            Mvx.RegisterSingleton<IAsyncStorageService>(new DroidAsyncStorageService());
        }
    }
}