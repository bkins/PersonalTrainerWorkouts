using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ExerciseId), nameof(ExerciseId))]
    public partial class TypeOfExerciseListPage : ContentPage
    {
        private TypeOfExerciseListViewModel ViewModel { get; set; }
        
        private string _exerciseId = "0";
        public string ExerciseId
        {
            get => _exerciseId;
            set => LoadExercises(value);
        }

        private void LoadExercises(string workoutId)
        {
            try
            {
                _exerciseId = workoutId;
                
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to load TypeOfExercises.", Category.Error, e);
                //BENDO: consider implementing a page that shows exception details
            }
        }

        public TypeOfExerciseListPage()
        {
            InitializeComponent();
            ViewModel = new TypeOfExerciseListViewModel();
            
            TypeOfExercisePicker.ItemsSource = ViewModel.ListOfAllExerciseTypes.Concat(new []
                                                                                       {
                                                                                           new TypeOfExercise
                                                                                           {
                                                                                               Name = "<New>"
                                                                                           }
                                                                                       }).OrderBy(field=>field.Id);
        }

        private async void TypeOfExercisePicker_OnOkButtonClicked(object                    sender
                                                                , SelectionChangedEventArgs e)
        {
            var selected = (TypeOfExercise)TypeOfExercisePicker.SelectedItem;

            if (selected.Name == "<New>")
            {
                //BENDO: Create page to add new Type of exercise
                //Open Type of exercise page
            }
            else
            {
                ViewModel.SelectedTypeOfExercise = selected;

                ViewModel.SaveExercise(int.Parse(ExerciseId));

            }
            
            await PageNavigation.NavigateBackwards();
        }
    }
}