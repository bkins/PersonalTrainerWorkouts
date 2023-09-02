using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avails.D_Flat.Extensions;
using Avails.GitHubService.POCOs;
using Avails.Xamarin.Interfaces;
using Avails.Xamarin.Logger;
using Xamarin.Essentials;
using GitHub = Avails.GitHubService.GitHubService;
using Log = Avails.Xamarin.Logger.Logger;

namespace PersonalTrainerWorkouts.Data;

public class InternetDataStore : IInternetDataStore
{
    public async Task<List<GitHubReleaseInfo>> GetAllReleases()
    {
        var releases = await GitHub.GetReleases()
                                   .ConfigureAwait(false);

        return releases;
    }

    public async Task<GitHubReleaseInfo> GetLatestRelease()
    {
        var latestRelease = await GitHub.GetLatestRelease().ConfigureAwait(false);

        return latestRelease;
    }
    public async Task<GitHubReleaseInfo> GetReleaseByTag(string tag)
    {
        var release = await GitHub.GetReleaseByTag(tag)
                                  .ConfigureAwait(false);

        return release;
    }
    public bool IsInternetAvailable()
    {
        var current = Connectivity.NetworkAccess;
        return current == NetworkAccess.Internet;
    }

    public bool NoInternetIsAvailable()
    {
        return ! IsInternetAvailable();
    }

    public async Task<string> DownloadFileAsync(string url)
    {
        using var client = new HttpClient();

        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var fileName = Path.GetFileName(url);
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);

            using var stream     = await response.Content.ReadAsStreamAsync();
            using var fileStream = File.Open(filePath, FileMode.Create);

            await stream.CopyToAsync(fileStream);

            Log.WriteLine($"Downloaded file: {url}", Category.Information);
            return filePath;
        }

        Log.WriteLineToToastForced($"Could not download file: {url}", Category.Error);
        return null;
    }

    public async Task OpenWebsite(string url)
    {
        if (url.IsNullEmptyOrWhitespace())
        {
            Log.WriteLineToToastForced("Website information could not be determined. Update cannot complete!"
                                     , Category.Error);
            return;
        }

        await Launcher.OpenAsync(new Uri(url));
    }
}
