using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(ExerciseId)
                 , nameof(ExerciseId))]
    [QueryProperty(nameof(WorkoutId)
                 , nameof(WorkoutId))]
    public partial class MuscleGroupListPage : ContentPage
                                             , IQueryAttributable
    {
        private MuscleGroupListViewModel OldViewModel { get; set; }
        private MuscleGroupViewModel     ViewModel    { get; set; }
        public  string                   ExerciseId   { get; set; }
        public  string                   WorkoutId    { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                // The query parameter requires URL decoding.
                ExerciseId = HttpUtility.UrlDecode(query[nameof(ExerciseId)]);
                WorkoutId  = HttpUtility.UrlDecode(query[nameof(WorkoutId)]);

                OldViewModel = new MuscleGroupListViewModel();
                ViewModel    = new MuscleGroupViewModel(ExerciseId);
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

        public MuscleGroupListPage()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            //Set the list of Synergists, plus the "Add New", to the ItemSource
            MuscleGroupPicker.ItemsSource = ViewModel.SynergistsNotInExercise.Concat(new[]
                                                                                    {
                                                                                        new ResolvedSynergistViewModel()
                                                                                    })
                                                     .OrderBy(field => field.DisplayedSynergist)
                                                     .ToList();
        }

        private async void MuscleGroupPicker_OnOkButtonClicked(object                    sender
                                                             , SelectionChangedEventArgs e)
        {
            var selected = (ResolvedSynergistViewModel)MuscleGroupPicker.SelectedItem;

            if (selected.PrimaryMuscleGroup.Name == "<Add")
            {
                await SaveNewSynergistToExercise();
            }
            else if (ExerciseId != "0")
            {
                ViewModel.SelectedSynergist = selected;

                await TryToSaveSelectedSynergistToExercise();
            }
        }

        private async Task TryToSaveSelectedSynergistToExercise()
        {
            try
            {
                ViewModel.SaveSynergistToExercise();

                await PageNavigation.NavigateTo(nameof(ExerciseAddEditPage)
                                              , nameof(ExerciseAddEditPage.ExerciseId)
                                              , ExerciseId
                                              , nameof(ExerciseAddEditPage.WorkoutId)
                                              , WorkoutId);
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

        private async Task SaveNewSynergistToExercise()
        {
            var muscleGroupName = await DisplayPromptAsync("New Muscle Group"
                                                         , "Enter the name of the new Muscle Group:"
                                                         , "OK"
                                                         , "Cancel"
                                                         , "Name"
                                                         , -1
                                                         , Keyboard.Create(KeyboardFlags.CapitalizeWord)
                                                         , "");

            var opposingMuscleGroupName = await DisplayPromptAsync("Assign Opposing Muscle Group"
                                                                 , "Name the Opposing Muscle Group.\r\nIf there is no Opposing Muscle Group, press Cancel"
                                                                 , "OK"
                                                                 , "Cancel"
                                                                 , "Name"
                                                                 , -1
                                                                 , Keyboard.Create(KeyboardFlags.CapitalizeWord)
                                                                 , "");

            var viewModel = new MuscleGroupViewModel(ExerciseId);

            try
            {
                viewModel.SaveNewSynergist(muscleGroupName
                                         , opposingMuscleGroupName);

                //if (await DisplayAlert("Add Opposite Synergist?"
                //                     , $"Would you like to add the opposite of this? i.e. Add {opposingMuscleGroupName} opposes {muscleGroupName}?"
                //                     , "Yes"
                //                     , "No"))
                //{
                //    viewModel.SaveOppositeSynergist(muscleGroupName, opposingMuscleGroupName);
                //}

                //BENDO: I think it is best to just add the opposite muscle group relationship, instead of asking the user.
                //However, if Owen want to be asked to add the opposite instead of doing automatically, then uncomment the above.
                //Furthermore, as it is currently written, if the user says No to the question above, there is not way to add the opposite afterwards.
                
                viewModel.SaveOppositeSynergist(muscleGroupName
                                              , opposingMuscleGroupName);

                await PageNavigation.NavigateBackwards();
            }
            catch (AttemptToAddDuplicateEntityException e)
            {
                await DisplayAlert("Muscle Group Already Exists"
                                 , $"The Muscle Group '{muscleGroupName}' already exists.  Name the Muscle Group with a different name."
                                 , "OK");
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Something went wrong while trying to save the Synergist.  View the log for more details"
                               , Category.Error
                               , ex);
            }
        }

        private async void MuscleGroupPicker_OnCancelButtonClicked(object                    sender
                                                                 , SelectionChangedEventArgs e)
        {
            await PageNavigation.NavigateBackwards();
        }
    }
}
