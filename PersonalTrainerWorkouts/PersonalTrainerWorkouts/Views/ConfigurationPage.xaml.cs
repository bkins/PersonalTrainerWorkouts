
using System;
using PersonalTrainerWorkouts.Views.Debugging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPage : ContentPage
    {
        public ConfigurationPage()
        {
            InitializeComponent();
        }

        private void DropTablesButtonClicked(object    sender,
                                             EventArgs e)
        {
            App.Database.DropTables();
        }

        private void CreateTablesButtonClicked(object    sender,
                                               EventArgs e)
        {
            App.Database.CreateTables();
        }

        private async void ViewLogButtonClicked(object    sender,
                                          EventArgs e)
        {
            var path = $"{nameof(MessageLog)}";
            await Shell.Current.GoToAsync(path);
        }
        
        private async void ViewWorkoutExerciseButtonClicked(object    sender
                                                    , EventArgs e)
        {
            var path = $"{nameof(WorkoutExerciseDebugPage)}";
            await Shell.Current.GoToAsync(path);
        }

        private async void ViewExercisesButtonClicked(object    sender
                                              , EventArgs e)
        {
            var path = $"{nameof(ExercisesDebugPage)}";
            await Shell.Current.GoToAsync(path);
        }

        private void FillTablesButtonClicked(object    sender
                                           , EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}