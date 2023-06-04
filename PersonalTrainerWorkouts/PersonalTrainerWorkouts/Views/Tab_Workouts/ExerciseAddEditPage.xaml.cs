using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avails.D_Flat.Exceptions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views.Tab_Workouts
{
    [QueryProperty(nameof(WorkoutId)
                 , nameof(WorkoutId))]
    [QueryProperty(nameof(ExerciseId)
                 , nameof(ExerciseId))]
    public partial class ExerciseAddEditPage : IQueryAttributable
    {
        private ExerciseAddEditViewModel ViewModel             { get; set; }
        private MuscleGroupViewModel     MuscleGroupsViewModel { get; set; }

        public  string               WorkoutId           { get; set; }
        public  string               ExerciseId          { get; set; }
        public  string               InitialName         { get; set; }
        public  string               InitialDescription  { get; set; }
        public  string               InitialLengthOfTime { get; set; }
        public  int                  InitialReps         { get; set; }
        private Entry                NameEntry           { get; set; }
        public  List<TypeOfExercise> TypesOfExerciseList { get; set; }
        //public  ICommand             DeleteCommand       { get; }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                // The query parameter requires URL decoding.
                WorkoutId  = HttpUtility.UrlDecode(query[nameof(WorkoutId)]);
                ExerciseId = HttpUtility.UrlDecode(query[nameof(ExerciseId)]);

                ViewModel = new ExerciseAddEditViewModel(int.Parse(WorkoutId)
                                                       , int.Parse(ExerciseId));

                MuscleGroupsViewModel = new MuscleGroupViewModel(ExerciseId);
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
                if (ExerciseId != "0")
                {
                    InitialName         = ViewModel.Exercise?.Name;
                    InitialDescription  = ViewModel.Exercise?.Description;
                    InitialLengthOfTime = ViewModel.Exercise?.LengthOfTime;
                    TypesOfExerciseList = ViewModel.TypesOfExerciseList;

                    BindingContext                           = ViewModel;
                    TypeOfExerciseCollectionView.ItemsSource = ViewModel.TypesOfExerciseList;
                    EquipmentCollectionView.ItemsSource      = ViewModel.EquipmentList;

                    MuscleGroupCollectionView.ItemsSource = MuscleGroupsViewModel.Synergists.Where(field => field.Exercise.Id == int.Parse(ExerciseId));
                }
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

            ViewModel      = new ExerciseAddEditViewModel();
            BindingContext = ViewModel;
        }

        /// <summary>
        /// Used to Update the Exercise anytime the Name, Description, LengthOfTime, or Reps are changed.
        /// </summary>
        private void UpdateExercise()
        {
            ViewModel.Exercise = GetContextExercise();
            ViewModel.UpdateExercise();
        }

        /// <summary>
        /// Used to save a new Exercise
        /// </summary>
        private void SaveExercise()
        {
            ViewModel.Exercise = GetContextExercise();

            try
            {
                ViewModel.SaveExercise(int.Parse(WorkoutId));
                ExerciseId          = ViewModel.Exercise.Id.ToString();
                InitialName         = ViewModel.Exercise.Name;
                InitialDescription  = ViewModel.Exercise?.Description;
                InitialLengthOfTime = ViewModel.Exercise?.LengthOfTime;
            }
            catch (AttemptToAddDuplicateEntityException e)
            {
                Logger.WriteLine($"An exercise with the name {ViewModel.Exercise.Name} already exists.  Please either add that exercise or use a different name."
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

        /// <summary>
        /// Gets exercise entered by the user, or if the exercise the user is entering already exists,
        /// the user will be if add the existing one.  If they say no, then the user will be able to edit
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

            if (InitialReps != int.Parse(reps.Text))
            {
                UpdateExercise();
            }
        }

        private void AddTypeOfExerciseButton_OnClicked(object    sender
                                                           , EventArgs e)
        {
            PageNavigation.NavigateTo(nameof(TypeOfExerciseListPage)
                                          , nameof(TypeOfExerciseListPage.WorkoutId)
                                          , ViewModel.Workout.Id.ToString()
                                          , nameof(TypeOfExerciseListPage.ExerciseId)
                                          , ViewModel.Exercise.Id.ToString());
        }

        private void RemoveType_OnClicked(object    sender
                                        , EventArgs e)
        {
            var itemToDelete = (Button)sender;
            ViewModel.DeleteExerciseType(int.Parse(itemToDelete.Text));

            TypeOfExerciseCollectionView.ItemsSource = ViewModel.TypesOfExerciseList;
        }

        private void AddEquipmentButton_OnClicked(object    sender
                                                      , EventArgs e)
        {
            PageNavigation.NavigateTo(nameof(EquipmentListPage)
                                          , nameof(EquipmentListPage.ExerciseId)
                                          , ExerciseId);
        }

        private void AddMuscleGroupButton_OnClicked(object    sender
                                                        , EventArgs e)
        {
            PageNavigation.NavigateTo(nameof(MuscleGroupListPage)
                                          , nameof(MuscleGroupListPage.ExerciseId)
                                          , ExerciseId
                                          , nameof(MuscleGroupListPage.WorkoutId)
                                          , WorkoutId);
        }

        private void RemoveEquipment_OnClicked(object    sender
                                             , EventArgs e)
        {
            var itemToDelete = (Button)sender;
            ViewModel.DeleteExerciseEquipment(int.Parse(itemToDelete.Text));

            EquipmentCollectionView.ItemsSource = ViewModel.EquipmentList;
        }

        private void RemoveMuscleGroup_OnClicked(object    sender
                                               , EventArgs e)
        {
            var itemToDelete = (Button)sender;
            ViewModel.DeleteExerciseMuscleGroup(int.Parse(itemToDelete.Text));

            MuscleGroupCollectionView.ItemsSource = MuscleGroupsViewModel.Synergists.Where(field => field.Exercise.Id == int.Parse(ExerciseId));
        }
    }
}
