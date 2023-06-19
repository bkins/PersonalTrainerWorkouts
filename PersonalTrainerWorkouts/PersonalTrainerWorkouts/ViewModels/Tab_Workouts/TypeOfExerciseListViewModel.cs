using System.Collections.Generic;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    class TypeOfExerciseListViewModel : ViewModelBase
    {
        public IEnumerable<TypeOfExercise> ListOfAllExerciseTypes => DataAccessLayer.GetAllTypesOfExercise();
        public TypeOfExercise              SelectedTypeOfExercise { get; set; }
        
        public void SaveExerciseType(int exerciseId)
        {
            if (SelectedTypeOfExercise == null)
            {
                return;
            }

            DataAccessLayer.AddExerciseType(exerciseId
                                          , SelectedTypeOfExercise.Id);
        }
    }
}
