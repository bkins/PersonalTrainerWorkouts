using System;
using System.Threading.Tasks;
using Android.OS;
using Avails.Xamarin.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ReleaseNotesViewModel : ViewModelBase
    {
        private string  BuildNumber   { get; set; }
        private string  VersionNumber { get; set; }

        public  string  ReleaseNotes  { get; set; }
        public  string  TagName       { get; set; }
        private ReleaseNotesViewModel(string buildNumber, string versionNumber)
        {
            BuildNumber   = buildNumber;
            VersionNumber = versionNumber;

            AppUpdater = new Updater(App.InternetData);
        }

        public static async Task<ReleaseNotesViewModel> CreateAsync(string buildNumber, string versionNumber)
        {
            var viewModel = new ReleaseNotesViewModel(buildNumber, versionNumber);
            await viewModel.InitializeReleaseNotesAsync();

            return viewModel;
        }

        private async Task InitializeReleaseNotesAsync()
        {
            await SetRelease().ConfigureAwait(false);
        }

        private async Task SetRelease()
        {
            var release = await AppUpdater.GetReleaseByBuildAndVersion(BuildNumber
                                                                     , VersionNumber)
                                          .ConfigureAwait(false);
            if (release is null) return;

            TagName      = release.TagName;
            ReleaseNotes = release.Body; //releaseNotes;//.Replace(" * ", $"{Environment.NewLine}\t * ");
        }
    }
}
