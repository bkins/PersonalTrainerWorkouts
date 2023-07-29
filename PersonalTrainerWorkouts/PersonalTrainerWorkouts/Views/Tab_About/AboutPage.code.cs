using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avails.D_Flat.Extensions;
using Xamarin.Essentials;
using Avails.GitHubService;
using Avails.Xamarin.Logger;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views.Tab_About;

public partial class AboutPage
{
    private void TableHeaderLabel_OnTapped(object sender, EventArgs eventArgs)
    {
        TableLabelScrollView.IsVisible = ! TableLabelScrollView.IsVisible;
    }

    private async void CheckForUpdates_OnButtonClicked(object sender, EventArgs e)
    {
        var releases = await GitHubService.GetReleases()
                                          .ConfigureAwait(false);

        var currentVersion = VersionTracking.CurrentVersion;
        var versionParts   = currentVersion.Split('.');
        var buildNumber    = versionParts.Length < 3 ? 0 : versionParts[2].ToSafeInt();
        //var currentBuild   = VersionTracking.CurrentBuild;
        var releaseInfo   = GetLatestReleaseInfo(releases, buildNumber);

        if (releaseInfo is null)
        {
            Logger.WriteLineToToastForced("App already has the most resent version", Category.Information);
            return;
        }

        Device.BeginInvokeOnMainThread(async () =>
        {
            var update = await DisplayAlert("New Version"
                                          , "There is a new version.  Would you like to install it?"
                                          , "Yes"
                                          , "No").ConfigureAwait(false);

            if (! update) return;

            Logger.WriteLineToToastForced("Downloading and installing... please be patient.", Category.Information);
            //await Launcher.OpenAsync(releaseInfo.DownloadUrl).ConfigureAwait(false);
            var fileName = GetFileNameFromUrl(releaseInfo.DownloadUrl);
            await DownloadAndOpenFileAsync(releaseInfo.DownloadUrl
                                         , fileName).ConfigureAwait(false);

            //This does not work because the install happens before this and the app will be closed at this point.
            // var displayAlertMessage = new StringBuilder();
            // displayAlertMessage.AppendLine("Would you like to see the Release Notes associated with this release?");
            // displayAlertMessage.AppendLine("If not, you can always view it online here:");
            // displayAlertMessage.AppendLine(releaseInfo.HtmlUrl);
            //
            // var viewReleaseNotes = await DisplayAlert("View Release Notes?"
            //                                         , displayAlertMessage.ToString()
            //                                         , "Yes"
            //                                         , "No").ConfigureAwait(false);
            // if (viewReleaseNotes)
            // {
            //     PageNavigation.NavigateTo(new ReleaseBodyPage {FormattedBody = releaseInfo.Body});
            // }
        });

    }

    public async Task DownloadAndOpenFileAsync(string fileUrl, string fileName)
    {
        using var httpClient = new HttpClient();
        var       fileBytes  = await httpClient.GetByteArrayAsync(fileUrl);

        // Save the downloaded file to a local location
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        File.WriteAllBytes(filePath, fileBytes);

        // Open the file using the default viewer
        await Launcher.OpenAsync(new OpenFileRequest
                                 {
                                     File = new ReadOnlyFile(filePath)
                                 });
    }

    public string GetFileNameFromUrl(string url)
    {
        // Get the filename from the URL
        var fileName = Path.GetFileName(url);

        // If the URL does not have a valid filename, you can generate a unique filename or handle it accordingly.
        if (string.IsNullOrEmpty(fileName))
        {
            throw new Exception("the download URL did not have a file name.");
        }

        return fileName;
    }

    private static GitHubReleaseInfo GetLatestReleaseInfo(IEnumerable<GitHubReleaseInfo> releases
                                                        , int                            buildNumber)
    {
        GitHubReleaseInfo latestRelease     = null;
        var               latestBuildNumber = -1;

        foreach (var release in releases)
        {
            var releaseVersionParts = GetReleaseVersionParts(release.TagName);
            var releaseBuildNumber  = GetReleaseBuildNumber(releaseVersionParts);

            if (releaseBuildNumber <= buildNumber
             || releaseBuildNumber <= latestBuildNumber) continue;

            latestRelease     = release;
            latestBuildNumber = releaseBuildNumber;
        }

        return latestRelease;
    }


    private static int GetReleaseBuildNumber(string[] releaseVersionParts)
    {
        return releaseVersionParts.Length < 3
            ? 0
            : releaseVersionParts[2].ToSafeInt();
    }

    private static string[] GetReleaseVersionParts(string releaseKey)
    {
        return releaseKey.Replace("v", "").Split('-')[0].Split('.');
    }

    /* Versioning Strategy:
     *
     * Versions hold the version number, formatted: major.minor.build.
     * major < 0, are pre-release versions (alpha, beta, rc)
     * minor changes include new small features, updates to features, medium to
     * large bug fixes
     * build number will increment with each release, regardless of the release type
     *   Consider the current version is 0.3; tag: v0.3.0-alpha and CurrentBuild is 1.
     *   I release an update, 0.4.1; tag: v0.4.1-alpha
     *   Then anther update, 0.4.2; tag: v0.4.2-alpha
     *   Now I feel it is ready to move to beta.
     *   I update the CurrentBuild to 2
     *   And update the CurrentVersion to 0.5.3; tag: v0.5.3-beta
     *   Then I feel it is ready for RC
     *   I update the CurrentBuild to 3
     *   And update the CurrentVersion to 0.6.4; tag: v0.6.4-rc
     *   Then I feel it is ready for release
     *   I update the CurrentBuild to 4
     *   And update the CurrentVersion to 1.0.5; tag: v1.0.5-prod
     * Reasoning: major and minor version numbers give level of insight of what type
     * of change was made in the new version. The build part of the version number
     * will trigger an update to the application.
     * CurrentBuild will hold the release type: 1 = Alpha; 2 = Beta; 3 = RC; 4 = Prod
     */
}
