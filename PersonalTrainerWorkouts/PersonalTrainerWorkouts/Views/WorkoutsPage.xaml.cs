using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Services;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkoutsPage : ContentPage
    {
        public WorkoutsPage()
        {
            var pageService  = new PageService();

            ViewModel = new WorkoutsPageViewModel(App.Database
                                                , pageService);

            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            ViewModel.LoadDataCommand.Execute(null);
            base.OnAppearing();
        }
        void OnContactSelected(object sender, SelectedItemChangedEventArgs e)
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