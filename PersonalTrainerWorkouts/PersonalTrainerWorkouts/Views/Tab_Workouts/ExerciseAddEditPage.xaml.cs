using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Avails.ApplicationExceptions;
using Avails.D_Flat.Extensions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.HelperClasses;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Helpers;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Helpers.Enums;
using Xamarin.Forms;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Workouts
{
    [QueryProperty(nameof(WorkoutId)
                 , nameof(WorkoutId))]
    [QueryProperty(nameof(ExerciseId)
                 , nameof(ExerciseId))]
    public partial class ExerciseAddEditPage : IQueryAttributable
    {
        private ExerciseAddEditViewModel    ExerciseViewModel                   { get; set; }
        private TypeOfExerciseListViewModel TypeOfExerciseListViewModel { get; set; }
        private EquipmentListViewModel      EquipmentListViewModel      { get; set; }
        private MuscleGroupViewModel        MuscleGroupsViewModel       { get; set; }

        public  string               WorkoutId           { get; set; }
        public  string               ExerciseId          { get; set; }
        public  string               InitialName         { get; set; }
        public  string               InitialDescription  { get; set; }
        public  string               InitialLengthOfTime { get; set; }
        public  int                  InitialReps         { get; set; }
        private Entry                NameEntry           { get; set; }
        public  List<TypeOfExercise> TypesOfExerciseList { get; set; }
        //public  ICommand             DeleteCommand       { get; }

        public ExerciseAddEditPage(string workoutId, string exerciseId)
        {
            WorkoutId  = workoutId;
            ExerciseId = exerciseId;
        }
        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                // The query parameter requires URL decoding.
                WorkoutId  = HttpUtility.UrlDecode(query[nameof(WorkoutId)]);
                ExerciseId = HttpUtility.UrlDecode(query[nameof(ExerciseId)]);

                ExerciseViewModel = new ExerciseAddEditViewModel(int.Parse(WorkoutId)
                                                       , int.Parse(ExerciseId));
                TypeOfExerciseListViewModel = new TypeOfExerciseListViewModel();
                EquipmentListViewModel      = new EquipmentListViewModel();
                MuscleGroupsViewModel       = new MuscleGroupViewModel(ExerciseId);
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
            try
            {
                if (ExerciseId == "0") return;

                InitialName         = ExerciseViewModel.Exercise?.Name;
                InitialDescription  = ExerciseViewModel.Exercise?.Description;
                InitialLengthOfTime = ExerciseViewModel.Exercise?.LengthOfTime;
                TypesOfExerciseList = ExerciseViewModel.TypesOfExerciseList;

                BindingContext = ExerciseViewModel;

                TypeOfExerciseAccordionCollectionView.ItemsSource = ExerciseViewModel.TypesOfExerciseList;
                EquipmentAccordionCollectionView.ItemsSource      = ExerciseViewModel.EquipmentList;

                MuscleGroupAccordionCollectionView.ItemsSource = MuscleGroupsViewModel.Synergists
                                                                                      .Where(field => field.Exercise.Id
                                                                                                   == int.Parse(ExerciseId));
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed to load Exercise."
                               , Category.Error
                               , e);

                //BENDO: consider implementing a page that shows exception details
            }
        }

        public ExerciseAddEditPage()
        {
            InitializeComponent();

            ExerciseViewModel = new ExerciseAddEditViewModel();
            BindingContext    = ExerciseViewModel;
        }

        /// <summary>
        /// Used to Update the Exercise anytime the Name, Description, LengthOfTime, or Reps are changed.
        /// </summary>
        private void UpdateExercise()
        {
            ExerciseViewModel.Exercise = GetContextExercise();
            ExerciseViewModel.UpdateExercise();
        }

        /// <summary>
        /// Used to save a new Exercise
        /// </summary>
        private void SaveExercise()
        {
            ExerciseViewModel.Exercise = GetContextExercise();

            try
            {
                ExerciseViewModel.SaveExercise(int.Parse(WorkoutId));
                ExerciseId          = ExerciseViewModel.Exercise.Id.ToString();
                InitialName         = ExerciseViewModel.Exercise.Name;
                InitialDescription  = ExerciseViewModel.Exercise?.Description;
                InitialLengthOfTime = ExerciseViewModel.Exercise?.LengthOfTime;
            }
            catch (AttemptToAddDuplicateEntityException e)
            {
                Logger.WriteLine($"An exercise with the name {ExerciseViewModel.Exercise.Name} already exists.  Please either add that exercise or use a different name."
                               , Category.Error
                               , e);

                NameEntry.Focus();
            }
            catch (UnnamedEntityException e)
            {
                Logger.WriteLine("An exercise must have a name to save."
                               , Category.Error
                               , e);

                NameEntry.Focus();
            }
        }

        //TODO:  Determine if the comments are right or the code.
        //The comments below do not match what is happening in the method.

        /// <summary>
        /// Gets exercise entered by the user, or if the exercise the user is entering already exists,
        /// the user will be asked if it should add the existing one.  If they say no, then the user will be able to edit
        /// the exercise they are trying to enter. 
        /// </summary>
        /// <returns></returns>
        private Exercise GetContextExercise()
        {
            var exercise = (ExerciseAddEditViewModel)BindingContext;

            if (exercise.Exercise    == null
             || exercise.Exercise.Id == 0)
            {
                exercise.Exercise = new Exercise
                                    {
                                        Name = NameEntry.Text
                                    };
            }

            exercise.Exercise.LengthOfTime = exercise.LengthOfTime ?? "00:00";
            exercise.Exercise.Reps         = exercise.Reps         ?? 0;

            return exercise.Exercise;
        }

        private void Name_OnUnfocused(object         sender
                                    , FocusEventArgs e)
        {
            NameEntry = (Entry)sender;

            if (string.IsNullOrEmpty(InitialName))
            {
                SaveExercise();
            }
            else if (NameEntry.Text != InitialName)
            {
                UpdateExercise();
            }
        }

        private void Description_OnUnfocused(object         sender
                                           , FocusEventArgs e)
        {
            var description = (Editor)sender;

            if (InitialDescription != description.Text)
            {
                UpdateExercise();
            }
        }

        private void LengthOfTimeEditor_OnUnfocused(object         sender
                                                  , FocusEventArgs e)
        {
            var lengthOfTime = (Editor)sender;

            if (InitialLengthOfTime != lengthOfTime.Text)
            {
                UpdateExercise();
            }
        }

        private void RepsEditor_OnUnfocused(object         sender
                                          , FocusEventArgs e)
        {
            var reps = (Editor)sender;

            if (InitialReps != reps.Text.ToSafeInt())
            {
                UpdateExercise();
            }
        }

        private void AddTypeOfExerciseButton_OnClicked(object    sender
                                                     , EventArgs e)
        {
            var pickerSource = new ExerciseChildrenPickerViewModel(ExercisePickerSwitch.Type);
            TypeOfExercisePicker.ItemsSource = pickerSource.GetChildListForPicker();

            TypeOfExercisePicker.IsOpen    = true;
            TypeOfExercisePicker.IsVisible = true;

            //TypeOfExerciseAccordionCollectionView.ItemsSource = ExeciseViewModel.TypesOfExerciseList;
        }

        private IOrderedEnumerable<TypeOfExercise> GetTypeOfExerciseListForDisplay()
        {

            return TypeOfExerciseListViewModel.ListOfAllExerciseTypes
                                              .Concat(new[]
                                                      {
                                                          new TypeOfExercise
                                                          {
                                                              Name = Constants.AddNew
                                                          }
                                                      })
                                              .OrderBy(field => field.Id);
        }

        private void RemoveType_OnClicked(object    sender
                                        , EventArgs e)
        {
            var itemToDelete = (Button)sender;
            ExerciseViewModel.DeleteExerciseType(int.Parse(itemToDelete.Text));

            TypeOfExerciseAccordionCollectionView.ItemsSource = ExerciseViewModel.TypesOfExerciseList;
        }

        private void AddEquipmentButton_OnClicked(object    sender
                                                      , EventArgs e)
        {
            var pickerSource = new ExerciseChildrenPickerViewModel(ExercisePickerSwitch.Equipment);
            EquipmentPicker.ItemsSource = pickerSource.GetChildListForPicker(); //GetEquipmentListForDisplay();

            EquipmentPicker.IsOpen      = true;
            EquipmentPicker.IsVisible   = true;

        }

        private void AddMuscleGroupButton_OnClicked(object    sender
                                                        , EventArgs e)
        {
            DisplayAlert("Under Construction"
                       , "Adding Muscle Groups to Exercises is currently under construction."
                       , "OK");
            return;

            var pickerSource = new ExerciseChildrenPickerViewModel(ExercisePickerSwitch.MuscleGroup);
            MuscleGroupPicker.ItemsSource = pickerSource.GetChildListForPicker();

            MuscleGroupPicker.IsOpen      = true;
            MuscleGroupPicker.IsVisible   = true;

            // MuscleGroupAccordionCollectionView.ItemsSource
            //     = MuscleGroupsViewModel.Synergists
            //                            .Where(field => field.Exercise.Id
            //                                         == int.Parse(ExerciseId));
        }

        private IOrderedEnumerable<ResolvedSynergistViewModel> GetMuscleGroupForDisplay()
        {

            return MuscleGroupsViewModel.Synergists
                                        .Concat(new[]
                                                {
                                                    new ResolvedSynergistViewModel()
                                                })
                                        .OrderBy(field => field.Id);
        }

        private void RemoveEquipment_OnClicked(object    sender
                                             , EventArgs e)
        {
            var itemToDelete = (Button)sender;
            ExerciseViewModel.DeleteExerciseEquipment(int.Parse(itemToDelete.Text));

            EquipmentAccordionCollectionView.ItemsSource = ExerciseViewModel.EquipmentList;
        }

        private IOrderedEnumerable<Equipment> GetEquipmentListForDisplay()
        {

            return ExerciseViewModel.EquipmentList
                            .Concat(new[]
                                    {
                                        new Equipment
                                        {
                                            Name = Constants.AddNew
                                        }
                                    })
                            .OrderBy(field => field.Id);
        }

        private void RemoveMuscleGroup_OnClicked(object    sender
                                               , EventArgs e)
        {
            var itemToDelete = (Button)sender;
            ExerciseViewModel.DeleteExerciseMuscleGroup(int.Parse(itemToDelete.Text));

            MuscleGroupAccordionCollectionView.ItemsSource = MuscleGroupsViewModel.Synergists
                                                                         .Where(field => field.Exercise.Id == int.Parse(ExerciseId));
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
        private async void TypeOfExercisePicker_OnOkButtonClicked(object                    sender
                                                                , SelectionChangedEventArgs e)
        {
            var selected = (PickerViewModel)TypeOfExercisePicker.SelectedItem;

            if (selected.Name == Constants.AddNew)
            {
                await SaveNewExerciseType().ConfigureAwait(false);

                ExerciseViewModel.LoadTheTypesOfExercise();
                TypeOfExerciseAccordionCollectionView.ItemsSource = ExerciseViewModel.TypesOfExerciseList;

                var pickerSource = new ExerciseChildrenPickerViewModel(ExercisePickerSwitch.Type);
                TypeOfExercisePicker.ItemsSource = pickerSource.GetChildListForPicker(); //GetTypeOfExerciseListForDisplay();
            }
            else if (ExerciseId != "0")
            {
                TypeOfExerciseListViewModel.SetSelectedTypeOfExercise(selected.Id);

                await TryToSaveExerciseType().ConfigureAwait(false);
                // ExeciseViewModel.UpdateExercise();
                ExerciseViewModel.LoadTheTypesOfExercise();
                TypeOfExerciseAccordionCollectionView.ItemsSource = ExerciseViewModel.TypesOfExerciseList;

                TypeOfExercisePicker.IsOpen                       = false;
                TypeOfExercisePicker.IsVisible                    = false;
            }

        }
        private async Task TryToSaveExerciseType()
        {
            try
            {
                TypeOfExerciseListViewModel.SaveExerciseType(int.Parse(ExerciseId));
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
            TypeOfExercisePicker.IsOpen    = false;
            TypeOfExercisePicker.IsVisible = false;
        }

        private async void EquipmentPicker_OnOkButtonClicked(object                    sender
                                                           , SelectionChangedEventArgs e)
        {
            var selected = (PickerViewModel)EquipmentPicker.SelectedItem;

            if (selected.Name == Constants.AddNew)
            {
                await SaveNewExerciseEquipment();

                ExerciseViewModel.LoadTheEquipment();
                EquipmentAccordionCollectionView.ItemsSource = ExerciseViewModel.EquipmentList;

                var pickerSource = new ExerciseChildrenPickerViewModel(ExercisePickerSwitch.Equipment);
                EquipmentPicker.ItemsSource = pickerSource.GetChildListForPicker(); //GetEquipmentListForDisplay();
            }
            else if (ExerciseId != "0")
            {
                EquipmentListViewModel.SetSelectedEquipmentById(selected.Id);

                await TryToSaveExerciseEquipment();
                ExerciseViewModel.LoadTheEquipment();
                EquipmentAccordionCollectionView.ItemsSource = ExerciseViewModel.EquipmentList;

                EquipmentPicker.IsOpen    = false;
                EquipmentPicker.IsVisible = false;
            }
        }

        private async Task TryToSaveExerciseEquipment()
        {
            try
            {
                EquipmentListViewModel.SaveExerciseEquipment(int.Parse(ExerciseId));
            }
            catch (EntityRelationAlreadyExistsException alreadyExistsException)
            {
                await DisplayAlert(Category.Warning.ToString()
                                 , alreadyExistsException.Message
                                 , "OK").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Logger.WriteLine(exception.Message
                               , Category.Error
                               , exception);
            }
        }

        private async Task SaveNewExerciseEquipment()
        {
            var saved = false;

            while (! saved)
            {
                var equipmentName = await DisplayPromptAsync("New Equipment"
                                                           , "Enter the name of the new Equipment:"
                                                           , "OK"
                                                           , "Cancel"
                                                           , "Name"
                                                           , -1
                                                           , Keyboard.Create(KeyboardFlags.CapitalizeWord)
                                                           , "");

                var viewModel = new EquipmentViewModel(ExerciseId);

                try
                {
                    viewModel.SaveEquipment(equipmentName);

                    saved = true;
                }
                catch (AttemptToAddDuplicateEntityException)
                {
                    await DisplayAlert("Equipment Already Exists"
                                     , $"The Equipment '{equipmentName}' already exists.  Name the Equipment with a different name."
                                     , "OK");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Something went wrong while trying to save the Equipment.  View the log for more details"
                                   , Category.Error
                                   , ex);
                }
            }
        }

        private void EquipmentPicker_OnCancelButtonClicked(object                    sender
                                                         , SelectionChangedEventArgs e)
        {
            EquipmentPicker.IsOpen    = false;
            EquipmentPicker.IsVisible = false;
        }

        private async void MuscleGroupPicker_OnOkButtonClicked(object                    sender
                                                       , SelectionChangedEventArgs e)
        {
            var selected = (PickerViewModel)MuscleGroupPicker.SelectedItem;

            if (selected.Name == "<Add")
            {
                await SaveNewSynergistToExercise().ConfigureAwait(false);

                MuscleGroupsViewModel.SetSynergistsInExercise(ExerciseId);
                MuscleGroupAccordionCollectionView.ItemsSource
                    = MuscleGroupsViewModel.Synergists
                                           .Where(field => field.Exercise.Id
                                                        == int.Parse(ExerciseId));

                var pickerSource = new ExerciseChildrenPickerViewModel(ExercisePickerSwitch.MuscleGroup);
                MuscleGroupPicker.ItemsSource = pickerSource.GetChildListForPicker(); //GetMuscleGroupForDisplay();
            }
            //TODO: as it is now, you cannot add MuscleGroups to the Exercise if it is a new one
            else if (ExerciseId != "0")
            {
                var selectedSynergist = new ResolvedSynergistViewModel();
                MuscleGroupsViewModel.SelectedSynergist = new ResolvedSynergistViewModel(selected, ExerciseId.ToSafeInt()); //selected;

                await TryToSaveSelectedSynergistToExercise();

                MuscleGroupsViewModel.SetSynergistsInExercise(ExerciseId);
                MuscleGroupAccordionCollectionView.ItemsSource
                    = MuscleGroupsViewModel.Synergists
                                           .Where(field => field.Exercise.Id
                                                        == int.Parse(ExerciseId));

                MuscleGroupPicker.IsOpen    = false;
                MuscleGroupPicker.IsVisible = false;
            }
        }

        private void MuscleGroupPicker_OnCancelButtonClicked(object                    sender
                                                           , SelectionChangedEventArgs e)
        {
            MuscleGroupPicker.IsOpen    = false;
            MuscleGroupPicker.IsVisible = false;
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

                viewModel.SaveOppositeSynergist(muscleGroupName
                                              , opposingMuscleGroupName);

            }
            catch (AttemptToAddDuplicateEntityException e)
            {
                var message = $"The Muscle Group '{muscleGroupName}' already exists.  Name the Muscle Group with a different name.";
                await DisplayAlert("Muscle Group Already Exists"
                                 , message
                                 , "OK");
                Logger.WriteLine(message, Category.Error, e);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Something went wrong while trying to save the Synergist.  View the log for more details"
                               , Category.Error
                               , ex);
            }
        }
        private async Task<int> TryToSaveSelectedSynergistToExercise()
        {
            try
            {
                return MuscleGroupsViewModel.SaveSynergistToExercise();
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

            return 0;
        }
    }
}
