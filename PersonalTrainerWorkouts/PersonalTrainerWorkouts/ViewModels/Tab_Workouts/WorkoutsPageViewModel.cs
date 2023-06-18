using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using PersonalTrainerWorkouts.Data.Interfaces;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Services;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    [System.Obsolete("This ViewModel and the View WorkoutPage are an incomplete trial of a different approach I would like to explore more later.  For now they are not used.")]
    public class WorkoutsPageViewModel : ViewModelBase
    {
        //BENDO: This isn't quite working.  I tried a little different approach.  See link below for example:
        //https://medium.com/swlh/xamarin-forms-mvvm-how-to-work-with-sqlite-db-c-xaml-26fcae303edd

        private          WorkoutViewModel _selectedWorkout;
        private readonly IDataStore       _dataStore;
        private readonly IPageService     _pageService;

        private bool _isDataLoaded;

        public ObservableCollection<Workout> Workouts { get; } = new ObservableCollection<Workout>();

        public WorkoutViewModel SelectedWorkout
        {
            get => _selectedWorkout;
            set => SetValue(ref _selectedWorkout
                          , value);
        }

        public ICommand LoadDataCommand      { get; }
        public ICommand SelectWorkoutCommand { get; }
        public ICommand DeleteWorkoutCommand { get; }

        public WorkoutsPageViewModel(IDataStore   dataStore
                                   , IPageService pageService)
        {
            _dataStore   = dataStore;
            _pageService = pageService;

            LoadDataCommand      = new Command(LoadData);
            SelectWorkoutCommand = new Command<Workout>(SelectWorkout);
            DeleteWorkoutCommand = new Command<Workout>(async viewModel => await DeleteWorkout(viewModel));
        }

        private void LoadData()
        {
            if (_isDataLoaded)
                return;

            _isDataLoaded = true;
            var workouts = _dataStore.GetWorkouts();

            foreach (var workout in workouts)
            {
                Workouts.Add(workout);
            }
        }

        private void SelectWorkout(Workout workoutViewModel)
        {
            if (workoutViewModel == null)
                return;

            SelectedWorkout = null;
        }

        private async Task DeleteWorkout(Workout workoutViewModel)
        {
            if (await _pageService.DisplayAlert("Warning"
                                              , $"Are you sure you want to delete {workoutViewModel.Name}?"
                                              , "Yes"
                                              , "No"))
            {
                Workouts.Remove(workoutViewModel);

                var workout = _dataStore.GetWorkout(workoutViewModel.Id);
                _dataStore.DeleteWorkout(ref workout);
            }
        }
    }
}
