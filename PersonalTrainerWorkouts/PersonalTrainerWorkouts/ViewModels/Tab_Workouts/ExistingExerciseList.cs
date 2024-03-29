using System.Collections.ObjectModel;
using PersonalTrainerWorkouts.Models;
using Syncfusion.SfPicker.XForms;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    /// <summary>
    /// Used to control the SfPicker
    /// BENDO: The view references this (or does it.  All references are now 0.), but I still need to implement the setting of these properties
    /// </summary>
    public class ExistingExerciseList : SfPicker
    {
        public ObservableCollection<Exercise> ExercisesList              { get; set; }
        public bool                           IsShowHeader               { get; set; }
        public bool                           IsShowColumnHeader         { get; set; }
        public bool                           IsEnableAutoReverse        { get; set; }
        public bool                           IsEnableBorderColor        { get; set; }
        public ObservableCollection<string>   ColumnHeaderTextCollection { get; set; }
        public string                         Name                       { get; set; }
    }
}
