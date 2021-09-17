using System;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class EquipmentViewModel : ViewModelBase
    {
        public Exercise Exercise { get; set; }

        public EquipmentViewModel(string exerciseId)
        {
            Exercise = DataAccessLayer.GetExercise(int.Parse(exerciseId));
        }

        public void SaveEquipment(string equipmentName)
        {
            if (equipmentName.IsNullEmptyOrWhitespace())
            {
                return;
            }

            var newEquipment = new Equipment
                               {
                                   Name = equipmentName
                               };

            DataAccessLayer.AddNewEquipment(newEquipment);

            DataAccessLayer.AddExerciseEquipment(Exercise.Id
                                               , newEquipment.Id);
        }
    }
}
