using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Misc;

[QueryProperty(nameof(Url), nameof(Url))]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class BrowserPage : ContentPage, IQueryAttributable
{
    public string Url { get; set; }
    public BrowserPage()
    {
        InitializeComponent();

        // Load the web page in the WebView
        // BrowserWebView.Source = "https://github.com/bkins/PersonalTrainerWorkouts/releases"; // Replace with the URL you want to load

    }

    public BrowserPage(string url)
    {
        Url = url;
    }

    public void ApplyQueryAttributes(IDictionary<string, string> query)
    {
        Url = HttpUtility.UrlDecode(query[nameof(Url)]);

        BrowserWebView.Source = Url;
    }
}
