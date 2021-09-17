using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class WorkoutListViewModel : ViewModelBase
    {
        public List<Workout> ListOfWorkouts { get; set; }

        public ObservableCollection<Workout> ObservableListOfWorkouts { get; set; }

        public WorkoutListViewModel()
        {
            ObservableListOfWorkouts = new ObservableCollection<Workout>(DataAccessLayer.GetWorkouts());
        }

        public WorkoutListViewModel(List<Workout> aListOfWorkouts) //: this()
        {
            ObservableCollection<Workout> testCollection = new ObservableCollection<Workout>(aListOfWorkouts);
            ObservableListOfWorkouts = new ObservableCollection<Workout>(aListOfWorkouts);
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

            //Examples:
            // * d=1    (3 chars)
            // * d<2    (3 chars)
            // * d<=3   (4 chars)
            // * d>=10  (5 chars)

            var firstChar  = searchText.ToUpper()[0];
            var secondChar = searchText.ToUpper()[1];
            var thirdChar  = searchText.ToUpper()[2];

            //1.  Is first char 'D'
            //2.  Is second char either '=', '>', or '<'
            //3.  Is third char either '>', or '<'
            //    Or is it a number
            //4.  If the third is either '>', or '<', is the fourth a number, then take the rest of the string and int.Parse

            if ( ! ValidateSearchByDifficultyText(firstChar
                                               , secondChar
                                               , thirdChar))
                return new ObservableCollection<Workout>();

            if (secondChar == '=')
            {
                var theNumber = int.Parse(thirdChar.ToString());
                return new ObservableCollection<Workout>(ObservableListOfWorkouts.Where(field => field.Difficulty.Equals(theNumber)));
            }

            if (secondChar == '<'
             && thirdChar  == '=')
            {
                var theNumber = int.Parse(searchText.Remove(0
                                                          , 3));
                return new ObservableCollection<Workout>(ObservableListOfWorkouts.Where(field => field.Difficulty <= theNumber));
            }
                
            if (secondChar == '>'
             && thirdChar  == '=')
            {
                var theNumber = int.Parse(searchText.Remove(0
                                                          , 3));
                return new ObservableCollection<Workout>(ObservableListOfWorkouts.Where(field => field.Difficulty >= theNumber));
            }
                
            if (secondChar == '<')
            {
                var theNumber = int.Parse(searchText.Remove(0
                                                          , 2));
                return new ObservableCollection<Workout>(ObservableListOfWorkouts.Where(field => field.Difficulty < theNumber));
            }
                
            if (secondChar == '>')
            {
                var theNumber = int.Parse(searchText.Remove(0
                                                          , 2));
                return new ObservableCollection<Workout>(ObservableListOfWorkouts.Where(field => field.Difficulty > theNumber));
            }
            return new ObservableCollection<Workout>();
        }


        private bool ValidateSearchByDifficultyText(char firstChar, char secondChar, char thirdChar)
        {
            return firstChar.Equals('D')
                && 
                   (
                       (secondChar == '=' || secondChar == '<' || secondChar == '>')
                    && (int.TryParse(thirdChar.ToString(), out _) || thirdChar  == '=')
                   );
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

            return SearchByNameAndDescription(workoutFilterText);
        }
    }
}
