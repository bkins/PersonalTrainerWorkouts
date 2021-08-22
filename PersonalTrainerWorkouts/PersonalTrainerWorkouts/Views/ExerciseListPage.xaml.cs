using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.HelperModels;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels;
using Syncfusion.DataSource.Extensions;
using Syncfusion.GridCommon.ScrollAxis;
using Syncfusion.ListView.XForms;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(WorkoutId), nameof(WorkoutId))]
    public partial class ExerciseListPage : ContentPage
    {
        private string                      _workoutId = "0";
        private WorkoutExerciseWithChildren DrugExercise;
        public  ExerciseListViewModel       ViewModel;

        public string WorkoutId
        {
            get => _workoutId;
            set => LoadExercises(value);
        }
        
       
        async void LoadExercises(string itemId)
        {
            try
            {
                _workoutId = itemId;
                var id                       = Convert.ToInt32(itemId);
                var workoutExerciseViewModel = new WorkoutExerciseViewModel(itemId);
                ViewModel = new ExerciseListViewModel(id);

                ExerciseList.ItemsSource = ViewModel.LinkWorkoutExercises;

                //ExerciseList.ItemsSource = App.Database.GetWorkout(id)
                //                              .Exercises;
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Failed to load Exercises.", Category.Error, ex);
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

        private async void OnAddNewClicked(object     sender
                                         , EventArgs e)
        {
            //var path = $"{nameof(ExerciseNewEntryPage)}?{nameof(ExerciseNewEntryPage.WorkoutId)}={WorkoutId}";
            //await Shell.Current.GoToAsync(path);
            await PageNavigation.NavigateTo(nameof(ExerciseNewEntryPage), nameof(ExerciseNewEntryPage.WorkoutId), WorkoutId, nameof(ExerciseNewEntryPage.ExerciseId), "0");
        }

        private async void OnAddExistingClicked(object     sender
                                              , EventArgs e)
        {
            ExercisePicker.IsVisible = true;
            
            //var path = $"{nameof(ExerciseExistingEntryPage)}?{nameof(ExerciseExistingEntryPage.ItemId)}={WorkoutId}";
            //await Shell.Current.GoToAsync(path);

            await PageNavigation.NavigateTo(nameof(ExerciseExistingEntryPage)
                                          , nameof(ExerciseExistingEntryPage.ItemId)
                                          , WorkoutId);
        }

        private async void OnSelectionChanged(object                    sender,
                                              SelectionChangedEventArgs e)
        {
            //var path = $"{nameof(ExerciseExistingEntryPage)}?{nameof(ExerciseExistingEntryPage.ItemId)}={WorkoutId}";
            //await Shell.Current.GoToAsync(path);

            await PageNavigation.NavigateTo(nameof(ExerciseExistingEntryPage)
                                          , nameof(ExerciseExistingEntryPage.ItemId)
                                          , WorkoutId);
        }

        private void SelectionChangedPicker(object                                               sender
                                          , Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ClearLogsButton_OnClickedClearLogsButton(object    sender
                                                            , EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExercisePicker_OnSelectionChanged(object                                               sender
                                                     , Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void ExerciseList_OnItemDragging(object                sender
                                               , ItemDraggingEventArgs e)
        {
            switch (e.Action)
            {
                case DragAction.Start:
                    
                    DrugExercise = (WorkoutExerciseWithChildren) e.ItemData;

                    break;

                case DragAction.Dragging:

                    break;
                
                case DragAction.Drop:

                    await Task.Delay(100);
                    ChangeData(e);
                    
                    return;

                    //if (DrugExercise.WorkoutExercise.OrderBy == e.NewIndex)
                    //{
                    //    return;
                    //}
                    
                    //ViewModel.LinkWorkoutExercises.MoveTo(e.OldIndex, e.NewIndex);

                    ////Assigns the OrderBy to the original order
                    //for (int i = 0; i < ExerciseList.DataSource.DisplayItems.Count-1; i++)
                    //{
                    //    var currentItem     = ExerciseList.DataSource.DisplayItems[i];
                    //    var workoutExercise = (WorkoutExerciseWithChildren) currentItem;

                    //    workoutExercise.WorkoutExercise.OrderBy = i;
                    //    App.Database.UpdateWorkoutExercises(workoutExercise.WorkoutExercise);
                    //}
                    //return;

                    //var movedDown = (e.NewIndex - e.OldIndex) > 0;

                    //foreach (var exercise in (List<Exercise>)ExerciseList.ItemsSource)
                    //{
                    //    //BENDO:  If the same exercise is in a workout more than once, how will I know which exercise I am moving?
                    //    //Answer: Change the the binding to a WorkoutExercises viewModel, and reference the Exercise associated with
                    //    //        the WorkoutExercise in the viewModel
                    //    var workoutExercise = App.Database.GetWorkoutExercises(Convert.ToInt32(_workoutId), DrugExercise.Id);
                    //}
                    

                    //DrugExercise.WorkoutExercise.OrderBy = e.NewIndex;

                    //foreach (var workoutExercise in ViewModel.LinkWorkoutExercises.Where(field=> field.WorkoutExercise.Id 
                    //                                                                      != DrugExercise.WorkoutExercise.Id))
                    //{
                    //    if ( ! movedDown && workoutExercise.WorkoutExercise.OrderBy >= e.NewIndex)
                    //    {
                    //        workoutExercise.WorkoutExercise.OrderBy += 1;
                    //    }
                    //    else if (movedDown && workoutExercise.WorkoutExercise.OrderBy <= e.NewIndex)
                    //    {
                    //        workoutExercise.WorkoutExercise.OrderBy -= 1;
                    //    }
                    //    //BENDO:  It is not save the order after dragging the item :-(
                    //    workoutExercise.Save();
                    //}
                    //break;
                    
            }
        }

        public  void ChangeData(ItemDraggingEventArgs e)
        {
            var exerciseListChanges = new StringBuilder();

            exerciseListChanges.AppendLine("BEFORE");
            
            foreach (var workoutExercise in ViewModel.LinkWorkoutExercises)
            {
                exerciseListChanges.AppendLine(workoutExercise.ExerciseForDebugging);
            }

            ViewModel.LinkWorkoutExercises.MoveTo(e.OldIndex, e.NewIndex);
            for (int i = 0; i < ViewModel.LinkWorkoutExercises.Count; i++)
            {
                ViewModel.LinkWorkoutExercises[i].WorkoutExercise.OrderBy = i;
                ViewModel.LinkWorkoutExercises[i].Save();
            }

            ViewModel.RefreshData();
            
            ExerciseList.ItemsSource = ViewModel.LinkWorkoutExercises;

            exerciseListChanges.AppendLine("AFTER");

            foreach (var workoutExercise in ViewModel.LinkWorkoutExercises)
            {
                exerciseListChanges.AppendLine(workoutExercise.ExerciseForDebugging);
            }
            
            var beforeChange = exerciseListChanges.ToString();

        }
    }

    public class SfPickerBehavior:Behavior<Syncfusion.SfPicker.XForms.SfPicker>
    {
        protected override void OnAttachedTo(Syncfusion.SfPicker.XForms.SfPicker bindable)
        {
            base.OnAttachedTo(bindable);
        }
        protected override void OnDetachingFrom(Syncfusion.SfPicker.XForms.SfPicker bindable)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                (bindable as Syncfusion.SfPicker.XForms.SfPicker).Dispose();
            }
            base.OnDetachingFrom(bindable);
        }
    }
}