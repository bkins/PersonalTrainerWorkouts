using System;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class WorkoutViewModel : ViewModelBase
    {
        public int Id { get; set; }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                SetValue(ref _name
                       , value);

                OnPropertyChanged();
            }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set
            {
                SetValue(ref _description
                       , value);

                OnPropertyChanged();
            }
        }

        private int _difficulty;

        public int Difficulty
        {
            get => _difficulty;
            set
            {
                SetValue(ref _difficulty
                       , value);

                OnPropertyChanged();
            }
        }

        private DateTime _createDateTime;

        public DateTime CreateDateTime
        {
            get => _createDateTime;
            set
            {
                SetValue(ref _createDateTime
                       , value);

                OnPropertyChanged(nameof(CreateDateTime));
            }
        }

        public WorkoutViewModel(Database database
                              , int      workoutId)
        {
            Id = workoutId;
        }

        public WorkoutViewModel(Workout workout)
        {
            Id              = workout.Id;
            _name           = workout.Name;
            _description    = workout.Description;
            _difficulty     = workout.Difficulty;
            _createDateTime = workout.CreateDateTime;
        }
    }
}
