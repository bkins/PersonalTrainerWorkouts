using System;
using System.Collections.Generic;
using System.Text;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    class TypeOfExerciseViewModel : ViewModelBase
    {
        public Exercise Exercise { get; set; }

        public TypeOfExerciseViewModel(string exerciseId)
        {
            Exercise = DataAccessLayer.GetExercise(int.Parse(exerciseId));
        }
        
        public void SaveTypeOfExercise(string newTypeName)
        {
            if (newTypeName.IsNullEmptyOrWhitespace())
            {
                return;
            }

            var newType = new TypeOfExercise
                          {
                              Name = newTypeName
                          };

            DataAccessLayer.AddNewTypeOfExercise(newType);
            DataAccessLayer.AddExerciseType(Exercise.Id
                                          , newType.Id);
        }
    }
}
