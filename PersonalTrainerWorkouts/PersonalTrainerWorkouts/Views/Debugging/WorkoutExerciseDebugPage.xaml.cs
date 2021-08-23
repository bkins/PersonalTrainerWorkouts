using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Debugging
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkoutExerciseDebugPage : ContentPage
    {

        public List<LinkedWorkoutsToExercises> WorkoutExercisesData { get; }
            
        public ObservableCollection<WorkoutExercise> ListOfWorkoutExercises { get; set; }

        public WorkoutExerciseDebugPage()
        {
            InitializeComponent();
            var linkedViewModel = new WorkoutsToExerciseViewModel("1");
            WorkoutExercisesData = linkedViewModel.WorkoutsToExercises;

            var viewModel = new WorkoutExerciseRawViewModel();
            //WorkoutExercisesData   = viewModel.WorkoutExercisesData;
            ListOfWorkoutExercises = viewModel.ListOfWorkoutExercises;

            BindingContext             = WorkoutExercisesData;
            CollectionView.ItemsSource = WorkoutExercisesData;
        }

        private void OnSelectionChanged(object                    sender
                                      , SelectionChangedEventArgs e)
        {
            

        }
    }
}