using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExerciseItemViewModel : ViewModelBase
    {
        
        private static DataAccess _dataAccess;
        
        private static DataAccess DataAccessLayer => _dataAccess = _dataAccess ?? new DataAccess(App.Database);

        //public List<Exercises> AllExercises => App.Database.GetAllExercisesAsync().Result;
        public  ObservableCollection<Exercise> AllExercises => App.Database.GetObservableExercises();
        private Exercise                       _selectedExercise;
        
        public Exercise SelectedExercise
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
        public void SaveExercise(int workoutId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_selectedExercise.Name))
                    throw new UnnamedEntityException($"{nameof(Exercise)} was not named.  Must be named before attempting to save.");
                
                var workoutExercise = new LinkedWorkoutsToExercises //WorkoutExercise
                                      {
                                          ExerciseId   = _selectedExercise.Id
                                        , WorkoutId    = workoutId
                                        , LengthOfTime = _selectedExercise.LengthOfTime
                                      };
                
                //var workoutExerciseId = DataAccessLayer.AddWorkoutExercise(workoutExercise);
                var workoutExerciseId = DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise);

                if (workoutExerciseId ==0)
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
