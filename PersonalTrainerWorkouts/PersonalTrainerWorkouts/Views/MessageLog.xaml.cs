using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageLog : ContentPage
    {
        public MessageLogViewModel PageDate { get; set; }

        public MessageLog()
        {
            InitializeComponent();

            BindingContext = new MessageLogViewModel();
        }
    }
}
