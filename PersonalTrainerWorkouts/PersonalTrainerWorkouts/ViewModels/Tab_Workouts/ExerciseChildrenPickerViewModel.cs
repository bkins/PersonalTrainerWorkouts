using System;
using System.Linq;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Helpers;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Helpers.Enums;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts;

/// <summary>
/// ViewModel to handle populating the pickers in the Add/Edit Exercise page
/// </summary>
public class ExerciseChildrenPickerViewModel
{
    private ExercisePickerSwitch PickerSwitch { get; set; }

    public ExerciseChildrenPickerViewModel(ExercisePickerSwitch pickerSwitch)
    {
        PickerSwitch = pickerSwitch;
    }

    public IOrderedEnumerable<PickerViewModel> GetChildListForPicker()
    {
        return PickerSwitch switch
               {
                   ExercisePickerSwitch.Type        => new TypeOfExerciseListViewModel().ListForDisplay
                 , ExercisePickerSwitch.Equipment   => new EquipmentViewModel().ListForDisplay
                 , ExercisePickerSwitch.MuscleGroup => new ResolvedSynergistViewModel().ListForDisplay
                 , _ => throw new ArgumentOutOfRangeException()
               };
    }
}
