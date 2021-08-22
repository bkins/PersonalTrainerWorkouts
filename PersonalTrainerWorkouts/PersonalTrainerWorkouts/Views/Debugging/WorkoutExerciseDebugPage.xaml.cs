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

        public List<WorkoutExercise> WorkoutExercisesData { get; }
            
        public ObservableCollection<WorkoutExercise> ListOfWorkoutExercises { get; set; }

        public WorkoutExerciseDebugPage()
        {
            InitializeComponent();
            var viewModel = new WorkoutExerciseRawViewModel();
            WorkoutExercisesData   = viewModel.WorkoutExercisesData;
            ListOfWorkoutExercises = viewModel.ListOfWorkoutExercises;

            BindingContext             = ListOfWorkoutExercises;
            CollectionView.ItemsSource = ListOfWorkoutExercises;
        }

        private void OnSelectionChanged(object                    sender
                                      , SelectionChangedEventArgs e)
        {
            

        }
    }
}