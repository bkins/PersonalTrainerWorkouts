using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Avails.D_Flat.Extensions;
using Avails.Xamarin.Logger;
using Avails.Xamarin.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static PersonalTrainerWorkouts.ViewModels.ReleaseNotesViewModel;

namespace PersonalTrainerWorkouts.Views.Tab_About;
[QueryProperty(nameof(BuildNumber)
             , nameof(BuildNumber))]
[QueryProperty(nameof(VersionNumber)
             , nameof(VersionNumber))]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ReleaseNotesPage : ContentPage
{
    private Updater AppUpdater { get; set; }

    public  ReleaseNotesViewModel ReleaseNotesViewModel { get; set; }
    public  string                BuildNumber           { get; set; }
    public  string                VersionNumber         { get; set; }

    public ReleaseNotesPage(string buildNumber
                          , string versionNumber)
    {
        BuildNumber   = buildNumber;
        VersionNumber = versionNumber;
        AppUpdater    = new Updater(App.InternetData);

        InitializeComponent();

        ReleaseNotesWebView = (WebView)Content; // Get reference to the _webView from Content
    }
    public void ApplyQueryAttributes(IDictionary<string, string> query)
    {
        BuildNumber   = HttpUtility.UrlDecode(query[nameof(BuildNumber)]);
        VersionNumber = HttpUtility.UrlDecode(query[nameof(VersionNumber)]);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDataAsync();
    }
    private async Task LoadDataAsync()
    {
        // ReleaseNotesViewModel = new ReleaseNotesViewModel(BuildNumber
        //                                                 , VersionNumber);

        ReleaseNotesViewModel = await CreateAsync(BuildNumber, VersionNumber).ConfigureAwait(false);
        // var releaseNotes = await Updater.GetReleaseNotesByBuildAndVersion(BuildNumber
        //                                                            , VersionNumber).ConfigureAwait(false);

        // Sanitize the HTML content using HtmlAgilityPack
        if (ReleaseNotesViewModel.ReleaseNotes is null)
        {
            Logger.WriteLineToToastForced($"No Release Notes found for {AppUpdater.ReleaseInfo?.TagName}. See logs for a problem."
                                        , Category.Information);
        }

        var sanitizedNotes = SanitizeHtml(ReleaseNotesViewModel.ReleaseNotes ?? string.Empty);

        Device.BeginInvokeOnMainThread(() =>
        {
            // Use the sanitized release notes for display
            ReleaseNotesWebView.Source = sanitizedNotes.IsNullEmptyOrWhitespace()
                                    ? new HtmlWebViewSource { Html = "No Release Notes found." }
                                    : new HtmlWebViewSource { Html = sanitizedNotes };
        });
    }

    private static string SanitizeHtml(string htmlContent)
    {
        // Load the HTML content into HtmlDocument
        var htmlDocument = new HtmlAgilityPack.HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);

        // Remove any unsafe elements or attributes
        // remove all script tags and attributes. remove other tags and attributes, as needed.
        foreach (var scriptTag in htmlDocument.DocumentNode.Descendants("script").ToList())
        {
            scriptTag.Remove();
        }

        foreach (var element in htmlDocument.DocumentNode.DescendantsAndSelf())
        {
            // Remove any event attributes that can execute JavaScript
            foreach (var attribute in element.Attributes
                                             .ToList()
                                             .Where(attribute => attribute.Name
                                                                          .StartsWith("on")))
            {
                element.Attributes.Remove(attribute);
            }
        }

        // Return the sanitized HTML content
        return htmlDocument.DocumentNode.OuterHtml;
    }
}
