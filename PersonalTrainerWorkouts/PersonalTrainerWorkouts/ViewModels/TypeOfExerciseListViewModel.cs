using System;
using System.Collections.Generic;
using System.Text;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    class TypeOfExerciseListViewModel : ViewModelBase
    {
        public IEnumerable<TypeOfExercise> ListOfAllExerciseTypes => DataAccessLayer.GetAllTypesOfExercise();
        public TypeOfExercise              SelectedTypeOfExercise { get; set; }

        public TypeOfExerciseListViewModel()
        {
            //BENDO:  Just for testing, until I wire up the adding of TypeOfExercises. But remember you cannot add duplicate Names
            //So only execute these lines if the data is erased.
            //DataAccessLayer.AddNewTypeOfExercise(new TypeOfExercise
            //                                     {
            //                                         Name = "Push"
            //                                     });

            //DataAccessLayer.AddNewTypeOfExercise(new TypeOfExercise
            //                                     {
            //                                         Name = "Pull"
            //                                     });

            //ListOfAllExerciseTypes = DataAccessLayer.GetAllTypesOfExercise();
        }

        public void SaveExerciseType(int exerciseId)
        {
            DataAccessLayer.AddExerciseType(exerciseId
                                          , SelectedTypeOfExercise.Id);
        }
    }
}
