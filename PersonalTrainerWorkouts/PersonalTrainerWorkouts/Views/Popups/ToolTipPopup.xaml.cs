using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Popups;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ToolTipPopup : Popup
{
    public ToolTipPopup(string message)
    {
        InitializeComponent();
        DisplayTextLabel.Text = message;
    }
}
