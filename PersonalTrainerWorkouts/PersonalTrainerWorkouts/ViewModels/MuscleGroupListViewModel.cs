using System;
using System.Collections.Generic;
using System.Text;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class MuscleGroupListViewModel : ViewModelBase
    {
        
        public IEnumerable<MuscleGroup> ListOfAllExerciseMuscleGroup => DataAccessLayer.GetAllMuscleGroups();
        public MuscleGroup              SelectedMuscleGroup          { get; set; }

        public MuscleGroupListViewModel()
        {

        }

        public void SaveExerciseMuscleGroup(int exerciseId)
        {
            if (SelectedMuscleGroup == null)
            {
                return;
            }

            throw new NotImplementedException();

            DataAccessLayer.AddExerciseEquipment(exerciseId
                                               , SelectedMuscleGroup.Id);
        }
    }
}
