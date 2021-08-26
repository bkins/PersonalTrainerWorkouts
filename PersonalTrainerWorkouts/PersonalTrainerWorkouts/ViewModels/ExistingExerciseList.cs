using System.Collections.Generic;
using System.Collections.ObjectModel;
using PersonalTrainerWorkouts.Models;
using Syncfusion.SfPicker.XForms;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExistingExerciseList : SfPicker
    {
        //public int             SelectedIndex              { get; set; }
        public ObservableCollection<Exercise> ExercisesList       { get; set; }
        public bool                           IsShowHeader        { get; set; }
        public bool                           IsShowColumnHeader  { get; set; }
        public bool                           IsEnableAutoReverse { get; set; }
        public bool                           IsEnableBorderColor { get; set; }
        //public string          HeaderText                { get; set; }
        public ObservableCollection<string>   ColumnHeaderTextCollection { get; set; }
        public string                         Name                       { get; set; }

    }
}