using System.Linq;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Helpers;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Interfaces;

public interface IViewModelPicker
{
    public IOrderedEnumerable<PickerViewModel> ListForDisplay { get; set; }
}
