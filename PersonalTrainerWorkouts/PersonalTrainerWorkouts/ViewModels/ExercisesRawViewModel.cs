using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PersonalTrainerWorkouts.Models;
using Syncfusion.DataSource.Extensions;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExercisesRawViewModel : ViewModelBase
    {
        public List<Exercise>                 ExercisesData   { get; }
        public ObservableCollection<Exercise> ListOfExercises { get; set; }
        
        public ExercisesRawViewModel()
        {
            //ExercisesData   = App.Database.Exercises;
            var arrayOfExercises = App.Database.GetExercises();

            ExercisesData   = new List<Exercise>();

            foreach (var exercise in arrayOfExercises)
            {
                ExercisesData.Add(exercise);
            }
            ListOfExercises = new ObservableCollection<Exercise>(ExercisesData);
        }
    }
}
