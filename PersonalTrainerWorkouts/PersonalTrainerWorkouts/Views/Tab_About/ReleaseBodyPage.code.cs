using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Tab_About;
[QueryProperty(nameof(FormattedBody)
             , nameof(FormattedBody))]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ReleaseBodyPage : IQueryAttributable
{
    public void ApplyQueryAttributes(IDictionary<string, string> query)
    {
        FormattedBody = HttpUtility.UrlDecode(query[nameof(FormattedBody)]);

        // Sanitize the HTML content using HtmlAgilityPack
        FormattedBody = SanitizeHtml(FormattedBody);

    }

    private static string SanitizeHtml(string htmlContent)
    {
        // Load the HTML content into HtmlDocument
        var htmlDocument = new HtmlDocument();
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
