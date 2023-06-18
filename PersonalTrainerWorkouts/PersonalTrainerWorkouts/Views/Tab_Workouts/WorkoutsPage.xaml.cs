using System;
using PersonalTrainerWorkouts.Services;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Tab_Workouts
{
    [Obsolete("This View and the WorkoutsPageViewModel are an incomplete trial of a different approach I would like to explore more later.  For now they are not used.")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkoutsPage
    {
        public WorkoutsPage()
        {
            var pageService = new PageService();

            ViewModel = new WorkoutsPageViewModel(App.Database
                                                , pageService);

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            ViewModel.LoadDataCommand.Execute(null);
            base.OnAppearing();
        }

        void OnContactSelected(object                       sender
                             , SelectedItemChangedEventArgs e)
        {
            ViewModel.SelectWorkoutCommand.Execute(e.SelectedItem);
        }

        public WorkoutsPageViewModel ViewModel
        {
            get => BindingContext as WorkoutsPageViewModel;
            set => BindingContext = value;
        }
    }
}
