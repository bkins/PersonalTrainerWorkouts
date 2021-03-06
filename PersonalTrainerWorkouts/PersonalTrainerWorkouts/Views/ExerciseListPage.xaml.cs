using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using ItemTappedEventArgs = Syncfusion.ListView.XForms.ItemTappedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(WorkoutId)
                 , nameof(WorkoutId))]
    public partial class ExerciseListPage
    {
        private string                      _workoutId = "0";
        private WorkoutExerciseWithChildren _drugExercise;
        public  ExerciseListViewModel       ViewModel;

        public string WorkoutId
        {
            get => _workoutId;
            set => LoadExercises(value);
        }

        private void LoadExercises(string itemId)
        {
            try
            {
                _workoutId = itemId;
                var workoutId = int.Parse(itemId);

                ViewModel = new ExerciseListViewModel(workoutId);

                ExerciseList.ItemsSource = ViewModel.LinkWorkoutExercises;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to load Exercises."
                               , Category.Error
                               , ex);

                //BENDO: consider implementing a page that shows exception details
            }
        }

        public ExerciseListPage()
        {
            InitializeComponent();
            ExerciseList.DataSource.LiveDataUpdateMode = Syncfusion.DataSource.LiveDataUpdateMode.AllowDataShaping;
        }

        protected override void OnAppearing()
        {
            ReloadExercises();
        }

        private void ReloadExercises()
        {
            if (_workoutId != "0")
            {
                LoadExercises(_workoutId);
            }
        }

        private async void OnAddNewClicked(object    sender
                                         , EventArgs e)
        {
            await PageNavigation.NavigateTo(nameof(ExerciseAddEditPage)
                                          , nameof(ExerciseAddEditPage.WorkoutId)
                                          , WorkoutId
                                          , nameof(ExerciseAddEditPage.ExerciseId)
                                          , "0");
        }

        private async void OnAddExistingClicked(object    sender
                                              , EventArgs e)
        {
            await PageNavigation.NavigateTo(nameof(ExerciseExistingEntryPage)
                                          , nameof(ExerciseExistingEntryPage.WorkoutId)
                                          , WorkoutId);
        }

        private async void OnSelectionChanged(object                    sender
                                            , SelectionChangedEventArgs e)
        {
            await PageNavigation.NavigateTo(nameof(ExerciseExistingEntryPage)
                                          , nameof(ExerciseExistingEntryPage.WorkoutId)
                                          , WorkoutId);
        }

        private async void ExerciseList_OnItemDragging(object                sender
                                                     , ItemDraggingEventArgs e)
        {
            switch (e.Action)
            {
                case DragAction.Start:

                    _drugExercise = (WorkoutExerciseWithChildren)e.ItemData;

                    break;

                case DragAction.Dragging:

                    break;

                case DragAction.Drop:

                    await Task.Delay(100);
                    MoveItem(e);

                    break;

                default:

                    Logger.WriteLine($"While dragging the Exercise, something went terribly wrong.  Please try again."
                                   , Category.Warning);

                    break;
            }
        }

        public void MoveItem(ItemDraggingEventArgs itemMoved)
        {
            ViewModel.LinkWorkoutExercises.MoveTo(itemMoved.OldIndex
                                                , itemMoved.NewIndex);

            for (var i = 0; i < ViewModel.LinkWorkoutExercises.Count; i++)
            {
                ViewModel.LinkWorkoutExercises[i]
                         .WorkoutExercise.OrderBy = i;

                ViewModel.LinkWorkoutExercises[i]
                         .Save();
            }

            ViewModel.RefreshData();

            ExerciseList.ItemsSource = ViewModel.LinkWorkoutExercises;
        }

        private async void ExerciseList_OnItemTapped(object              sender
                                                   , ItemTappedEventArgs e)
        {
            var tappedExercise = (WorkoutExerciseWithChildren)e.ItemData;

            await PageNavigation.NavigateTo(nameof(ExerciseAddEditPage)
                                          , nameof(ExerciseAddEditPage.WorkoutId)
                                          , tappedExercise.Workout.Id.ToString()
                                          , nameof(ExerciseAddEditPage.ExerciseId)
                                          , tappedExercise.Exercise.Id.ToString());
        }
    }
}
