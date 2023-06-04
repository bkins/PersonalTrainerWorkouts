using Avails.D_Flat.Extensions;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
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

            var newTypeOfExerciseId = DataAccessLayer.AddNewTypeOfExercise(newType);

            if (newTypeOfExerciseId == -1)
            {
                Logger.WriteLineToToastForced("Could not add new type.", Category.Error);
                return;
            }
            
            Logger.WriteLine($"New Type {newType} ({newType.Id}) added."
                           , Category.Information);

            Logger.WriteLine($"Adding Exercise Type: ExerciseId: {Exercise.Id}; TypeId: {newType.Id}."
                           , Category.Information);

            DataAccessLayer.AddExerciseType(Exercise.Id
                                          , newType.Id);
        }
    }
}
