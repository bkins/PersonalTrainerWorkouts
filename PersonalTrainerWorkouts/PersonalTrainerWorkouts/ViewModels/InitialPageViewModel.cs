using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class InitialPageViewModel: ViewModelBase
    {
        public ObservableCollection<Workout> ListOfWorkouts
        {
            get => new ObservableCollection<Workout>(App.Database.GetWorkouts());
            set { }
        }

        public InitialPageViewModel()
        {
            ListOfWorkouts = new ObservableCollection<Workout>(App.Database.GetWorkouts());
        }

        public string Delete(int index)
        {
            if (index > ListOfWorkouts.Count - 1)
            {
                return string.Empty;
            }

            //Get the workout to be deleted
            var workoutToDelete = ListOfWorkouts[index];
            var workoutName     = workoutToDelete.Name;

            //Remove the workout from the source list
            ListOfWorkouts.RemoveAt(index);

            //Delete the Workout from the database
            App.Database.DeleteWorkout(ref workoutToDelete);
            
            ListOfWorkouts = new ObservableCollection<Workout>(App.Database.GetWorkouts());

            return workoutName;
        }
    }
}
