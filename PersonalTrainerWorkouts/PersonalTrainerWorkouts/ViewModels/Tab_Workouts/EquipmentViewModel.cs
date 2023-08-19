using System.Linq;
using Avails.D_Flat.Extensions;
using PersonalTrainerWorkouts.HelperClasses;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Helpers;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Interfaces;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class EquipmentViewModel : ViewModelBase, IViewModelPicker
    {
        public Exercise Exercise { get; set; }

        public IOrderedEnumerable<PickerViewModel> ListForDisplay { get; set; }

        public EquipmentViewModel()
        {
            var allExerciseChildren = DataAccessLayer.GetExercises();

            ListForDisplay = allExerciseChildren.Select(exerciseChild => new PickerViewModel
                                                                  {
                                                                      Id   = exerciseChild.Id
                                                                    , Name = exerciseChild.Name
                                                                  })
                                         .ToList()
                                         .Concat(new[]
                                                 {
                                                     new PickerViewModel
                                                     {
                                                         Name = Constants.AddNew
                                                     }
                                                 })
                                         .OrderBy(exerciseChild => exerciseChild.Id);
        }

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
