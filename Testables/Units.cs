using Avails.D_Flat.Exceptions;
using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Data.Interfaces;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.Models.Intermediates;
using Syncfusion.DataSource.Extensions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using Tests.Helpers;
using Xamarin.Essentials;

using Xunit;
using Xunit.Abstractions;

using SequenceContainsNoElementsException = Avails.D_Flat.Exceptions.SequenceContainsNoElementsException;

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
        private static Database          _database;
        private static ContactsDataStore _contactsDataStore;
        private        DataAccess        _dataAccess;

        //public static AsyncDatabase AsyncDatabase   => _asyncDatabase ??= new AsyncDatabase(DbPath);
        //BENDO:  Make AsyncDatabase inherit from IDataStore, so same tests can test the async version of the database
        //BENDO:  Create a MockDatabase that inherits from IDStore, so the same tests can be tested without having to go out to the database

        public static IDataStore         Database          => _database ??= new Database(DbPath);
        public static IContactsDataStore ContactsDataStore => _contactsDataStore ??= new ContactsDataStore();
        public        DataAccess         DataAccessLayer   => _dataAccess ??= new DataAccess(Database, ContactsDataStore);

        public Units(ITestOutputHelper testOutputHelper)
        {
            Database.CreateTables();
            _testOutputHelper = testOutputHelper;
        }

        #region DataAccessLayer

        #region Simple Adds

        [Fact]
        public void TestAddContact()
        {
            //Arrange
            var mrRogers = GetMrRogers();
            var appContact = new AppContact(mrRogers);

            //Act
            var mrRogersId = DataAccessLayer.AddNewContact(appContact);
            var foundMrRogers = DataAccessLayer.GetAppContacts()
                                               .FirstOrDefault(contact => contact.Id == mrRogersId);

            //Assert
            Assert.NotNull(foundMrRogers);
        }

        private static Contact GetMrRogers()
        {
            var phoneList = new List<ContactPhone>
                            {
                                new ("123-456-7890")
                            };

            var emailLIst = new List<ContactEmail>
                            {
                                new ContactEmail("fRogers@gmail.com")
                            };

            var mrRogers = new Contact("1"
                                     , "Mr"
                                     , "Fred"
                                     , "M"
                                     , "Rogers"
                                     , "III"
                                     , phoneList
                                     , emailLIst);

            return mrRogers;
        }

        [Fact]
        public void TestAddNewWorkout()
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
        public void TestAddNewExercise()
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

            var workoutId = DataAccessLayer.AddNewWorkout(workout);

            var linkedWorkoutsToExercises1 = new LinkedWorkoutsToExercises
            {
                ExerciseId = exercise1Id
                                               ,
                WorkoutId = workoutId
            };

            var linkedWorkoutsToExercises2 = new LinkedWorkoutsToExercises
            {
                ExerciseId = exercise2Id
                                               ,
                WorkoutId = workoutId
            };

            var linkedWorkoutsToExercises3 = new LinkedWorkoutsToExercises
            {
                ExerciseId = exercise3Id
                                               ,
                WorkoutId = workoutId
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

            var workoutId = DataAccessLayer.AddNewWorkout(workout);

            var workoutExercise1 = new WorkoutExercise
            {
                ExerciseId = exercise1Id
                                     ,
                WorkoutId = workoutId
            };

            var workoutExercise2 = new WorkoutExercise
            {
                ExerciseId = exercise2Id
                                     ,
                WorkoutId = workoutId
            };

            var workoutExercise3 = new WorkoutExercise
            {
                ExerciseId = exercise3Id
                                     ,
                WorkoutId = workoutId
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

        [Fact(Skip = "failing - not sure why")]
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

        [Fact(Skip = "Fix or rewrite: DataAccessLayer.AddNewOpposingMuscleGroupRelationship was removed")]
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

            var muscleGroupId = DataAccessLayer.AddNewMuscleGroup(muscleGroup);
            var opposingMuscleGroupId = DataAccessLayer.AddNewMuscleGroup(opposingMuscleGroup);

            //int opposingMuscleGroupRelationshipId = DataAccessLayer.AddNewOpposingMuscleGroupRelationship(muscleGroupId
            //                                                                                            , opposingMuscleGroupId);
            //Assert.True(opposingMuscleGroupRelationshipId > 0);
        }

        [Fact(Skip = "Fix broken reference: DataAccessLayer.AddNewOpposingMuscleGroupRelationship was removed")]
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

            //int opposingMuscleGroupRelationshipId = DataAccessLayer.AddNewOpposingMuscleGroupRelationship(muscleGroup
            //                                                                                            , opposingMuscleGroup);
            //Assert.True(opposingMuscleGroupRelationshipId > 0);
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

            Assert.True(exerciseOne.WorkoutExercise.OrderBy < exerciseTwo.WorkoutExercise.OrderBy);
            Assert.True(exerciseTwo.WorkoutExercise.OrderBy < exerciseThree.WorkoutExercise.OrderBy);
            Assert.True(exerciseThree.WorkoutExercise.OrderBy < exerciseFour.WorkoutExercise.OrderBy);

            _testOutputHelper.WriteLine("BEFORE");

            foreach (var workoutExercise in viewModel.LinkWorkoutExercises)
            {
                _testOutputHelper.WriteLine(workoutExercise.ExerciseForDebugging);
            }

            var e = new
            {
                OldIndex = 0
                      ,
                NewIndex = 1
            };

            //MoveTo simply removes the item at OldIndex and inserts it at NewIndex
            viewModel.LinkWorkoutExercises.MoveTo(e.OldIndex
                                                , e.NewIndex);

            for (var i = 0; i < viewModel.LinkWorkoutExercises.Count; i++)
            {
                viewModel.LinkWorkoutExercises[i]
                         .WorkoutExercise.OrderBy = i;

                viewModel.LinkWorkoutExercises[i]
                         .Save();
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

            //Why am I using the Count of the list?  Because I added 4 exercises with 1 rep each
            Assert.Equal(viewModel.ExercisesWithIntermediateFields.Count
                       , totalReps);

            var totalRepsFromDb = 0;

            var workoutsToExercises = new List<LinkedWorkoutsToExercises>(DataAccessLayer.GetLinkedWorkoutsToExercises(1));

            foreach (var workoutsToExercise in workoutsToExercises)
            {
                var exercises = DataAccessLayer.GetLinkedWorkoutsToExercises(workoutsToExercise.WorkoutId);
                var exercise = exercises.First(field => field.ExerciseId == workoutsToExercise.ExerciseId);

                totalRepsFromDb += exercise.Reps;
            }

            Assert.Equal(totalReps
                       , totalRepsFromDb);
        }

        #endregion

        #endregion

        #region ApplicationExceptions

        [Fact]
        public void WiringException()
        {
            Action testAction = AddToNullList;
            var wiringException = (WiringException)Record.Exception(testAction);

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
            Action testAction = SearchListThatResultInNotElementsInSequence;
            var sequenceException = (SequenceContainsNoElementsException)Record.Exception(testAction);

            Assert.NotNull(sequenceException);
            Assert.IsType<SequenceContainsNoElementsException>(sequenceException);

            _testOutputHelper.WriteLine($"Message: {sequenceException.Message}");
            _testOutputHelper.WriteLine($"EntityName: {sequenceException.EntityName}");
            _testOutputHelper.WriteLine($"InnerExceptionMessage: {sequenceException.InnerException?.Message}");
        }

        #endregion

        #region Goal Status
        
        //TODO: This happened today (5/28/2023). Add unit test for this scenario:
        //Appears that MissedTarget doesn't count today's date
        /*
         * DateStarted: 5/27/2023
           TargetDate: 5/28/2023
           DateCompleted: null
           GetStatus: Missed Target
           Failed: False
           InProcess: True              <-- Shouldn't have two true statuses
           MissedTarget: True           <-- Shouldn't have two true statuses
           NotStarted: False
           SuccessfullyCompleted: False
         */
        [Theory]
        [ClassData(typeof(EnumeratedGoalStatus))]
        // [InlineData("5/28/2023", "5/27/2023", "5/28/2023", null, false, Goal.Status.InProgress)]
        // [InlineData("5/28/2023", "5/27/2023", "6/27/2023", null, false, Goal.Status.InProgress)]
        // [InlineData("5/28/2023", "5/27/2023", "6/27/2023", "6/10/2023", false, Goal.Status.CompletedSuccessfully)]
        // [InlineData("5/28/2023", "5/27/2023", "6/27/2023", null, true, Goal.Status.Failed)]
        // [InlineData("5/28/2023", null, "6/27/2023", "6/10/2023", false, Goal.Status.NotStarted)]
        // [InlineData("5/28/2023", "5/27/2023", "6/27/2023", "6/28/2023", false, Goal.Status.MissTarget)]
        public void TestGoalStatuses(string      todaysDate
                                   , string      startDate
                                   , string      targetDate
                                   , string      completedDate
                                   , bool        failed
                                   , Goal.Status expectedStatus)
        //public void TestGoalStatuses( GoalStatusTestObject testObject)
        {
            //Arrange
            var goal = new Goal
                           {
                               Id            = 1
                             , ClientId      = 1
                             , Name          = "Test Goal"
                             , Description   = "Test Goal description"
                             , Failed        = failed
                             , DateStarted   = ParseDateTime(startDate)
                             , TargetDate    = DateTime.Parse(targetDate)
                             , DateCompleted = ParseDateTime(completedDate)
                             , TodaysDate    = DateTime.Parse(todaysDate) 
                           };
            
            //Act
            var actualStatus = goal.GetStatus();
            
            var statuses = new List<bool>
                               {
                                   goal.Failed
                                 , goal.InProcess
                                 , goal.MissedTarget
                                 , goal.NotStarted
                                 , goal.SuccessfullyCompleted
                               };
            
            //Assert
            //The expected status matches the actual status
            Assert.Equal(expectedStatus, actualStatus);
            
            //There is only one status set to true
            Assert.True(statuses.Count(status => status) == 1);
        }

        #endregion
        #region Search

        [Theory]
        [InlineData("", 0)]
        [InlineData("d=3", 1)]
        [InlineData("d>1", 3)]
        [InlineData("d>=2", 3)]
        [InlineData("d<3", 2)]
        [InlineData("d<=3", 3)]
        public void TestSearchDifficulty(string searchText
                                       , int expectedCount)
        {
            //Arrange
            RefreshDatabase();

            var viewModel = GetLoadedWorkoutListViewModel();

            //Act
            var listOfWorkouts = viewModel.SearchByDifficulty(searchText);

            //Assert
            Assert.Equal(expectedCount
                       , listOfWorkouts.Count);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("TR=0", 0)]
        [InlineData("TR=-1", 0)]
        [InlineData("TR=9999999", 0)]
        [InlineData("TR<9999999", 4)]
        [InlineData("TR=150", 1)]
        [InlineData("TR<71", 2)]
        [InlineData("TR<=70", 2)]
        [InlineData("TR>70", 2)]
        [InlineData("TR>=70", 3)]
        [InlineData("TR>", 0)]
        [InlineData("TR<", 0)]
        [InlineData("TR=", 0)]
        [InlineData("TR>=", 0)]
        [InlineData("TR<=", 0)]
        public void TestSearchTotalReps(string searchText
                                      , int expectedCount)
        {
            //Arrange
            RefreshDatabase();

            var viewModel = GetLoadedWorkoutListViewModel();

            //Act
            var listOfWorkouts = viewModel.SearchByTotalReps(searchText);

            //Assert
            Assert.Equal(expectedCount
                       , listOfWorkouts.Count);
        }

        [Theory]
        [InlineData("TT=1:00", 1)]
        public void TestSearchTotalTime(string searchText
                                      , int expectedCount)
        {
            //Arrange
            //RefreshDatabase();

            //var viewModel = GetLoadedWorkoutListViewModel();

            ////Act
            //ObservableCollection<Workout> listOfWorkouts = viewModel.SearchByTotalTime(searchText);

            ////Assert
            //Assert.Equal(expectedCount
            //           , listOfWorkouts.Count);
        }

        [Theory]
        [InlineData("", 4)]
        [InlineData("Econ", 1)]
        [InlineData("cess", 1)]
        [InlineData("etc.", 1)]
        [InlineData("or ", 2)]
        [InlineData(".", 4)]
        public void TestSearchNameAndDescription(string searchText
                                               , int expectedCount)
        {
            //Arrange
            RefreshDatabase();

            var viewModel = GetLoadedWorkoutListViewModel();

            //Act
            var listOfWorkouts = viewModel.SearchByNameAndDescription(searchText);

            //Assert
            Assert.Equal(expectedCount
                       , listOfWorkouts.Count);
        }

        [Theory(Skip = "failing - not sure why")]
        [InlineData("", 4)]
        [InlineData("Econ", 1)]
        [InlineData("cess", 1)]
        [InlineData("etc.", 1)]
        [InlineData("or ", 2)]
        [InlineData(".", 4)]
        [InlineData("d=3", 1)]
        [InlineData("d>1", 3)]
        [InlineData("d>=2", 3)]
        [InlineData("d<3", 2)]
        [InlineData("d<=3", 3)]
        [InlineData("TR=150", 1)]
        [InlineData("TR<71", 2)]
        [InlineData("TR<=70", 2)]
        [InlineData("TR>70", 2)]
        [InlineData("TR>=70", 3)]
        public void TestSearchWorkouts(string searchText
                                     , int expectedCount)
        {
            //Arrange
            RefreshDatabase();

            var viewModel = GetLoadedWorkoutListViewModel();

            //Act
            var listOfWorkouts = viewModel.SearchWorkouts(searchText);

            //Assert
            Assert.Equal(expectedCount
                       , listOfWorkouts.Count);
        }

        #endregion

        #region Helper Methods

        private static DateTime? ParseDateTime(string dateTime)
        {
            return dateTime is null
                        ? null
                        : DateTime.Parse(dateTime);
        }
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

            var workoutId = DataAccessLayer.AddNewWorkout(workout);
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

            var exerciseId = DataAccessLayer.AddNewExercise(exercise);
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

            var typeOfExerciseId = DataAccessLayer.AddNewTypeOfExercise(typeOfExercise);
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

            var equipmentId = DataAccessLayer.AddNewEquipment(equipment);
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

            var muscleGroupId = DataAccessLayer.AddNewMuscleGroup(muscleGroup);
            var nextMuscleGroupId = DataAccessLayer.AddNewMuscleGroup(nextMuscleGroup);
        }

        private void TestForDuplicatedEntity(Action action)
        {
            var attemptToAddDuplicateEntityException = (AttemptToAddDuplicateEntityException)Record.Exception(action);

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
                Logger.WriteToToast = false;
                Logger.WriteToLogCat = false;

                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                //Reset Logger
                Logger.WriteToToast = true;
                Logger.WriteToLogCat = true;

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

        private WorkoutsToExerciseViewModel LoadWorkoutExerciseViewModel()
        {
            var exerciseListViewModel = LoadExerciseListViewModel(out _
                                                                , out _
                                                                , out _
                                                                , out _);

            var workoutExerciseViewModel = new WorkoutsToExerciseViewModel(exerciseListViewModel.WorkoutId.ToString());

            foreach (var exerciseLengthOfTime in workoutExerciseViewModel.ExercisesWithIntermediateFields)
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
                              ,
                Reps = 1
            };

            var exercise2 = new Exercise
            {
                Name = "Exercise 2"
                              ,
                Reps = 1
            };

            var exercise3 = new Exercise
            {
                Name = "Exercise 3"
                              ,
                Reps = 1
            };

            var exercise4 = new Exercise
            {
                Name = "Exercise 4"
                              ,
                Reps = 1
            };

            var exercise1Id = DataAccessLayer.AddNewExercise(exercise1);
            var exercise2Id = DataAccessLayer.AddNewExercise(exercise2);
            var exercise3Id = DataAccessLayer.AddNewExercise(exercise3);
            var exercise4Id = DataAccessLayer.AddNewExercise(exercise4);

            var workoutExercise1 = new LinkedWorkoutsToExercises
            {
                ExerciseId = exercise1Id
                                     ,
                WorkoutId = workoutId
                                     ,
                Reps = exercise1.Reps
            };

            var workoutExercise2 = new LinkedWorkoutsToExercises
            {
                ExerciseId = exercise2Id
                                     ,
                WorkoutId = workoutId
                                     ,
                Reps = exercise2.Reps
            };

            var workoutExercise3 = new LinkedWorkoutsToExercises
            {
                ExerciseId = exercise3Id
                                     ,
                WorkoutId = workoutId
                                     ,
                Reps = exercise3.Reps
            };

            var workoutExercise4 = new LinkedWorkoutsToExercises
            {
                ExerciseId = exercise4Id
                                     ,
                WorkoutId = workoutId
                                     ,
                Reps = exercise4.Reps
            };

            DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise1);
            DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise2);
            DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise3);
            DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise4);

            var viewModel = new ExerciseListViewModel(workoutId);

            exerciseOne = viewModel.LinkWorkoutExercises.First(field => field.Exercise.Name == exercise1.Name);
            exerciseTwo = viewModel.LinkWorkoutExercises.First(field => field.Exercise.Name == exercise2.Name);
            exerciseThree = viewModel.LinkWorkoutExercises.First(field => field.Exercise.Name == exercise3.Name);
            exerciseFour = viewModel.LinkWorkoutExercises.First(field => field.Exercise.Name == exercise4.Name);

            return viewModel;
        }

        private WorkoutListViewModel GetLoadedWorkoutListViewModel()
        {
            var workout1 = AddNewWorkout("Economics"
                                       , "the branch of knowledge concerned with the production, consumption, and transfer of wealth."
                                       , 1);

            var workout2 = AddNewWorkout("Success"
                                       , "the accomplishment of an aim or purpose."
                                       , 2);

            var workout3 = AddNewWorkout("Warning"
                                       , "a statement or event that indicates a possible or impending danger, problem, or other unpleasant situation."
                                       , 3);

            var workout4 = AddNewWorkout("Organization"
                                       , "an organized body of people with a particular purpose, especially a business, society, association, etc."
                                       , 4);

            var exercise1 = AddNewExercise("Exercise 1"
                                         , "E1 description"
                                         , "1:00"
                                         , 10);

            var exercise2 = AddNewExercise("Exercise 2"
                                         , "E2 Description"
                                         , "2:00"
                                         , 20);

            var exercise3 = AddNewExercise("Exercise 3"
                                         , "E3 Description"
                                         , "3:00"
                                         , 30);

            var exercise4 = AddNewExercise("Exercise 4"
                                         , "E4 Description"
                                         , "4:00"
                                         , 40);

            var exercise5 = AddNewExercise("Exercise 5"
                                         , "E5 Description"
                                         , "5:00"
                                         , 50);

            var exercise6 = AddNewExercise("Exercise 6"
                                         , "E6 Description"
                                         , "6:00"
                                         , 60);

            var exercise7 = AddNewExercise("Exercise 7"
                                         , "E7 Description"
                                         , "7:00"
                                         , 70);

            var exercise8 = AddNewExercise("Exercise 8"
                                         , "E8 Description"
                                         , "8:00"
                                         , 80);

            DataAccessLayer.AddLinkedWorkoutsToExercises(new LinkedWorkoutsToExercises
            {
                WorkoutId = workout1.Id
                                                           ,
                ExerciseId = exercise1.Id
                                                           ,
                LengthOfTime = exercise1.LengthOfTime
                                                           ,
                Reps = exercise1.Reps
            });

            DataAccessLayer.AddLinkedWorkoutsToExercises(new LinkedWorkoutsToExercises
            {
                WorkoutId = workout1.Id
                                                           ,
                ExerciseId = exercise2.Id
                                                           ,
                LengthOfTime = exercise2.LengthOfTime
                                                           ,
                Reps = exercise2.Reps
            });

            DataAccessLayer.AddLinkedWorkoutsToExercises(new LinkedWorkoutsToExercises
            {
                WorkoutId = workout2.Id
                                                           ,
                ExerciseId = exercise3.Id
                                                           ,
                LengthOfTime = exercise3.LengthOfTime
                                                           ,
                Reps = exercise3.Reps
            });

            DataAccessLayer.AddLinkedWorkoutsToExercises(new LinkedWorkoutsToExercises
            {
                WorkoutId = workout2.Id
                                                           ,
                ExerciseId = exercise4.Id
                                                           ,
                LengthOfTime = exercise4.LengthOfTime
                                                           ,
                Reps = exercise4.Reps
            });

            DataAccessLayer.AddLinkedWorkoutsToExercises(new LinkedWorkoutsToExercises
            {
                WorkoutId = workout3.Id
                                                           ,
                ExerciseId = exercise5.Id
                                                           ,
                LengthOfTime = exercise5.LengthOfTime
                                                           ,
                Reps = exercise5.Reps
            });

            DataAccessLayer.AddLinkedWorkoutsToExercises(new LinkedWorkoutsToExercises
            {
                WorkoutId = workout3.Id
                                                           ,
                ExerciseId = exercise6.Id
                                                           ,
                LengthOfTime = exercise6.LengthOfTime
                                                           ,
                Reps = exercise6.Reps
            });

            DataAccessLayer.AddLinkedWorkoutsToExercises(new LinkedWorkoutsToExercises
            {
                WorkoutId = workout4.Id
                                                           ,
                ExerciseId = exercise7.Id
                                                           ,
                LengthOfTime = exercise7.LengthOfTime
                                                           ,
                Reps = exercise7.Reps
            });

            DataAccessLayer.AddLinkedWorkoutsToExercises(new LinkedWorkoutsToExercises
            {
                WorkoutId = workout4.Id
                                                           ,
                ExerciseId = exercise8.Id
                                                           ,
                LengthOfTime = exercise8.LengthOfTime
                                                           ,
                Reps = exercise8.Reps
            });

            var viewModel = new WorkoutListViewModel(new List<Workout>()
                                                     {
                                                         workout1
                                                       , workout2
                                                       , workout3
                                                       , workout4
                                                     }
                                                   , DataAccessLayer);

            return viewModel;
        }

        private Exercise AddNewExercise(string name
                                      , string description
                                      , string lengthOfTime
                                      , int reps)
        {
            var exercise = new Exercise
            {
                Name = name
                             ,
                Description = description
                             ,
                LengthOfTime = lengthOfTime
                             ,
                Reps = reps
            };

            DataAccessLayer.AddNewExercise(exercise);

            return exercise;
        }

        private Workout AddNewWorkout(string name
                                    , string description
                                    , int difficulty)
        {
            var workout = new Workout
            {
                Name = name
                            ,
                Description = description
                            ,
                Difficulty = difficulty
            };

            DataAccessLayer.AddNewWorkout(workout);

            return workout;
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