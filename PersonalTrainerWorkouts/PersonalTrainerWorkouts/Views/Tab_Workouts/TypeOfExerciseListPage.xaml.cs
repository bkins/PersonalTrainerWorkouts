using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Avails.D_Flat.Exceptions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using Xamarin.Forms;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Workouts
{
    [QueryProperty(nameof(WorkoutId)
                 , nameof(WorkoutId))]
    [QueryProperty(nameof(ExerciseId)
                 , nameof(ExerciseId))]
    public partial class TypeOfExerciseListPage : IQueryAttributable
    {
        private TypeOfExerciseListViewModel ViewModel   { get; set; }
        public  string                      WorkoutId   { get; set; }
        private string                      _exerciseId { get; set; }
        public  string                      ExerciseId  { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                // The query parameter requires URL decoding.
                WorkoutId  = HttpUtility.UrlDecode(query[nameof(WorkoutId)]);
                ExerciseId = HttpUtility.UrlDecode(query[nameof(ExerciseId)]);

                _exerciseId = ExerciseId;
                ViewModel   = new TypeOfExerciseListViewModel();

                LoadData();
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed initiate ExerciseAddEditPage."
                               , Category.Error
                               , e);

                throw;
            }
        }

        private void LoadData()
        {
            TypeOfExercisePicker.ItemsSource = ViewModel.ListOfAllExerciseTypes.Concat(new[]
                                                                                       {
                                                                                           new TypeOfExercise
                                                                                           {
                                                                                               Name = "<New>"
                                                                                           }
                                                                                       })
                                                        .OrderBy(field => field.Id);
        }

        public TypeOfExerciseListPage()
        {
            InitializeComponent();
        }

        private void TypeOfExercisePicker_OnOkButtonClicked(object                    sender
                                                                , SelectionChangedEventArgs e)
        {
            var selected = (TypeOfExercise)TypeOfExercisePicker.SelectedItem;
            
            if (selected.Name == "<New>")
            {
                SaveNewExerciseType().ConfigureAwait(false);
            }
            else if (_exerciseId != "0")
            {
                ViewModel.SelectedTypeOfExercise = selected;

                TryToSaveExerciseType().ConfigureAwait(false);
            }

            PageNavigation.NavigateTo(nameof(ExerciseAddEditPage)
                                          , nameof(ExerciseAddEditPage.WorkoutId)
                                          , WorkoutId
                                          , nameof(ExerciseAddEditPage.ExerciseId)
                                          , _exerciseId);
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

                var viewModel = new TypeOfExerciseViewModel(_exerciseId);

                Logger.WriteLine($"ViewModel {nameof(TypeOfExerciseViewModel)} instantiated."
                               , Category.Information);

                try
                {
                    Logger.WriteLine($"Saving Type {typeName}."
                                   , Category.Information);

                    viewModel.SaveTypeOfExercise(typeName);

                    saved = true;

                    Logger.WriteLine("New type saved."
                                   , Category.Information);
                }
                // catch (AttemptToAddDuplicateEntityException)
                // {
                //     await DisplayAlert("Type Already Exists"
                //                      , $"The Type '{typeName}' already exists.  Name the Type with a different name."
                //                      , "OK");
                // }
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
                ViewModel.SaveExerciseType(int.Parse(_exerciseId));
            }
            catch (EntityRelationAlreadyExistsException alreadyExistsException)
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

        private void TypeOfExercisePicker_OnCancelButtonClicked(object                    sender
                                                                    , SelectionChangedEventArgs e)
        {
            PageNavigation.NavigateBackwards();
        }
    }
}
