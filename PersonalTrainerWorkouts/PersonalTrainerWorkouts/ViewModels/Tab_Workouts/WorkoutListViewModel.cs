using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class WorkoutListViewModel : ViewModelBase
    {
        public ObservableCollection<Workout>     ObservableListOfWorkouts { get; set; }
        public List<WorkoutsToExerciseViewModel> ListOfWorkoutsWithTotals { get; set; }
        
        public WorkoutListViewModel()
        {
            LoadData(DataAccessLayer.GetWorkouts());

            //ObservableListOfWorkouts = new ObservableCollection<Workout>(DataAccessLayer.GetWorkouts());
            //ListOfWorkoutsWithTotals = new List<WorkoutsToExerciseViewModel>();

            //foreach (var workout in ObservableListOfWorkouts)
            //{
            //    ListOfWorkoutsWithTotals.Add(new WorkoutsToExerciseViewModel(workout.Id.ToString()));
            //}
        }

        public WorkoutListViewModel(List<Workout> aListOfWorkouts, DataAccess dbAccessLayer)
        {
            DataAccessLayer          = dbAccessLayer;
            LoadData(aListOfWorkouts);

            //ObservableListOfWorkouts = new ObservableCollection<Workout>(aListOfWorkouts);
            //ListOfWorkoutsWithTotals = new List<WorkoutsToExerciseViewModel>();

            //foreach (var workout in ObservableListOfWorkouts)
            //{
            //    ListOfWorkoutsWithTotals.Add(new WorkoutsToExerciseViewModel(workout.Id.ToString(), DataAccessLayer));
            //}
        }

        public void LoadData(IEnumerable<Workout> workoutData)
        {
            
            ObservableListOfWorkouts = new ObservableCollection<Workout>(workoutData);
            ListOfWorkoutsWithTotals = new List<WorkoutsToExerciseViewModel>();

            foreach (var workout in ObservableListOfWorkouts)
            {
                ListOfWorkoutsWithTotals.Add(new WorkoutsToExerciseViewModel(workout.Id.ToString(), DataAccessLayer));
            }
        }
        public string Delete(int index)
        {
            if (index > ObservableListOfWorkouts.Count - 1)
            {
                return string.Empty;
            }

            //Get the workout to be deleted
            var workoutToDelete = ObservableListOfWorkouts[index];
            var workoutName     = workoutToDelete.Name;

            //Remove the workout from the source list
            ObservableListOfWorkouts.RemoveAt(index);

            //Delete the Workout from the database
            App.Database.DeleteWorkout(ref workoutToDelete);

            ObservableListOfWorkouts = new ObservableCollection<Workout>(DataAccessLayer.GetWorkouts());

            return workoutName;
        }

        public ObservableCollection<Workout> SearchByDifficulty(string searchText)
        {
            searchText = searchText.Replace(" "
                                          , "");

            if (searchText.Length < 3)
            {
                return new ObservableCollection<Workout>();
            }
            
            var firstChar  = searchText.ToUpper()[0];
            var secondChar = searchText.ToUpper()[1];
            var thirdChar  = searchText.ToUpper()[2];

            if ( ! ValidateSearchByDifficultyText(firstChar
                                                , secondChar
                                                , thirdChar))
            {
                return new ObservableCollection<Workout>();
            }

            if (secondChar == '='
            && int.TryParse(thirdChar.ToString(), out var theNumber))
            {
                return new ObservableCollection<Workout>(ObservableListOfWorkouts.Where(field => field.Difficulty.Equals(theNumber)));
            }

            if (secondChar == '<'
            &&  thirdChar  == '='
            &&  int.TryParse(searchText.Remove(0, 3)
                           , out theNumber))
            {
                return new ObservableCollection<Workout>(ObservableListOfWorkouts.Where(field => field.Difficulty <= theNumber));
            }
                
            if (secondChar == '>'
             && thirdChar  == '='
             && int.TryParse(searchText.Remove(0, 3)
                           , out theNumber))
            {
                return new ObservableCollection<Workout>(ObservableListOfWorkouts.Where(field => field.Difficulty >= theNumber));
            }
                
            if (secondChar == '<'
            && int.TryParse(searchText.Remove(0, 2)
                          , out theNumber))
            {
                return new ObservableCollection<Workout>(ObservableListOfWorkouts.Where(field => field.Difficulty < theNumber));
            }
                
            if (secondChar == '>'
            && int.TryParse(searchText.Remove(0, 2)
                          , out theNumber))
            {
                return new ObservableCollection<Workout>(ObservableListOfWorkouts.Where(field => field.Difficulty > theNumber));
            }
            return new ObservableCollection<Workout>();
        }

        public ObservableCollection<Workout> SearchByTotalReps(string searchText)
        {
            searchText = searchText.Replace(" "
                                          , "");
            
            if (searchText.Length < 4)
            {
                return new ObservableCollection<Workout>();
            }
            
            var thirdChar = searchText.ToUpper()[2];
            var forthChar = searchText.ToUpper()[3];
            
            if ( ! ValidateSearchByTotalReps(new string(searchText.Take(2).ToArray())
                                           , thirdChar
                                           , forthChar
                                           , searchText[searchText.Length-1]))
            {
                return new ObservableCollection<Workout>();
            }

            if (thirdChar == '=')
            {
                var theNumber = int.Parse(searchText.Remove(0
                                                          , 3));

                var foundWorkouts  = ListOfWorkoutsWithTotals.Where(field => field.TotalReps == theNumber);
                var returnWorkouts = foundWorkouts.Select(foundWorkout => foundWorkout.Workout)
                                                  .ToList();

                return new ObservableCollection<Workout>(returnWorkouts);
            }

            if (thirdChar == '<'
             && forthChar == '=')
            {
                var theNumber = int.Parse(searchText.Remove(0
                                                          , 4));

                var foundWorkouts = ListOfWorkoutsWithTotals.Where(field => field.TotalReps <= theNumber);
                var returnWorkouts = foundWorkouts.Select(foundWorkout => foundWorkout.Workout)
                                                  .ToList();

                return new ObservableCollection<Workout>(returnWorkouts);
            }
                
            if (thirdChar == '>'
             && forthChar == '=')
            {
                var theNumber = int.Parse(searchText.Remove(0
                                                          , 4));

                var foundWorkouts = ListOfWorkoutsWithTotals.Where(field => field.TotalReps >= theNumber);
                var returnWorkouts = foundWorkouts.Select(foundWorkout => foundWorkout.Workout)
                                                  .ToList();

                return new ObservableCollection<Workout>(returnWorkouts);
            }
                
            if (thirdChar == '<')
            {
                var theNumber = int.Parse(searchText.Remove(0
                                                          , 3));

                var foundWorkouts = ListOfWorkoutsWithTotals.Where(field => field.TotalReps < theNumber);
                var returnWorkouts = foundWorkouts.Select(foundWorkout => foundWorkout.Workout)
                                                  .ToList();

                return new ObservableCollection<Workout>(returnWorkouts);
            }
                
            if (thirdChar == '>')
            {
                var theNumber = int.Parse(searchText.Remove(0
                                                          , 3));

                var foundWorkouts = ListOfWorkoutsWithTotals.Where(field => field.TotalReps > theNumber);
                var returnWorkouts = foundWorkouts.Select(foundWorkout => foundWorkout.Workout)
                                                  .ToList();

                return new ObservableCollection<Workout>(returnWorkouts);
            }
            return new ObservableCollection<Workout>();
        }

        private bool ValidateSearchByDifficultyText(char firstChar, char secondChar, char thirdChar)
        {
            return firstChar.Equals('D')
                && 
                   (
                       (secondChar == '=' 
                     || secondChar == '<' 
                     || secondChar == '>')
                    && (int.TryParse(thirdChar.ToString(), out _) 
                     || thirdChar  == '=')
                   );
        }
        
        private bool ValidateSearchByTotalReps(string firstTwoChars, char thirdChar, char forthChar, char lastChar)
        {
            return firstTwoChars.Equals("TR", StringComparison.CurrentCultureIgnoreCase)
                && 
                   (
                       (thirdChar == '='
                     || thirdChar == '<' 
                     || thirdChar == '>')
                    && (int.TryParse(forthChar.ToString(), out _) 
                     || forthChar  == '=')
                   )
                && int.TryParse(lastChar.ToString(), out _);
        }

        public ObservableCollection<Workout> SearchByNameAndDescription(string workoutFilterText)
        {
            return new ObservableCollection<Workout>
                   (
                        ObservableListOfWorkouts.Where(field=>field.Name
                                                                   .ToUpper()
                                                                   .Contains(workoutFilterText.ToUpper())
                                                            || field.Description
                                                                    .ToUpper()
                                                                    .Contains(workoutFilterText.ToUpper()))
                   );
        }

        public ObservableCollection<Workout> SearchWorkouts(string workoutFilterText)
        {
            var foundWorkouts = SearchByDifficulty(workoutFilterText);
            if (foundWorkouts.Any())
            {
                return foundWorkouts;
            }

            foundWorkouts = SearchByTotalReps(workoutFilterText);

            if (foundWorkouts.Any())
            {
                return foundWorkouts;
            }

            return SearchByNameAndDescription(workoutFilterText);
        }
    }
    
}
