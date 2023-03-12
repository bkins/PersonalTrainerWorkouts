using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace PersonalTrainerWorkouts.Droid
{
    [Activity
            (
                Label        = "PersonalTrainerWorkouts"
              , Icon         = "@mipmap/icon"
              , Theme        = "@style/MainTheme"
              , MainLauncher = true
              , LaunchMode = LaunchMode.SingleInstance
              , ConfigurationChanges = ConfigChanges.ScreenSize
                                     | ConfigChanges.Orientation
                                     | ConfigChanges.UiMode
                                     | ConfigChanges.ScreenLayout
                                     | ConfigChanges.SmallestScreenSize
             , ScreenOrientation = ScreenOrientation.Portrait
            )]
    [IntentFilter(new[]
                  {
                      Intent.ActionView
                  }
                , Categories = new[]
                               {
                                   Intent.CategoryDefault
                                 , Intent.CategoryBrowsable
                               }
                , DataScheme = "https"
                , DataHost = "*"
                , DataPath = "/oauth2redirect")]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if ( ! CheckPermissionGranted(Manifest.Permission.ReadExternalStorage) 
                  || ! CheckPermissionGranted(Manifest.Permission.WriteExternalStorage)
                  || ! CheckPermissionGranted(Manifest.Permission.ReadContacts))
                {
                    RequestPermission();
                }
            }

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);
            
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int                          requestCode
                                                      , string[]                     permissions
                                                      , [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode
                                                                 , permissions
                                                                 , grantResults);

            base.OnRequestPermissionsResult(requestCode
                                          , permissions
                                          , grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent != null)
            {
                var data = intent.Data;

                var queryParameter = data?.GetQueryParameter("code");
                //https://accounts.google.com/signin/oauth/consent?authuser=0&part=AJi8hAMLDzceWgMwh61fV45B0UFnWrDLV4msoFYcZs2gE6WovPjlBF0gKyKmodOVsotlCkmUHxoLMvvKjO-0p1fKYtGPajvTTVDQUQFPCBqpDqyfCdrOpaKOONMeExJkR-5TmBs_ky8L98NUOOzDqK-t_zcxFM2gbeMHbLjS7FZlZAlVxGF5Z9RvQMoDkLXhZI-gYnxHOdS9oPFcWYO8JrQo_H2jl2Ny_Sa4oouUYbe5CpKTLI5K7cB4926LtCtescMR_qD1wiG41UxDPB0fQrJLWfho3aho81dE3PuHFz0Ypy4ohQLhEEW4Y01CNSUfrLUMXV-UiUlYFNCWBYvSXrjtMwQqkvH0GU9WY2DrhqpiWe1kjTm_DPo-CD2l-_hi8J4JumzjWLtKhIrntLftk3sTHl02DhGxFQ&as=S-1676832930%3A1631559198923021&pli=1&rapt=AEjHL4MEIF0d8cMISvK8bmv8Dv4gdxpZhvLeMeLOfk3hFaTea76nFe2ZxUTXKNyiDLHbaD4Q3tHldUzO7_7nr7wju_ym4pCcdg#

            }
            base.OnNewIntent(intent);
        }
        
        private void RequestPermission()
        {
            ActivityCompat.RequestPermissions(this
                                            , new[]
                                              {
                                                  Manifest.Permission.ReadExternalStorage
                                                , Manifest.Permission.WriteExternalStorage
                                                , Manifest.Permission.ReadContacts
                                              }
                                            , 0);
        }

        public bool CheckPermissionGranted(string permissions)
        {
            // Check if the permission is already available.
            if (ContextCompat.CheckSelfPermission(this, permissions) != Permission.Granted)
            {
                return false;
            }

            return true;
        }

    }
}