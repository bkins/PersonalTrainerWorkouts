using System.Collections.Generic;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class EquipmentListViewModel : ViewModelBase
    {
        public IEnumerable<Equipment> ListOfAllExerciseEquipment => DataAccessLayer.GetAllEquipment();
        public Equipment              SelectedEquipment          { get; set; }

        public void SaveExerciseEquipment(int exerciseId)
        {
            if (SelectedEquipment == null)
            {
                return;
            }

            DataAccessLayer.AddExerciseEquipment(exerciseId
                                               , SelectedEquipment.Id);
        }
    }
}
