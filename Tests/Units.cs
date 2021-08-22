using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.HelperModels;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.ViewModels;
using PersonalTrainerWorkouts.Views;
using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    //These test are more of integration test.
    //BENDO: Consider moving these to an Integrations Test class. Or renaming and recreating the Unit Test class

    public class Units : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        private static readonly string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                                                           , "WorkoutDatabase.db3");

        //private static AsyncDatabase _asyncDatabase;
        private static Database      _database;
        private        DataAccess    _dataAccess;

        //public static AsyncDatabase AsyncDatabase   => _asyncDatabase ??= new AsyncDatabase(DbPath);
        //BENDO:  Make AsyncDatabase inherit from IDataStore, so same tests can test the async version of the database
        //BENDO:  Create a MockDatabase that inherits from IDStore, so the same tests can be tested without having to go out to the database
        
        public static IDataStore    Database        => _database ??= new Database(DbPath);
        public        DataAccess    DataAccessLayer => _dataAccess ??= new DataAccess(Database);

        public Units(ITestOutputHelper testOutputHelper)
        {
            Database.CreateTables();
            _testOutputHelper = testOutputHelper;
        }

#region DataAccessLayer

    #region Simple Adds
    
        [Fact]
        public void AddNewWorkout()
        {
                    
            var workout = new Workout
                          {
                              Name = "TestWorkout"
                          };
        
            var workoutId = DataAccessLayer.AddNewWorkout(workout);
        
            Assert.True(workoutId > 0);
        
            var dbWorkout = DataAccessLayer.GetWorkout(workoutId);
        
            Assert.True(dbWorkout.Id > 0);
        }
        
        [Fact]
        public void AddNewExercise()
        {
            var exercise = new Exercise
                           {
                               Name = "Test Exercise"
                           };
        
            var exerciseId = DataAccessLayer.AddNewExercise(exercise);
        
            Assert.True(exerciseId > 0);
        }

        [Fact]
        public void AddLinkedWorkoutsToExercises()
        {
            RefreshDatabase();

            var exercise1 = new Exercise
                            {
                                Name = "Exercise 1 With LinkedWorkoutsToExercises"
                            };
            var exercise2 = new Exercise
                            {
                                Name = "Exercise 2 With LinkedWorkoutsToExercises"
                            };
            var exercise3 = new Exercise
                            {
                                Name = "Exercise 3 With LinkedWorkoutsToExercises"
                            };

            var workout = new Workout
                          {
                              Name = "Workout with LinkedWorkoutsToExercises"
                          };

            var exercise1Id = DataAccessLayer.AddNewExercise(exercise1);
            var exercise2Id = DataAccessLayer.AddNewExercise(exercise2);
            var exercise3Id = DataAccessLayer.AddNewExercise(exercise3);

            var workoutId   = DataAccessLayer.AddNewWorkout(workout);

            var linkedWorkoutsToExercises1 = new LinkedWorkoutsToExercises
                                             {
                                                 ExerciseId = exercise1Id
                                               , WorkoutId  = workoutId
                                             };
            var linkedWorkoutsToExercises2 = new LinkedWorkoutsToExercises
                                             {
                                                 ExerciseId = exercise2Id
                                               , WorkoutId  = workoutId
                                             };
            var linkedWorkoutsToExercises3 = new LinkedWorkoutsToExercises
                                             {
                                                 ExerciseId = exercise3Id
                                               , WorkoutId  = workoutId
                                             };

            var linkedWorkoutsToExercises1Id = DataAccessLayer.AddLinkedWorkoutsToExercises(linkedWorkoutsToExercises1);
            var linkedWorkoutsToExercises2Id = DataAccessLayer.AddLinkedWorkoutsToExercises(linkedWorkoutsToExercises2);
            var linkedWorkoutsToExercises3Id = DataAccessLayer.AddLinkedWorkoutsToExercises(linkedWorkoutsToExercises3);

            Assert.True(linkedWorkoutsToExercises1Id > 0);
            Assert.True(linkedWorkoutsToExercises2Id > 0);
            Assert.True(linkedWorkoutsToExercises3Id > 0);

            Assert.True(linkedWorkoutsToExercises1.OrderBy == 0);
            Assert.True(linkedWorkoutsToExercises2.OrderBy == 1);
            Assert.True(linkedWorkoutsToExercises3.OrderBy == 2);
        }

        [Fact]
        public void AddExerciseWithWorkoutExerciseOrderByIsCorrect()
        {
            RefreshDatabase();

            var exercise1 = new Exercise
                            {
                                Name = "Exercise 1 With WorkoutExercise"
                            };
            var exercise2 = new Exercise
                            {
                                Name = "Exercise 2 With WorkoutExercise"
                            };
            var exercise3 = new Exercise
                            {
                                Name = "Exercise 3 With WorkoutExercise"
                            };

            var workout = new Workout
                          {
                              Name = "Workout with WorkoutExercise"
                          };

            var exercise1Id = DataAccessLayer.AddNewExercise(exercise1);
            var exercise2Id = DataAccessLayer.AddNewExercise(exercise2);
            var exercise3Id = DataAccessLayer.AddNewExercise(exercise3);

            var workoutId   = DataAccessLayer.AddNewWorkout(workout);

            var workoutExercise1 = new WorkoutExercise
                                  {
                                      ExerciseId = exercise1Id
                                    , WorkoutId  = workoutId
                                  };
            var workoutExercise2 = new WorkoutExercise
                                   {
                                       ExerciseId = exercise2Id
                                     , WorkoutId  = workoutId
                                   };
            var workoutExercise3 = new WorkoutExercise
                                   {
                                       ExerciseId = exercise3Id
                                     , WorkoutId  = workoutId
                                   };

            var workoutExercise1Id = DataAccessLayer.AddWorkoutExercise(workoutExercise1);
            var workoutExercise2Id = DataAccessLayer.AddWorkoutExercise(workoutExercise2);
            var workoutExercise3Id = DataAccessLayer.AddWorkoutExercise(workoutExercise3);

            Assert.True(workoutExercise1Id > 0);
            Assert.True(workoutExercise2Id > 0);
            Assert.True(workoutExercise3Id > 0);

            Assert.True(workoutExercise1.OrderBy == 0);
            Assert.True(workoutExercise2.OrderBy == 1);
            Assert.True(workoutExercise3.OrderBy == 2);
        }

        [Fact]
        public void AddNewTypeOfExercise()
        {
            RefreshDatabase();

            var typeOfExercise = new TypeOfExercise
                                 {
                                     Name = "Test Type Of Exercise"
                                 };

            var typeOfExerciseId = DataAccessLayer.AddNewTypeOfExercise(typeOfExercise);

            Assert.True(typeOfExerciseId > 0);
        }

        [Fact]
        public void AddNewEquipment()
        {
            var equipment = new Equipment
                            {
                                Name = "Test Equipment"
                            };

            var equipmentId = DataAccessLayer.AddNewEquipment(equipment);

            Assert.True(equipmentId > 0);
        }

        [Fact]
        public void AddNewMuscleGroup()
        {
            var muscleGroup = new MuscleGroup
                              {
                                  Name = "Test Muscle Group"
                              };

            var muscleGroupId = DataAccessLayer.AddNewMuscleGroup(muscleGroup);

            Assert.True(muscleGroupId > 0);
        }

        [Fact]
        public void AddNewMuscleGroupThatOpposesAnother()
        {
            var muscleGroup = new MuscleGroup
                              {
                                  Name = "Bicep"
                              };

            var opposingMuscleGroup = new MuscleGroup
                                      {
                                          Name = "Tricep"
                                      };

            var muscleGroupId         = DataAccessLayer.AddNewMuscleGroup(muscleGroup);
            var opposingMuscleGroupId = DataAccessLayer.AddNewMuscleGroup(opposingMuscleGroup);

            int opposingMuscleGroupRelationshipId = DataAccessLayer.AddNewOpposingMuscleGroupRelationship(muscleGroupId
                                                                                                        , opposingMuscleGroupId);
            Assert.True(opposingMuscleGroupRelationshipId > 0);
        }

        [Fact]
        public void AddNewMuscleGroupThatOpposesAnotherSimplified()
        {
            var muscleGroup = new MuscleGroup
                              {
                                  Name = "Bicep"
                              };

            var opposingMuscleGroup = new MuscleGroup
                                      {
                                          Name = "Tricep"
                                      };

            int opposingMuscleGroupRelationshipId = DataAccessLayer.AddNewOpposingMuscleGroupRelationship(muscleGroup
                                                                                                        , opposingMuscleGroup);
            Assert.True(opposingMuscleGroupRelationshipId > 0);
        }

    #endregion

    #region Prevent Duplicates

        [Fact]
        public void PreventAddDuplicateWorkout()
        {
            TestForDuplicatedEntity(AddDuplicateWorkout);
        }

        [Fact]
        public void PreventAddDuplicateExercise()
        {
            TestForDuplicatedEntity(AddDuplicateExercise);
        }

        [Fact]
        public void PreventAddDuplicateTypeOfExercise()
        {
            TestForDuplicatedEntity(AddDuplicatedTypeOfExercise);
        }

        [Fact]
        public void PreventAddDuplicateEquipment()
        {
            TestForDuplicatedEntity(AddDuplicatedEquipment);
        }

        [Fact]
        public void PreventAddDuplicateMuscleGroups()
        {
            TestForDuplicatedEntity(AddDuplicatedMuscleGroup);
        }

        #endregion

    #region ViewModels

        [Fact]
        public void ReorderExerciseList()
        {
            RefreshDatabase();

            var viewModel = LoadExerciseListViewModel(out var exerciseOne
                                                , out var exerciseTwo
                                                , out var exerciseThree
                                                , out var exerciseFour);

            Assert.True(exerciseOne.WorkoutExercise.OrderBy   < exerciseTwo.WorkoutExercise.OrderBy);
            Assert.True(exerciseTwo.WorkoutExercise.OrderBy   < exerciseThree.WorkoutExercise.OrderBy);
            Assert.True(exerciseThree.WorkoutExercise.OrderBy < exerciseFour.WorkoutExercise.OrderBy);
                
            _testOutputHelper.WriteLine("BEFORE");
                
            foreach (var workoutExercise in viewModel.LinkWorkoutExercises)
            {
                _testOutputHelper.WriteLine(workoutExercise.ExerciseForDebugging);
            }

            var e = new
                    {
                        OldIndex = 0
                      , NewIndex = 1
                    };
            
            //MoveTo simply removes the item at OldIndex and inserts it at NewIndex
            viewModel.LinkWorkoutExercises.MoveTo(e.OldIndex, e.NewIndex);
            
            for (var i = 0; i < viewModel.LinkWorkoutExercises.Count; i++)
            {
                viewModel.LinkWorkoutExercises[i].WorkoutExercise.OrderBy = i;
                viewModel.LinkWorkoutExercises[i].Save();
            }
            
            viewModel.RefreshData();

            _testOutputHelper.WriteLine("AFTER");

            foreach (var workoutExercise in viewModel.LinkWorkoutExercises)
            {
                _testOutputHelper.WriteLine(workoutExercise.ExerciseForDebugging);
            }
        }

        [Fact]
        public void CalculateTotalReps()
        {
            RefreshDatabase();

            var viewModel = LoadWorkoutExerciseViewModel();
            var totalReps = viewModel.TotalReps;

            Assert.Equal(viewModel.ExercisesWithLengthOfTime.Count, totalReps);
        }

    #endregion

    #endregion

#region ApplicationExceptions

        [Fact]
        public void WiringException()
        {
            Action testAction      = AddToNullList;
            var    wiringException = (WiringException) Record.Exception(testAction);     

            Assert.NotNull(wiringException);
            Assert.IsType<WiringException>(wiringException);
            
            _testOutputHelper.WriteLine($"Message: {wiringException.Message}");
            _testOutputHelper.WriteLine($"CallingClass: {wiringException.CallingClass}");
            _testOutputHelper.WriteLine($"CalledClass: {wiringException.CalledClass}");
            _testOutputHelper.WriteLine($"InnerExceptionMessage: {wiringException.InnerException?.Message}");
        }

        [Fact]
        public void SequenceContainsNoElementsException()
        {
            
            Action testAction        = SearchListThatResultInNotElementsInSequence;
            var    sequenceException = (SequenceContainsNoElementsException) Record.Exception(testAction);
                
            Assert.NotNull(sequenceException);
            Assert.IsType<SequenceContainsNoElementsException>(sequenceException);

            _testOutputHelper.WriteLine($"Message: {sequenceException.Message}");
            _testOutputHelper.WriteLine($"EntityName: {sequenceException.EntityName}");
            _testOutputHelper.WriteLine($"InnerExceptionMessage: {sequenceException.InnerException?.Message}");
        }

#endregion
        
        
#region Helper Methods

        private static void RefreshDatabase()
        {
            Database.DropTables();
            Database.CreateTables();
        }

        private void AddDuplicateWorkout()
        {
            const string sharedExerciseName = "Test Exercise";

            var workout = new Workout()
                          {
                              Name = sharedExerciseName
                          };

            var nextWorkout = new Workout()
                              {
                                  Name = sharedExerciseName
                              };

            var workoutId     = DataAccessLayer.AddNewWorkout(workout);
            var nextWorkoutId = DataAccessLayer.AddNewWorkout(nextWorkout);
        }

        private void AddDuplicateExercise()
        {
            const string sharedExerciseName = "Test Exercise";

            var exercise = new Exercise
                           {
                               Name = sharedExerciseName
                           };

            var nextExercise = new Exercise
                               {
                                   Name = sharedExerciseName
                               };

            var exerciseId     = DataAccessLayer.AddNewExercise(exercise);
            var nextExerciseId = DataAccessLayer.AddNewExercise(nextExercise);
        }

        private void AddDuplicatedTypeOfExercise()
        {
            const string sharedName = "Test Type Of Exercise";

            TypeOfExercise typeOfExercise = new TypeOfExercise
                                            {
                                                Name = sharedName
                                            };

            TypeOfExercise nextTypeOfExercise = new TypeOfExercise
                                                {
                                                    Name = sharedName
                                                };

            var typeOfExerciseId     = DataAccessLayer.AddNewTypeOfExercise(typeOfExercise);
            var nextTypeOfExerciseId = DataAccessLayer.AddNewTypeOfExercise(nextTypeOfExercise);
        }
        
        private void AddDuplicatedEquipment()
        {
            const string sharedName = "Test Equipment";

            Equipment equipment = new Equipment
                                       {
                                           Name = sharedName
                                       };

            Equipment nextEquipment = new Equipment
                                      {
                                          Name = sharedName
                                      };

            var equipmentId     = DataAccessLayer.AddNewEquipment(equipment);
            var nextEquipmentId = DataAccessLayer.AddNewEquipment(nextEquipment);
        }

        private void AddDuplicatedMuscleGroup()
        {
            const string sharedName = "Test Equipment";

            MuscleGroup muscleGroup = new MuscleGroup
                                    {
                                        Name = sharedName
                                    };

            MuscleGroup nextMuscleGroup = new MuscleGroup
                                          {
                                              Name = sharedName
                                          };

            var muscleGroupId     = DataAccessLayer.AddNewMuscleGroup(muscleGroup);
            var nextMuscleGroupId = DataAccessLayer.AddNewMuscleGroup(nextMuscleGroup);
        }

        private void TestForDuplicatedEntity(Action action)
        {
            var attemptToAddDuplicateEntityException = (AttemptToAddDuplicateEntityException) Record.Exception(action);
            
            Assert.NotNull(attemptToAddDuplicateEntityException);
            Assert.IsType<AttemptToAddDuplicateEntityException>(attemptToAddDuplicateEntityException);
            
            _testOutputHelper.WriteLine($"Message: {attemptToAddDuplicateEntityException.Message}");
            _testOutputHelper.WriteLine($"TypeDuplicated: {attemptToAddDuplicateEntityException.TypeDuplicated}");
            _testOutputHelper.WriteLine($"NameOfTypeDuplicated: {attemptToAddDuplicateEntityException.NameOfTypeDuplicated}");
            _testOutputHelper.WriteLine($"ObjectDuplicated: {attemptToAddDuplicateEntityException.ObjectDuplicated}");
            _testOutputHelper.WriteLine($"FieldDuplicated: {attemptToAddDuplicateEntityException.FieldDuplicated}");
            
            //return attemptToAddDuplicateEntityException;
        }
        
        private static void AddToNullList()
        {
            List<string> nullListOfStrings = null;

            try
            {
                // ReSharper disable once PossibleNullReferenceException
                nullListOfStrings.Add("Item");
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);

                throw new WiringException("Success: the list of string is null."
                                        , nameof(Units)
                                        , nameof(List<string>)
                                        , e);
            }
        }

        private static void SearchListThatResultInNotElementsInSequence()
        {
            Database.GetWorkout(-1);
        }

        private WorkoutExerciseViewModel LoadWorkoutExerciseViewModel()
        {
            var exerciseListViewModel = LoadExerciseListViewModel(out _, out _, out _, out _);

            var workoutExerciseViewModel = new WorkoutExerciseViewModel(exerciseListViewModel.WorkoutId.ToString());

            foreach (var exerciseLengthOfTime in workoutExerciseViewModel.ExercisesWithLengthOfTime)
            {
                exerciseLengthOfTime.Reps = 1;
            }

            return workoutExerciseViewModel;
        }

        private ExerciseListViewModel LoadExerciseListViewModel(out WorkoutExerciseWithChildren exerciseOne
                                                              , out WorkoutExerciseWithChildren exerciseTwo
                                                              , out WorkoutExerciseWithChildren exerciseThree
                                                              , out WorkoutExerciseWithChildren exerciseFour)
        {
            var workout = new Workout
                          {
                              Name = "Test Reordering Exercises"
                          };

            var workoutId = DataAccessLayer.AddNewWorkout(workout);

            var exercise1 = new Exercise
                            {
                                Name = "Exercise 1"
                            };

            var exercise2 = new Exercise
                            {
                                Name = "Exercise 2"
                            };

            var exercise3 = new Exercise
                            {
                                Name = "Exercise 3"
                            };

            var exercise4 = new Exercise
                            {
                                Name = "Exercise 4"
                            };

            var exercise1Id = DataAccessLayer.AddNewExercise(exercise1);
            var exercise2Id = DataAccessLayer.AddNewExercise(exercise2);
            var exercise3Id = DataAccessLayer.AddNewExercise(exercise3);
            var exercise4Id = DataAccessLayer.AddNewExercise(exercise4);

            var workoutExercise1 = new LinkedWorkoutsToExercises
                                   {
                                       ExerciseId = exercise1Id
                                     , WorkoutId  = workoutId
                                   };

            var workoutExercise2 = new LinkedWorkoutsToExercises
                                   {
                                       ExerciseId = exercise2Id
                                     , WorkoutId  = workoutId
                                   };

            var workoutExercise3 = new LinkedWorkoutsToExercises
                                   {
                                       ExerciseId = exercise3Id
                                     , WorkoutId  = workoutId
                                   };

            var workoutExercise4 = new LinkedWorkoutsToExercises
                                   {
                                       ExerciseId = exercise4Id
                                     , WorkoutId  = workoutId
                                   };

            DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise1);
            DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise2);
            DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise3);
            DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise4);

            var viewModel = new ExerciseListViewModel(workoutId);

            exerciseOne   = viewModel.LinkWorkoutExercises.First(field => field.Exercise.Name == exercise1.Name);
            exerciseTwo   = viewModel.LinkWorkoutExercises.First(field => field.Exercise.Name == exercise2.Name);
            exerciseThree = viewModel.LinkWorkoutExercises.First(field => field.Exercise.Name == exercise3.Name);
            exerciseFour  = viewModel.LinkWorkoutExercises.First(field => field.Exercise.Name == exercise4.Name);

            return viewModel;
        }

#endregion


#region Cleanup

        public void Dispose()
        {
            Database.DropTables();
        }
    
#endregion

    }
}
