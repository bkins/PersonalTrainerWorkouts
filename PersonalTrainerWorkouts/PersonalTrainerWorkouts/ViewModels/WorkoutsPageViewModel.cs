using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Services;
using PersonalTrainerWorkouts.Views;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class WorkoutsPageViewModel : ViewModelBase
    {
        //BENDO: This isn't quite working.  I tried a little different approach.  See link below for example:
        //https://medium.com/swlh/xamarin-forms-mvvm-how-to-work-with-sqlite-db-c-xaml-26fcae303edd

        private WorkoutViewModel _selectedWorkout;
        private IDataStore       _dataStore;
        private IPageService     _pageService;

        private bool _isDataLoaded;

        public ObservableCollection<Workout> Workouts { get; private set; } 
            = new ObservableCollection<Workout>();

        public WorkoutViewModel SelectedWorkout
        {
            get => _selectedWorkout;
            set => SetValue(ref _selectedWorkout, value);
        }

        
        public ICommand LoadDataCommand      { get; private set; }
        public ICommand AddWorkoutCommand    { get; private set; }
        public ICommand SelectWorkoutCommand { get; private set; }
        public ICommand DeleteWorkoutCommand { get; private set; }

        public WorkoutsPageViewModel(IDataStore   dataStore
                                   , IPageService pageService)
        {
            _dataStore   = dataStore;
            _pageService = pageService;

            LoadDataCommand      = new Command( LoadData);
            AddWorkoutCommand    = new Command( async () => await AddWorkout());
            SelectWorkoutCommand = new Command<Workout>( async viewModel => await SelectWorkout(viewModel)); 
            DeleteWorkoutCommand = new Command<Workout>( async viewModel => await DeleteWorkout(viewModel));

        }

        private  void LoadData()
        {
            if (_isDataLoaded)
                return;

            _isDataLoaded = true;
            var workouts =  _dataStore.GetWorkouts();

            foreach (var workout in workouts)
            {
                Workouts.Add(workout);
            }
        }
        
        private async Task AddWorkout()
        {
            //BENDO: Modify WorkoutEntryPage to take a WorkoutViewModel
            //await _pageService.PushAsync(new WorkoutEntryPage(new WorkoutViewModel()));
        }
        
        private async Task SelectWorkout(Workout workoutViewModel)
        {
            if (workoutViewModel == null)
                return;

            SelectedWorkout = null;

            //BENDO: Modify WorkoutEntryPage to take a WorkoutViewModel
            //await _pageService.PushAsync(new WorkoutEntryPage(workoutViewModel));
        }
        
        private async Task DeleteWorkout(Workout workoutViewModel)
        {
            if (await _pageService.DisplayAlert("Warning", $"Are you sure you want to delete {workoutViewModel.Name}?", "Yes", "No"))
            {
                Workouts.Remove(workoutViewModel);

                var workout = _dataStore.GetWorkout(workoutViewModel.Id);
                _dataStore.DeleteWorkout(ref workout);
            }
        }
    }
}
