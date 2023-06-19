using System.Collections.Generic;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class MuscleGroupListViewModel : ViewModelBase
    {
        public IEnumerable<MuscleGroup> ListOfAllExerciseMuscleGroup => DataAccessLayer.GetAllMuscleGroups();
        public MuscleGroup              SelectedMuscleGroup          { get; set; }

        public void SaveExerciseMuscleGroup(int exerciseId)
        {
            if (SelectedMuscleGroup == null)
            {
                return;
            }

            DataAccessLayer.AddExerciseEquipment(exerciseId
                                               , SelectedMuscleGroup.Id);
        }
    }
}
