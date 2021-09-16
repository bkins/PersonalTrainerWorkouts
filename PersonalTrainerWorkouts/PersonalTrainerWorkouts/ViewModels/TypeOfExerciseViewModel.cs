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
            Logger.WriteLine($"Getting ExerciseId: {exerciseId}"
                           , Category.Information);

            Exercise = DataAccessLayer.GetExercise(int.Parse(exerciseId));

            Logger.WriteLine($"Got Exercise: {Exercise.Name} ({Exercise.Id})"
                           , Category.Information);
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

            Logger.WriteLine($"New Type {newType} ({newType.Id}) added."
                           , Category.Information);

            Logger.WriteLine($"Adding Exercise Type: ExerciseId: {Exercise.Id}; TypeId: {newType.Id}."
                           , Category.Information);

            DataAccessLayer.AddExerciseType(Exercise.Id
                                          , newType.Id);
        }
    }
}
