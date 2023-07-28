using System;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Tab_Workouts
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ExerciseId)
                 , nameof(ExerciseId))]
    [QueryProperty(nameof(WorkoutId)
                 , nameof(WorkoutId))]
    public partial class TypeOfExerciseEntryPage
    {
        private TypeOfExerciseViewModel _viewModel;

        private string _workoutId = "0";

        public string WorkoutId
        {
            get => _workoutId;
            set => LoadWorkout(value);
        }

        private string _exerciseId = "0";

        public string ExerciseId
        {
            get => _exerciseId;
            set => LoadExercise(value);
        }

        private void LoadWorkout(string workoutId)
        {
            _workoutId = workoutId;
        }

        private void LoadExercise(string exerciseId)
        {
            try
            {
                _exerciseId = exerciseId;
                _viewModel   = new TypeOfExerciseViewModel(_exerciseId);
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to load TypeOfExercises."
                               , Category.Error
                               , e);

                //BENDO: consider implementing a page that shows exception details
            }
        }

        public TypeOfExerciseEntryPage()
        {
            InitializeComponent();
        }

        private void Name_OnUnfocused(object         sender
                                          , FocusEventArgs e)
        {
            var nameEntry = (Entry)sender;

            _viewModel.SaveTypeOfExercise(nameEntry.Text);
            var instance = new ExerciseAddEditPage(WorkoutId
                                                 , ExerciseId);

            PageNavigation.NavigateTo(instance);
        }
    }
}
