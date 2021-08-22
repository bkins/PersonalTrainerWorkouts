using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationExceptions;
using Xunit.Abstractions;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using SQLite;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xunit;
using static PersonalTrainerWorkouts.Data.DataAccess;

namespace Tests
{
    public class EndToEnds
    {
        private readonly        ITestOutputHelper _testOutputHelper;
        private static          Database          _database;
        private static readonly string            DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                                                           , "P2Database_test.db3");
        public static Database   Database        => _database ??= new Database(DbPath);
        public static DataAccess DataAccessLayer { get; set; }

        public static TestData   TestData        { get; set; }

        public EndToEnds(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _testOutputHelper.WriteLine($"Database path: {DbPath}");

            DataAccessLayer = new DataAccess(Database);
            TestData        = new TestData();
        }

        [Fact]
        public void AddWorkoutTest()
        {
            RefreshDatabase();
            
            Workout workout;

            try
            {
                workout = AddWorkout();
            }
            catch (WiringException e)
            {
                _testOutputHelper.WriteLine(e.Message);

                throw;
            }
            
            Assert.Equal(1, workout.Id);
        }

        [Fact]
        public void AddOneExerciseTest()
        {
            RefreshDatabase();

            var workout   = TestData.Workout;
            var exercises = AddExercise(workout).ToList();

            Assert.Single(exercises);
            Assert.Equal(1, exercises.First().Id);
        }

        [Fact]
        public void AddEquipmentToExerciseInWorkoutTest()
        {
            RefreshDatabase();

            var workout = TestData.Workout;

            workout.Exercises = new List<Exercise>()
                                {
                                    TestData.Exercise1
                                };

            workout.Exercises
                   .First()
                   .Equipment = new List<Equipment>()
                                {
                                    TestData.DumbBells
                                };
            
            Database.AddCompleteWorkout(workout);

            Assert.True  (workout.Id > 0);

            var theExercise = Assert.Single(workout.Exercises);
            Assert.True (theExercise.Id > 0);

            var theEquipment = Assert.Single(workout.Exercises.First().Equipment);
            Assert.True  (theEquipment.Id > 0);
        }
        
        [Fact]
        public void AddMuscleGroupToExerciseInWorkoutTest()
        {
            RefreshDatabase();

            var workout = TestData.Workout;

            workout.Exercises = new List<Exercise>()
                                {
                                    TestData.Exercise1
                                };

            workout.Exercises
                   .First()
                   .MuscleGroups = new List<MuscleGroup>()
                                   {
                                       TestData.Bicep
                                   };
            
            Database.AddCompleteWorkout(workout);

            Assert.True  (workout.Id > 0);

            var theExercise = Assert.Single(workout.Exercises);
            Assert.True  (theExercise.Id > 0);

            var theMuscleGroup = Assert.Single(workout.Exercises.First().MuscleGroups);
            Assert.True  (theMuscleGroup.Id > 0);
        }
        
        [Fact]
        public void AddMuscleGroupWithOpposingMuscleGroupToExerciseInWorkoutTest()
        {
            RefreshDatabase();

            // 1. Create the base Workout and add it to the database
            
            var workout = TestData.Workout;

            workout.Exercises = new List<Exercise>()
                                {
                                    TestData.Exercise1
                                };

            workout.Exercises
                   .First()
                   .MuscleGroups = new List<MuscleGroup>()
                                   {
                                       TestData.Bicep
                                   };
            
            Database.AddCompleteWorkout(workout);

            // 2. Find the Muscle Group that the new Opposing Muscle Group will be associated with
            var theMuscleGroup = Assert.Single(workout.Exercises
                                                      .First()
                                                      .MuscleGroups);
            
            // 3. Add the Opposing Muscle Group and add it to the database
            Database.AddMuscleGroups(TestData.Tricep);

            //4. Get newly added Opposing Muscle Group from the database
            MuscleGroup opposingMuscleGroup = Database.GetMuscleGroups()
                                                      .First(field => 
                                                             field.Name == TestData.Tricep.Name);
            
            // 5. Insert the relationship between the Muscle Group and Opposing Muscle Group in the OpposingMuscleGroup table
            Database.AddOpposingMuscleGroup(theMuscleGroup.Id, opposingMuscleGroup.Id);
            
            // 6. Get the junction table's OpposingMuscleGroup value
            var newOpposingMuscleGroups = Database.GetOpposingMuscleGroupByMuscleGroup(theMuscleGroup.Id);

            // 7. Save that newly created relationship in the object and the database.
            workout.Exercises.First()
                   .MuscleGroups.First()
                   .OpposingMuscleGroup = newOpposingMuscleGroups;

            Database.UpdateWorkout(workout);

            var allOpposingMuscleGroups = Database.GetOpposingMuscleGroups();

            Assert.True(allOpposingMuscleGroups.First().Id > 0);

            Assert.True  (workout.Exercises.First().MuscleGroups.First().Id > 0);

            Assert.True  (workout.Exercises.First()
                                 .MuscleGroups.First()
                                 .OpposingMuscleGroup.Id > 0
                         );
        }

        [Fact]
        public void AddTypeOfExerciseToExerciseInWorkout()
        {
            RefreshDatabase();

            // 1. Create the base Workout and add it to the database
            var workout = new Workout
                          {
                              Name           = "Workout 1"
                            , CreateDateTime = DateTime.Now
                            , Description    = "Workout One description."
                            , Difficulty     = 1
                            , Exercises = new List<Exercise>()
                                          {
                                              new()
                                              {
                                                  Name         = "Exercise 1"
                                                , Description  = "Exercise One description."
                                                , LengthOfTime = 5
                                                , MuscleGroups = new List<MuscleGroup>
                                                                 {
                                                                     new ()
                                                                     {
                                                                         Name = "Muscle Group 1"
                                                                     }
                                                                 }
                                              }
                                          }
                          };
            Database.AddCompleteWorkout(workout);

            // 2. Find the Muscle Group that the new Opposing Muscle Group will be associated with
            var muscleGroupId = workout.Exercises.First()
                                       .MuscleGroups.First()
                                       .Id;

            // 3. Create the new Opposing Muscle Group and add it to the database
            Database.AddMuscleGroups(new MuscleGroup{Name = "Opposing Muscle Group"});
            MuscleGroup opposingMuscleGroup = Database.GetMuscleGroups().First(field => field.Name == "Opposing Muscle Group");
            
            // 4. Insert the relationship between the Muscle Group and Opposing Muscle Group in the OpposingMuscleGroup table
            Database.AddOpposingMuscleGroup(muscleGroupId, opposingMuscleGroup.Id);

            var testMuscleGroup         = Database.GetMuscleGroups();
            var newOpposingMuscleGroups = Database.GetOpposingMuscleGroupByMuscleGroup(muscleGroupId);

            // 5. Save that newly created relationship in the object and the database.
            workout.Exercises.First()
                   .MuscleGroups.First()
                   .OpposingMuscleGroup = newOpposingMuscleGroups;

            Database.UpdateWorkout(workout);

            //Begin adding the Type Of Exercise
            var typeOfExercise = new TypeOfExercise
                                 {
                                     Name = "Type Of Exercise 1"
                                 };

            var newTypeOfExerciseId = Database.AddType(typeOfExercise);

            Assert.True(newTypeOfExerciseId > 0);

            var thisExercisesTypes = new List<TypeOfExercise>()
                                     {
                                         Database.GetTypeOfExercise(newTypeOfExerciseId)
                                     };
            workout.Exercises.First()
                   .TypesOfExercise = thisExercisesTypes;

            Assert.Single(workout.Exercises.First().TypesOfExercise);
            Assert.True(workout.Exercises.First().TypesOfExercise.First().Id == newTypeOfExerciseId);
        }

        [Fact]
        public void AddEquipmentToExerciseInWorkout()
        {
            RefreshDatabase();

            // 1. Create the base Workout and add it to the database
            var workout = new Workout
                          {
                              Name           = "Workout 1"
                            , CreateDateTime = DateTime.Now
                            , Description    = "Workout One description."
                            , Difficulty     = 1
                            , Exercises = new List<Exercise>()
                                          {
                                              new()
                                              {
                                                  Name         = "Exercise 1"
                                                , Description  = "Exercise One description."
                                                , LengthOfTime = 5
                                                , MuscleGroups = new List<MuscleGroup>
                                                                 {
                                                                     new ()
                                                                     {
                                                                         Name = "Muscle Group 1"
                                                                     }
                                                                 }
                                              }
                                          }
                          };
            Database.AddCompleteWorkout(workout);

            // 2. Find the Muscle Group that the new Opposing Muscle Group will be associated with
            var muscleGroupId = workout.Exercises.First()
                                       .MuscleGroups.First()
                                       .Id;

            // 3. Create the new Opposing Muscle Group and add it to the database
            Database.AddMuscleGroups(new MuscleGroup{Name = "Opposing Muscle Group"});
            MuscleGroup opposingMuscleGroup = Database.GetMuscleGroups().First(field => field.Name == "Opposing Muscle Group");
            
            // 4. Insert the relationship between the Muscle Group and Opposing Muscle Group in the OpposingMuscleGroup table
            Database.AddOpposingMuscleGroup(muscleGroupId, opposingMuscleGroup.Id);

            var testMuscleGroup         = Database.GetMuscleGroups();
            var newOpposingMuscleGroups = Database.GetOpposingMuscleGroupByMuscleGroup(muscleGroupId);

            // 5. Save that newly created relationship in the object and the database.
            workout.Exercises.First()
                   .MuscleGroups.First()
                   .OpposingMuscleGroup = newOpposingMuscleGroups;

            Database.UpdateWorkout(workout);

            //Begin adding the Type Of Exercise
            var typeOfExercise = new TypeOfExercise
                                 {
                                     Name = "Type Of Exercise 1"
                                 };

            var newTypeOfExerciseId = Database.AddType(typeOfExercise);
            
            var thisExercisesTypes = new List<TypeOfExercise>()
                                     {
                                         Database.GetTypeOfExercise(newTypeOfExerciseId)
                                     };
            workout.Exercises.First()
                   .TypesOfExercise = thisExercisesTypes;

            //Begin adding Equipment to the Exercise
            var equipment = new Equipment
                            {
                                Name = "Equipment 1"
                            };

            var newEquipmentId = Database.AddEquipment(equipment);

            var thisExercisesEquipment = new List<Equipment>()
                                         {
                                             Database.GetEquipment(newEquipmentId)
                                         };

            Assert.True(newEquipmentId > 0);

            workout.Exercises.First()
                   .Equipment = thisExercisesEquipment;

            Assert.Single(workout.Exercises.First().Equipment);
            Assert.True(workout.Exercises.First().Equipment.First().Id == newEquipmentId);
        }
        
        [Fact]
        public void RefreshDatabaseTest()
        {
            RefreshDatabase();
        }

        [Fact(Skip = "Broken")]
        //[Fact]
        public void FullTestOfWorkout()
        {
            //Start with fresh DB
            Database.DropTables();
            Database.CreateTables();

            //Arrange
            var expectedWorkoutName     = "Workout 1";
            var expectedExerciseOneName = "Exercise 1";
            var expectedExerciseTwoName = "Exercise 2";
            
            var rack = new Equipment
                       {
                           Name = "Rack"
                       };

            var dumbBells = new Equipment()
                            {
                                Name = "DumbBells"
                            };

            var largeExerciseBall = new Equipment()
                                    {
                                        Name = "Large Exercise Ball"
                                    };

            var mediumExerciseBall = new Equipment()
                                     {
                                         Name = "Medium Exercise Ball"
                                     };

            var smallExerciseBall = new Equipment()
                                    {
                                        Name = "Small Exercise Ball"
                                    };

            var recoveryBands = new Equipment()
                                {
                                    Name = "recoveryBands"
                                };

            var bicep = new MuscleGroup()
                               {
                                   Name = "Bicep"
                               };
            
            var tricep = new MuscleGroup()
                         {
                             Name = "Tricep"

                         };

            _database.AddMuscleGroups(bicep);
            _database.AddMuscleGroups(tricep); 

            var aTypeOfExercise = new TypeOfExercise()
                                  {
                                      Name = "Some type of exercise"
                                  };

            var anotherTypeOfExercise = new TypeOfExercise()
                                        {
                                            Name = "Some other type of exercise"
                                        };

            var exercise1 = new Exercise()
                            {
                                Name         = expectedExerciseOneName
                              , Description  = $"{expectedExerciseOneName} description"
                              , LengthOfTime = 5
                              , Equipment = new List<Equipment>()
                                            {
                                                rack
                                              , dumbBells
                                            }
                              , MuscleGroups = new List<MuscleGroup>()
                                               {
                                                   bicep
                                               }
                              , TypesOfExercise = new List<TypeOfExercise>()
                                                  {
                                                      aTypeOfExercise
                                                  }
                            };

            var exercise2 = new Exercise()
                            {
                                Name         = expectedExerciseTwoName
                              , Description  = $"{expectedExerciseTwoName} description"
                              , LengthOfTime = 10
                              , Equipment = new List<Equipment>()
                                            {
                                                largeExerciseBall
                                              , recoveryBands
                                            }
                              , MuscleGroups = new List<MuscleGroup>()
                                               {
                                                   tricep
                                               }
                              , TypesOfExercise = new List<TypeOfExercise>()
                                                  {
                                                      anotherTypeOfExercise
                                                  }
                            };
            
            var workout = new Workout
                          {
                              Name           = expectedWorkoutName
                            , Description    = $"{expectedWorkoutName} description"
                            , CreateDateTime = DateTime.Now
                            , Difficulty     = 1
                            , Exercises = new List<Exercise>()
                                          {
                                              exercise1
                                            , exercise2
                                          }
                          };

            //Act
            _database.AddCompleteWorkout(workout);

            var muscleGroupId = workout.Exercises
                                       .First(field => field.Name == expectedExerciseOneName)
                                       .Id;

            var opposingMuscleGroupId = _database.GetMuscleGroups()
                                                 .First(field => field.Name == tricep.Name).Id;
            
            Database.AddOpposingMuscleGroup(muscleGroupId, opposingMuscleGroupId);

            muscleGroupId = workout.Exercises
                                   .First(field => field.Name == expectedExerciseTwoName)
                                   .Id;

            opposingMuscleGroupId = _database.GetMuscleGroups()
                                             .First(field => field.Name == bicep.Name)
                                             .Id;
            
            Database.AddOpposingMuscleGroup(muscleGroupId, opposingMuscleGroupId);
            
            Database.UpdateWorkout(workout);

            //BENDO:  Add more Asserts
            var workoutGottenFromDatabase = _database.GetWorkout(workout.Id);

            var opposingMuscleGroupFromDb = _database.GetMuscleGroup
                    (
                     workout.Exercises.First()
                            .MuscleGroups.First()
                            .OpposingMuscleGroup.Id
                    );
            //Assert
            //Assert.True(opposingMuscleGroupFromDb.Name == opposingMuscleGroup.Name);

            //Passes
            Assert.Equal(workout.Id, workoutGottenFromDatabase.Id);

            Assert.Equal(workout.Exercises.Count, workoutGottenFromDatabase.Exercises.Count);
        }

    #region Helper methods

        private void RefreshDatabase()
        {
            Database.DropTables();
            Database.CreateTables();
        }

        private IEnumerable<Exercise> AddExercise(Workout workout)
        {
            var exercise = TestData.Exercise1;
            
            workout.Exercises.Add(exercise);
            Database.AddWorkoutWithExercises(workout);

            return Database.GetExercises();
        }

        private static Workout AddWorkout()
        {
            var workout = TestData.Workout;

            if (DataAccessLayer == null)
                throw new WiringException($"Problem accessing the {nameof(DataAccess)} -- {nameof(DataAccessLayer)} is null."
                                          , nameof(EndToEnds)
                                          , nameof(DataAccess));

            DataAccessLayer.AddNewWorkout(workout);

            //Database.AddJustOneWorkout(workout);

            return workout;
        }
        
    #endregion
        
    }

    public class TestData
    {
        public string ExpectedWorkoutName     => "Workout 1";
        public string ExpectedExerciseOneName => "Exercise 1";
        public string ExpectedExerciseTwoName => "Exercise 2";

        public Equipment Rack => new()
                                 {
                                     Name = "Rack"
                                 };

        public Equipment DumbBells => new()
                                      {
                                          Name = "DumbBells"
                                      };

        public Equipment LargeExerciseBall => new()
                                              {
                                                  Name = "Large Exercise Ball"
                                              };

        public Equipment MediumExerciseBall => new()
                                               {
                                                   Name = "Medium Exercise Ball"
                                               };

        public Equipment SmallExerciseBall => new()
                                              {
                                                  Name = "Small Exercise Ball"
                                              };

        public Equipment RecoveryBands => new()
                                          {
                                              Name = "recoveryBands"
                                          };

        public MuscleGroup Bicep => new()
                                    {
                                        Name = "Bicep"
                                    };

        public MuscleGroup Tricep => new()
                                     {
                                         Name = "Tricep"
                                     };
        
            public TypeOfExercise OneTypeOfExercise => new()
                                  {
                                      Name = "Some type of exercise"
                                  };

            public TypeOfExercise AnotherTypeOfExercise => new()
                                        {
                                            Name = "Some other type of exercise"
                                        };

            public Exercise Exercise1 => new()
                            {
                                Name         = ExpectedExerciseOneName
                              , Description  = $"{ExpectedExerciseOneName} description"
                              , LengthOfTime = 5
                              , Equipment = new List<Equipment>()
                                            {
                                                Rack
                                              , DumbBells
                                            }
                              , MuscleGroups = new List<MuscleGroup>()
                                               {
                                                   Bicep
                                               }
                              , TypesOfExercise = new List<TypeOfExercise>()
                                                  {
                                                      OneTypeOfExercise
                                                  }
                            };

            public Exercise Exercise2 => new()
                            {
                                Name         = ExpectedExerciseTwoName
                              , Description  = $"{ExpectedExerciseTwoName} description"
                              , LengthOfTime = 10
                              , Equipment = new List<Equipment>()
                                            {
                                                LargeExerciseBall
                                              , RecoveryBands
                                            }
                              , MuscleGroups = new List<MuscleGroup>()
                                               {
                                                   Tricep
                                               }
                              , TypesOfExercise = new List<TypeOfExercise>()
                                                  {
                                                      AnotherTypeOfExercise
                                                  }
                            };
            
        public Workout Workout => new()
                      {
                          Name           = "Workout 1"
                        , CreateDateTime = DateTime.Now
                        , Description    = "Workout One description."
                        , Difficulty     = 1
                      };
    }
}
