using System;
using System.Collections.Generic;
using System.Text;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExerciseViewModel : ViewModelBase
    {
        public List<Exercises> AllExercises => App.Database.GetAllExercisesAsync().Result;

        private Exercises _selectedExercise;
        
        public Exercises SelectedExercise
        {
            get => _selectedExercise;
            set
            {
                if (_selectedExercise != value)
                {
                    _selectedExercise = value;
                    OnPropertyChanged();
                }
            }
        }

    }
    
}
