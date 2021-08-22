using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
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
                SetValue(ref _name, value);
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                SetValue(ref _description, value);
                OnPropertyChanged(nameof(Description));
            }
        }

        private int _difficulty;
        public int Difficulty 
        {
            get => _difficulty;
            set
            {
                SetValue(ref _difficulty, value);
                OnPropertyChanged(nameof(Difficulty));
            }
        }

        private DateTime _createDateTime;
        public DateTime CreateDateTime 
        {
            get => _createDateTime;
            set
            {
                SetValue(ref _createDateTime, value);
                OnPropertyChanged(nameof(CreateDateTime));
            }
        }

        private Workout    _workout;
        private Database   _database;
        private DataAccess _dataAccess;
                
        public WorkoutViewModel(Database database, int workoutId)
        {
            _database   = database;
            _dataAccess = new DataAccess(_database);

            Id       = workoutId;
            //_workout = _dataAccess.GetWorkout(Id);
        }

        public WorkoutViewModel(Workout workout)
        {
            Id           = workout.Id;

            _name           = workout.Name;
            _description    = workout.Description;
            _difficulty     = workout.Difficulty;
            _createDateTime = workout.CreateDateTime;
        }
    }
}
