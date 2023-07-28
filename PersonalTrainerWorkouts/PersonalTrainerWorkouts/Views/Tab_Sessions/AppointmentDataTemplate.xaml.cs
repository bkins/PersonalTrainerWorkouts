using PersonalTrainerWorkouts.ViewModels.Tab_Clients;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Tab_Sessions;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AppointmentDataTemplate : AbsoluteLayout
{
    public ClientViewModel ClientViewModel { get; set; }
    public AppointmentDataTemplate()
    {
        InitializeComponent();
    }

}
