using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class WorkoutExerciseRawViewModel : ViewModelBase
    {
        public List<WorkoutExercise>                 WorkoutExercisesData   { get; }
        public ObservableCollection<WorkoutExercise> ListOfWorkoutExercises { get; set; }
        
        public WorkoutExerciseRawViewModel()
        {
            //WorkoutExercisesData   = App.AsyncDatabase.WorkoutExercises;
            WorkoutExercisesData   = new List<WorkoutExercise>(App.Database.GetWorkoutExercises());
            ListOfWorkoutExercises = new ObservableCollection<WorkoutExercise>(WorkoutExercisesData);
        }
    }
}
