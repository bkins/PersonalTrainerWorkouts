using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views.Tab_About;

public partial class ReleaseBodyPage : ContentPage
{
    public string FormattedBody { get; set; }

    public ReleaseBodyPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var webView = new WebView
                      {
                          Source          = new HtmlWebViewSource { Html = FormattedBody }
                        , VerticalOptions = LayoutOptions.FillAndExpand
                      };

        Content = webView;
    }
}
