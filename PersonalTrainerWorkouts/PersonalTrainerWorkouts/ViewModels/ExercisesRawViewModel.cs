using System.Collections.Generic;
using System.Collections.ObjectModel;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExercisesRawViewModel : ViewModelBase
    {
        private List<Exercise>                 ExercisesData   { get; }
        public ObservableCollection<Exercise> ListOfExercises { get; set; }
        
        public ExercisesRawViewModel()
        {
            var arrayOfExercises = DataAccessLayer.GetExercises();

            ExercisesData   = new List<Exercise>();

            foreach (var exercise in arrayOfExercises)
            {
                ExercisesData.Add(exercise);
            }
            ListOfExercises = new ObservableCollection<Exercise>(ExercisesData);
        }
    }
}
