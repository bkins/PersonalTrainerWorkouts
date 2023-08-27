using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Avails.Xamarin.Logger;
using Avails.Xamarin.Utilities;
using Markdig;
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
public partial class ReleaseBodyPage : IQueryAttributable
{
    private const string TemplatePath = "PersonalTrainerWorkouts.Views.Tab_About.MarkdownTemplate.html"; //"MarkdownTemplate.html";
    private       Updater        AppUpdater     { get; set; }

    public ReleaseNotesViewModel ReleaseNotesViewModel { get; set; }

    public string BuildNumber   { get; set; }
    public string VersionNumber { get; set; }

    public void ApplyQueryAttributes(IDictionary<string, string> query)
    {
        BuildNumber   = HttpUtility.UrlDecode(query[nameof(BuildNumber)]);
        VersionNumber = HttpUtility.UrlDecode(query[nameof(VersionNumber)]);
    }

    public ReleaseBodyPage(string buildNumber
                         , string versionNumber)
    {
        BuildNumber   = buildNumber;
        VersionNumber = versionNumber;
        Title         = "Release Notes";

        AppUpdater = new Updater(App.InternetData);

        InitializeComponent();
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
            Logger.WriteLineToToastForced($"No Release Notes found for {AppUpdater.ReleaseInfo?.TagName}. See logs for a problem"
                                        , Category.Information);
        }

        var markdownContent = ReleaseNotesViewModel.ReleaseNotes ?? string.Empty;
        var htmlContent     = Markdown.ToHtml(markdownContent);
        htmlContent = htmlContent.Replace("<pre><code>", "")
                                 .Replace("</code></pre>", "");

        //var htmlWithMarkdown = string.Format(File.ReadAllText(TemplatePath), htmlContent);
        string templateContent = string.Empty;

        var assembly = Assembly.GetExecutingAssembly(); // Get a reference to the current assembly
        using (var stream = assembly.GetManifestResourceStream(TemplatePath))
        {
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                templateContent = await reader.ReadToEndAsync();
            }
        }
        var htmlWithMarkdown = string.Format(templateContent, htmlContent);

        Device.BeginInvokeOnMainThread(() =>
        {
            Title = $"Release Notes: {ReleaseNotesViewModel.TagName}";
            _webView.Source = new HtmlWebViewSource { Html = htmlWithMarkdown };
        });

        // var markdownContent = ReleaseNotesViewModel.ReleaseNotes ?? string.Empty;
        // var htmlContent     = Markdown.ToHtml(markdownContent);
        // //_webView = (WebView)Content; // Get reference to the _webView from Content
        // Device.BeginInvokeOnMainThread(() =>
        // {
        //     _webView.Source = new HtmlWebViewSource { Html = htmlContent };
        // });

        // var sanitizedNotes = SanitizeHtml(ReleaseNotesViewModel.ReleaseNotes ?? string.Empty);
        //
        // Device.BeginInvokeOnMainThread(() =>
        // {
        //     // Use the sanitized release notes for display
        //     _webView.Source = sanitizedNotes.IsNullEmptyOrWhitespace()
        //                             ? new HtmlWebViewSource { Html = "No Release Notes found." }
        //                             : new HtmlWebViewSource { Html = sanitizedNotes };
        // });
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
