using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views.Tab_About;

public partial class ReleaseBodyPage : ContentPage
{
    private WebView _webView;

    public ReleaseBodyPage()
    {
        InitializeComponent();
    }

    // Define the UI components in the constructor
    private void InitializeComponent()
    {
        _webView = new WebView // Assign the WebView instance to the private field
                   {
                       Source          = new HtmlWebViewSource(),
                       VerticalOptions = LayoutOptions.FillAndExpand
                   };

        Content = _webView; // Set the WebView as the Content of the page
    }
}
