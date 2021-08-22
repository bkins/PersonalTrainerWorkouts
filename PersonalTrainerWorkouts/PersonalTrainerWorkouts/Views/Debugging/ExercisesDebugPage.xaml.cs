using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Debugging
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExercisesDebugPage : ContentPage
    {
        
        public List<Exercise> ExercisesData { get; }
            
        public ObservableCollection<Exercise> ListOfExercises { get; set; }

        public ExercisesDebugPage()
        {
            InitializeComponent();
            var viewModel = new ExercisesRawViewModel();
            ExercisesData   = viewModel.ExercisesData;
            ListOfExercises = viewModel.ListOfExercises;

            BindingContext             = ListOfExercises;
            CollectionView.ItemsSource = ListOfExercises;
        }
    }
}