using System;
using System.Collections.Generic;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExerciseItemViewModel : ViewModelBase
    {   
        public IEnumerable<Exercise> AllExercises { get; set; }

        private Exercise _selectedExercise;
        public  Exercise SelectedExercise
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
        
        public ExerciseItemViewModel()
        {
            AllExercises = DataAccessLayer.GetExercises();
        }

        public void SaveExercise(int workoutId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_selectedExercise.Name))
                    throw new UnnamedEntityException($"{nameof(Exercise)} was not named.  Must be named before attempting to save.");
                
                var workoutExercise = new LinkedWorkoutsToExercises
                                      {
                                          ExerciseId   = _selectedExercise.Id
                                        , WorkoutId    = workoutId
                                        , LengthOfTime = _selectedExercise.LengthOfTime
                                      };
                
                var workoutExerciseId = DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise);

                if (workoutExerciseId == 0)
                {
                    throw new Exception("LinkedWorkoutsToExercises not added");
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message, Category.Error, e);

                throw;
            }
        }
    }
    
}
