using System.Collections.Generic;
using System.Linq;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Helpers;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
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

        public void SetSelectedEquipmentById(int selectedId)
        {
            SelectedEquipment = DataAccessLayer.GetAllEquipment()
                                               ?.FirstOrDefault(equipment => equipment.Id == selectedId);

            if (SelectedEquipment is null)
            {
                Logger.WriteLineToToastForced($"Something went wrong! Check logs."
                                            , Category.Error
                                            , $"Looked for Equipment.Id ({selectedId}), but it was not found.");
            }
        }
    }
}
