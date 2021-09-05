using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ApplicationExceptions;
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
    public partial class TypeOfExerciseListPage : IQueryAttributable
    {
        private TypeOfExerciseListViewModel ViewModel { get; set; }
        
        public string ExerciseId { get; set; }
        
        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                // The query parameter requires URL decoding.
                ExerciseId = HttpUtility.UrlDecode(query[nameof(ExerciseId)]);
                
                ViewModel = new TypeOfExerciseListViewModel();

                LoadData();
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed initiate ExerciseAddEditPage.", Category.Error, e);

                throw;
            }
        }

        private void LoadData()
        {
            TypeOfExercisePicker.ItemsSource = ViewModel.ListOfAllExerciseTypes.Concat(new []
                                                                                       {
                                                                                           new TypeOfExercise
                                                                                           {
                                                                                               Name = "<New>"
                                                                                           }
                                                                                       }).OrderBy(field=>field.Id);
        }
        public TypeOfExerciseListPage()
        {
            InitializeComponent();
        }

        private async void TypeOfExercisePicker_OnOkButtonClicked(object                    sender
                                                                , SelectionChangedEventArgs e)
        {
            var selected = (TypeOfExercise)TypeOfExercisePicker.SelectedItem;

            if (selected.Name == "<New>")
            {
                await SaveNewExerciseType();
            }
            else if (ExerciseId != "0")
            {
                ViewModel.SelectedTypeOfExercise = selected;

                await TryToSaveExerciseType();
            }

            await PageNavigation.NavigateBackwards();
        }

        private async Task SaveNewExerciseType()
        {
            var saved = false;

            while (! saved)
            {
                var typeName = await DisplayPromptAsync("New Type"
                                                      , "Enter the name of the new Type:"
                                                      , "OK"
                                                      , "Cancel"
                                                      , "Name"
                                                      , -1
                                                      , Keyboard.Create(KeyboardFlags.CapitalizeWord)
                                                      , "");

                var viewModel = new TypeOfExerciseViewModel(ExerciseId);

                try
                {
                    viewModel.SaveExerciseType(typeName);

                    saved = true;
                }
                catch (AttemptToAddDuplicateEntityException)
                {
                    await DisplayAlert("Type Already Exists"
                                     , $"The Type '{typeName}' already exists.  Name the Type with a different name."
                                     , "OK");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Something went wrong while trying to save the Type.  View the log for more details"
                                   , Category.Error
                                   , ex);
                }
            }
        }

        private async Task TryToSaveExerciseType()
        {
            try
            {
                ViewModel.SaveExerciseType(int.Parse(ExerciseId));

                await PageNavigation.NavigateBackwards();
            }
            catch (ExerciseTypeRelationAlreadyExistsException alreadyExistsException)
            {
                await DisplayAlert(Category.Warning.ToString()
                                 , alreadyExistsException.Message
                                 , "OK");
            }
            catch (Exception exception)
            {
                Logger.WriteLine(exception.Message
                               , Category.Error
                               , exception);
            }
        }

        private async void TypeOfExercisePicker_OnCancelButtonClicked(object                    sender
                                                                    , SelectionChangedEventArgs e)
        {
            await PageNavigation.NavigateBackwards();
        }
    }
}