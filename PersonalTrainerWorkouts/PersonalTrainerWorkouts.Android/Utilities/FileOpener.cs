using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Avails.Xamarin.Utilities.FileDownloader;
using Xamarin.Forms;
using PersonalTrainerWorkouts.Droid.Utilities;
using Avails.Xamarin.Logger;
using Xamarin.Essentials;
using Application = Android.App.Application;
using FileProvider = Android.Support.V4.Content.FileProvider;

[assembly: Dependency(typeof(FileOpener))]
namespace PersonalTrainerWorkouts.Droid.Utilities
{
    public class FileOpener : IFileOpener
    {
        public async Task OpenFileAsync(string filePath)
        {
            try
            {
                // var downloadFolderPath = FileSystem.AppDataDirectory;
                // var downloadedFilePath = Path.Combine(downloadFolderPath, "myDownloadedFile.apk");

                // Download the file and save it to downloadedFilePath

                // Open the downloaded file using Uri directly
                var uri = Android.Net.Uri.Parse("file://" + filePath);

                var intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(uri, "application/vnd.android.package-archive");
                intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.GrantReadUriPermission);
                //intent.SetPackage("com.google.android.apps.nbu.files");

                Application.Context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                Logger.WriteLine(ex.Message, Category.Error, ex);
            }
        }
        // public async Task OpenFileAsync(string filePath)
        // {
        //     try
        //     {
        //         var uri = FileProvider.GetUriForFile(Application.Context
        //                                            , "com.companyname.personaltrainerworkouts.fileprovider"
        //                                            , new Java.IO.File(filePath));
        //
        //         if (Application.Context.PackageManager != null)
        //         {
        //             if (Application.Context.PackageManager != null)
        //             {
        //                 var packageInstaller              = Application.Context.PackageManager.PackageInstaller;
        //                 var packageInstallerSessionParams = new PackageInstaller.SessionParams(PackageInstallMode.FullInstall);
        //                 packageInstallerSessionParams.SetAppPackageName("com.companyname.personaltrainerworkouts"); // Replace with your app's package name
        //
        //                 var sessionId = packageInstaller.CreateSession(packageInstallerSessionParams);
        //
        //                 using var       packageInSession = packageInstaller.OpenSession(sessionId);
        //                 await using var output           = packageInSession.OpenWrite("package", 0, -1);
        //
        //                 if (Application.Context.ContentResolver != null)
        //                 {
        //                     await using var inputStream = Application.Context
        //                                                              .ContentResolver
        //                                                              .OpenInputStream(uri);
        //                     if (inputStream != null) await inputStream.CopyToAsync(output);
        //                 }
        //
        //                 packageInSession.Fsync(output);
        //
        //                 var pendingIntent = PendingIntent.GetActivity(Application.Context
        //                                                             , sessionId
        //                                                             , new Intent()
        //                                                             , PendingIntentFlags.UpdateCurrent);
        //                 packageInstaller.RegisterSessionCallback(new MySessionCallback(), null!);
        //                 pendingIntent?.Send();
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Logger.WriteLine(ex.Message, Category.Error, ex);
        //     }
        // }

        private class MySessionCallback : PackageInstaller.SessionCallback
        {
            public override void OnCreated(int sessionId) { }

            public override void OnFinished(int sessionId, bool success) { }

            public override void OnBadgingChanged(int sessionId) { }

            public override void OnActiveChanged(int sessionId, bool active) { }

            public override void OnProgressChanged(int sessionId, float progress) { }
        }

    }
}
