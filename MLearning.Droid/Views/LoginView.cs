using Android.App;
using Android.OS;
using Cirrious.MvvmCross.Droid.Views;

namespace MLearning.Droid.Views
{
    [Activity(Label = "View for FirstViewModel")]
    public class LoginView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LoginView);
        }
    }
}