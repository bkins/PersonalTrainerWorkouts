using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class WorkoutEntryViewModel : ViewModelBase
    {
        public  DataAccess DataAccessLayer => new DataAccess(App.Database);

        private int     _Id                                { get; set; }
        public  Workout NewWorkout                         { get; set; }
        public  bool    ManageExerciseToolBarItemIsEnabled { get; set; }
        
        public WorkoutEntryViewModel()
        {
            NewWorkout = new Workout();
            _Id        = 0;
        }

        public void SaveWorkout()
        {
            if (_Id == 0)
            {
                _Id = ! string.IsNullOrWhiteSpace(NewWorkout.Name) ?
                              DataAccessLayer.AddNewWorkout(NewWorkout) :
                              throw new UnnamedEntityException($"{nameof(Workout)} was not named.  Must be named before attempting to save.");
            }
            else
            {
                DataAccessLayer.UpdateWorkout(NewWorkout);
            }
            
        }
        
    }
}
