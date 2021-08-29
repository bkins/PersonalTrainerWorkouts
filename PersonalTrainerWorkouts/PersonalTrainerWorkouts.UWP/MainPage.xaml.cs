using Syncfusion.ListView.XForms.UWP;
using PersonalTrainerWorkouts.UWP.Utilities;

[assembly: Xamarin.Forms.Dependency(typeof(MessageUwp))]
namespace PersonalTrainerWorkouts.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
this.InitializeComponent();
SfListViewRenderer.Init();

            LoadApplication(new PersonalTrainerWorkouts.App());
        }
    }
}
