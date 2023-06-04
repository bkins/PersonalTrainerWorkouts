using Avails.D_Flat.Exceptions;
using Avails.D_Flat.Extensions;

using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.ViewModels;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Converters;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts;
using Xamarin.Essentials;

using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class EndToEnds
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private static   Database          _database;
        // private static   ContactsDataStore _contactsDataStore;
        private static readonly string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                                                           , "P2Database_test.db3");

        public static Database          Database => _database ??= new Database(DbPath);
        // public static ContactsDataStore ContactsDataStore = _contactsDataStore ??= new ContactsDataStore();
        public static DataAccess        DataAccessLayer { get; set; }

        public static TestData TestData { get; set; }

        public EndToEnds(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _testOutputHelper.WriteLine($"Database path: {DbPath}");

            // DataAccessLayer = new DataAccess(Database, ContactsDataStore);
            DataAccessLayer = new DataAccess(Database);
            TestData        = new TestData();
        }

        [Fact]
        public void AddClient()
        {
            //The ClientViewModel's DB is C:\Users\Ben\AppData\Local\WorkoutDatabase.db3 instead of the path defined above
            //TODO: Fix this ^^^
            
            //RefreshDatabase();
            var deviceContacts = new List<Contact>
                                 {
                                     new Contact
                                     {
                                         Emails = new List<ContactEmail>
                                                  {
                                                      new ContactEmail
                                                      {
                                                          EmailAddress = "contact1@email.com"
                                                      }
                                                  }
                                       , FamilyName = "Rogers"
                                       , GivenName  = "Fred"
                                       , Id         = "001"
                                       , MiddleName = "McFeely"
                                       , NamePrefix = "Mr."
                                       , NameSuffix = "III"
                                       , Phones = new List<ContactPhone>
                                                  {
                                                      new ContactPhone
                                                      {
                                                          PhoneNumber = "123-456-7890"
                                                      }
                                                  }
                                     }

                                 };
            
            var contactDataStore = new ContactsDataStore(deviceContacts);
            var appContact       = new AppContact(deviceContacts.FirstOrDefault());

            var client = new Client(appContact.ToContact(contactDataStore, appContact))
                         {
                             AppContact = appContact
                         };

            client.SetMainNumber();
            client.SetName();
            
            SaveClient(client);
            
            Assert.True(client.Id > 0);
            
            var clients = DataAccessLayer.GetClients();
            
            _testOutputHelper.WriteLine("Clients: ");
            foreach (var foundClient in clients)
            {
                _testOutputHelper.WriteLine(foundClient.ToString());
            }
        }

        private void SaveClient(Client client)
        {
            if (client.Id == 0)
            { //Add new

                client.Contact = new Contact();
                // DataAccessLayer.AddNewClientWithChildren(client);
                
                DataAccessLayer.AddNewClient(client);
                
                foreach (var phoneNumber in client.PhoneNumbers)
                {
                    phoneNumber.ClientId = client.Id;
                    DataAccessLayer.AddNewPhone(phoneNumber);
                }

                client.AppContact.ClientId = client.Id;
                
                DataAccessLayer.AddNewContact(client.AppContact);
                
                var newClient = DataAccessLayer.GetClients()
                                               .First(dbClient => dbClient.Id == client.Id);
            }
            else
            { //Update existing
                DataAccessLayer.UpdateClient(client);
            }
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

            var workout = TestData.Workout;
            var exercises = AddExercise(workout).ToList();

            Assert.Single(exercises);
            Assert.Equal(1, exercises.First().Id);
        }

        [Fact(Skip = "Broken - Need to rewrite.  Some of the aspects of this test are not valid (e.g. AddCompleteWorkout - I will never (as it is written right now) build a complete Workout object, then add it to the DB")]
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
            //Replace with what will really happen in the app
            //Database.AddCompleteWorkout(workout);

            Assert.True(workout.Id > 0);

            var theExercise = Assert.Single(workout.Exercises);
            Assert.True(theExercise.Id > 0);

            var theEquipment = Assert.Single(workout.Exercises.First().Equipment);
            Assert.True(theEquipment.Id > 0);
        }

        [Fact(Skip = "Fix or remove.  Uses AddCompleteWorkout (which was removed) and MusclesGroups need to be replaced with Synergists")]
        public void AddMuscleGroupToExerciseInWorkoutTest()
        {
            RefreshDatabase();

            var workout = TestData.Workout;

            workout.Exercises = new List<Exercise>()
                                {
                                    TestData.Exercise1
                                };

            //workout.Exercises
            //       .First()
            //       .MuscleGroups = new List<MuscleGroup>()
            //                       {
            //                           TestData.Bicep
            //                       };

            //Database.AddCompleteWorkout(workout);

            Assert.True(workout.Id > 0);

            var theExercise = Assert.Single(workout.Exercises);
            Assert.True(theExercise.Id > 0);

            //var theMuscleGroup = Assert.Single(workout.Exercises.First().MuscleGroups);
            //Assert.True  (theMuscleGroup.Id > 0);
        }

        [Fact]
        public void AddMuscleGroupWithOpposingMuscleGroupToExerciseInWorkoutTest()
        {
            RefreshDatabase();

            // 1. Build each entity and add to the DB as it is built
            var workout = TestData.Workout;
            var exercise = new Exercise
            {
                Name = "Exercise Test for MuscleGroups/Synergists"
            };

            var exerciseAddEditViewModel = new ExerciseAddEditViewModel
            {
                DataAccessLayer = DataAccessLayer
            };
            //This is simulating an already added Workout.
            //The Use-Case is the
            //Workout would be added,
            //the Workout is opened,
            //then the Exercise is created and added to the Workout. 
            var workoutId = exerciseAddEditViewModel.DataAccessLayer.AddNewWorkout(workout);

            exerciseAddEditViewModel.Workout = workout;
            exerciseAddEditViewModel.Exercise = exercise;
            exerciseAddEditViewModel.Workout.Exercises.Add(exerciseAddEditViewModel.Exercise);
            exerciseAddEditViewModel.SaveExercise(workoutId);

            var muscleGroupId = exerciseAddEditViewModel.DataAccessLayer.AddNewMuscleGroup(TestData.Bicep);
            var opposingMuscleGroupId = exerciseAddEditViewModel.DataAccessLayer.AddNewMuscleGroup(TestData.Tricep);

            var bicepTricepSynergist = new Synergist
            {
                ExerciseId = exerciseAddEditViewModel.Exercise.Id
                                          ,
                MuscleGroupId = muscleGroupId
                                          ,
                OpposingMuscleGroupId = opposingMuscleGroupId
            };

            var tricepBicepSynergist = new Synergist
            {
                ExerciseId = exerciseAddEditViewModel.Exercise.Id
                                         ,
                MuscleGroupId = opposingMuscleGroupId
                                         ,
                OpposingMuscleGroupId = muscleGroupId
            };

            exerciseAddEditViewModel.DataAccessLayer.AddSynergist(bicepTricepSynergist);
            exerciseAddEditViewModel.DataAccessLayer.AddSynergist(tricepBicepSynergist);

            workout.Exercises.First()
                   .Synergists = new List<Synergist>()
                                 {
                                     bicepTricepSynergist
                                   , tricepBicepSynergist
                                 };

            // 2. Query DB for the newly created object/relationships above
            var workoutFromDb = DataAccessLayer.GetWorkout(workoutId);

            // 3. Assert that all the values are present and accurate
            Assert.True(workoutFromDb.Id != 0);
            Assert.True(workoutFromDb.Exercises.Any());
            Assert.True(workoutFromDb.Exercises.First().Id != 0);
            Assert.True(workoutFromDb.Exercises.First().Synergists.Any());
            Assert.True(workoutFromDb.Exercises.First().Synergists.First().Id != 0);
        }

        [Fact(Skip = "Fix or rewrite. Uses AddCompleteWorkout (which was removed) and replace reference to MuscleGroups with Synergists")]
        public void AddTypeOfExerciseToExerciseInWorkout()
        {
            RefreshDatabase();

            // 1. Create the base Workout and add it to the database
            //var workout = new Workout
            //              {
            //                  Name           = "Workout 1"
            //                , CreateDateTime = DateTime.Now
            //                , Description    = "Workout One description."
            //                , Difficulty     = 1
            //                , Exercises = new List<Exercise>()
            //                              {
            //                                  new()
            //                                  {
            //                                      Name         = "Exercise 1"
            //                                    , Description  = "Exercise One description."
            //                                    , LengthOfTime = "5:00"
            //                                    , MuscleGroups = new List<MuscleGroup>
            //                                                     {
            //                                                         new ()
            //                                                         {
            //                                                             Name = "Muscle Group 1"
            //                                                         }
            //                                                     }
            //                                  }
            //                              }
            //              };
            //Database.AddCompleteWorkout(workout);

            // 2. Find the Muscle Group that the new Opposing Muscle Group will be associated with
            //var muscleGroupId = workout.Exercises.First()
            //                           .MuscleGroups.First()
            //                           .Id;

            // 3. Create the new Opposing Muscle Group and add it to the database
            //Database.AddMuscleGroups(new MuscleGroup{Name = "Opposing Muscle Group"});
            var opposingMuscleGroup = Database.GetMuscleGroups().First(field => field.Name == "Opposing Muscle Group");

            // 4. Insert the relationship between the Muscle Group and Opposing Muscle Group in the OpposingMuscleGroup table
            //Database.AddOpposingMuscleGroup(muscleGroupId, opposingMuscleGroup.Id);

            var testMuscleGroup = Database.GetMuscleGroups();
            //var newOpposingMuscleGroups = Database.GetOpposingMuscleGroupByMuscleGroup(muscleGroupId);

            // 5. SaveWorkoutsToExercise that newly created relationship in the object and the database.
            //workout.Exercises.First()
            //       .MuscleGroups.First()
            //       .OpposingMuscleGroup = newOpposingMuscleGroups;

            //Database.UpdateWorkout(workout);

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
            //workout.Exercises.First()
            //       .TypesOfExercise = thisExercisesTypes;

            //Assert.Single(workout.Exercises.First().TypesOfExercise);
            //Assert.True(workout.Exercises.First().TypesOfExercise.First().Id == newTypeOfExerciseId);
        }

        [Fact(Skip = "Fix or rewrite. Uses AddCompleteWorkout (which was removed) and replace reference to MuscleGroups with Synergists")]
        public void AddEquipmentToExerciseInWorkout()
        {
            RefreshDatabase();

            // 1. Create the base Workout and add it to the database
            //var workout = new Workout
            //              {
            //                  Name           = "Workout 1"
            //                , CreateDateTime = DateTime.Now
            //                , Description    = "Workout One description."
            //                , Difficulty     = 1
            //                , Exercises = new List<Exercise>()
            //                              {
            //                                  new()
            //                                  {
            //                                      Name         = "Exercise 1"
            //                                    , Description  = "Exercise One description."
            //                                    , LengthOfTime = "5:00"
            //                                    , MuscleGroups = new List<MuscleGroup>
            //                                                     {
            //                                                         new ()
            //                                                         {
            //                                                             Name = "Muscle Group 1"
            //                                                         }
            //                                                     }
            //                                  }
            //                              }
            //              };
            //Database.AddCompleteWorkout(workout);

            // 2. Find the Muscle Group that the new Opposing Muscle Group will be associated with
            //var muscleGroupId = workout.Exercises.First()
            //                           .MuscleGroups.First()
            //                           .Id;

            // 3. Create the new Opposing Muscle Group and add it to the database
            //Database.AddMuscleGroups(new MuscleGroup{Name = "Opposing Muscle Group"});
            var opposingMuscleGroup = Database.GetMuscleGroups().First(field => field.Name == "Opposing Muscle Group");

            // 4. Insert the relationship between the Muscle Group and Opposing Muscle Group in the OpposingMuscleGroup table
            //Database.AddOpposingMuscleGroup(muscleGroupId, opposingMuscleGroup.Id);

            var testMuscleGroup = Database.GetMuscleGroups();
            //var newOpposingMuscleGroups = Database.GetOpposingMuscleGroupByMuscleGroup(muscleGroupId);

            // 5. SaveWorkoutsToExercise that newly created relationship in the object and the database.
            //workout.Exercises.First()
            //       .MuscleGroups.First()
            //       .OpposingMuscleGroup = newOpposingMuscleGroups;

            //Database.UpdateWorkout(workout);

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
            //workout.Exercises.First()
            //       .TypesOfExercise = thisExercisesTypes;

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

            //workout.Exercises.First()
            //       .Equipment = thisExercisesEquipment;

            //Assert.Single(workout.Exercises.First().Equipment);
            //Assert.True(workout.Exercises.First().Equipment.First().Id == newEquipmentId);
        }


        [Fact(Skip = "Fix or rewrite. Uses AddCompleteWorkout and _database.AddOpposingMuscleGroup (both were removed) and replace reference to MuscleGroups with Synergists")]
        //[Fact]
        public void FullTestOfWorkout()
        {
            //Start with fresh DB
            Database.DropTables();
            Database.CreateTables();

            //Arrange
            var expectedWorkoutName = "Workout 1";
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

            //_database.AddMuscleGroups(bicep);
            //_database.AddMuscleGroups(tricep); 

            var aTypeOfExercise = new TypeOfExercise()
            {
                Name = "Some type of exercise"
            };

            var anotherTypeOfExercise = new TypeOfExercise()
            {
                Name = "Some other type of exercise"
            };

            //var exercise1 = new Exercise()
            //                {
            //                    Name         = expectedExerciseOneName
            //                  , Description  = $"{expectedExerciseOneName} description"
            //                  , LengthOfTime = "5:00"
            //                  , Equipment = new List<Equipment>()
            //                                {
            //                                    rack
            //                                  , dumbBells
            //                                }
            //                  , MuscleGroups = new List<MuscleGroup>()
            //                                   {
            //                                       bicep
            //                                   }
            //                  , TypesOfExercise = new List<TypeOfExercise>()
            //                                      {
            //                                          aTypeOfExercise
            //                                      }
            //                };


            //_database.AddOpposingMuscleGroup(bicep.Id
            //                               , tricep.Id);

            //exercise1.MuscleGroups.First()
            //         .OpposingMuscleGroup = tricep;

            //var exercise2 = new Exercise()
            //                {
            //                    Name         = expectedExerciseTwoName
            //                  , Description  = $"{expectedExerciseTwoName} description"
            //                  , LengthOfTime = "10:00"
            //                  , Equipment = new List<Equipment>()
            //                                {
            //                                    largeExerciseBall
            //                                  , recoveryBands
            //                                }
            //                  , MuscleGroups = new List<MuscleGroup>()
            //                                   {
            //                                       tricep
            //                                   }
            //                  , TypesOfExercise = new List<TypeOfExercise>()
            //                                      {
            //                                          anotherTypeOfExercise
            //                                      }
            //                };

            //var workout = new Workout
            //              {
            //                  Name           = expectedWorkoutName
            //                , Description    = $"{expectedWorkoutName} description"
            //                , CreateDateTime = DateTime.Now
            //                , Difficulty     = 1
            //                , Exercises = new List<Exercise>()
            //                              {
            //                                  exercise1
            //                                , exercise2
            //                              }
            //              };

            //Act
            //_database.AddCompleteWorkout(workout);

            //var muscleGroupId = workout.Exercises
            //                           .First(field => field.Name == expectedExerciseOneName)
            //                           .Id;

            var opposingMuscleGroupId = _database.GetMuscleGroups()
                                                 .First(field => field.Name == tricep.Name).Id;

            //Database.AddOpposingMuscleGroup(muscleGroupId, opposingMuscleGroupId);

            //muscleGroupId = workout.Exercises
            //                       .First(field => field.Name == expectedExerciseTwoName)
            //                       .Id;

            //Database.AddOpposingMuscleGroup(muscleGroupId, opposingMuscleGroupId);

            //Database.UpdateWorkout(workout);

            //BENDO:  Add more Asserts
            //var workoutGottenFromDatabase = _database.GetWorkout(workout.Id);

            //var opposingMuscleGroupFromDb = _database.GetMuscleGroup
            //        (
            //         workout.Exercises.First()
            //                .MuscleGroups.First()
            //                .OpposingMuscleGroup.Id
            //        );
            //Assert
            //Assert.True(opposingMuscleGroupFromDb.Name == opposingMuscleGroup.Name);

            //Passes
            //Assert.Equal(workout.Id, workoutGottenFromDatabase.Id);

            //Assert.Equal(workout.Exercises.Count, workoutGottenFromDatabase.Exercises.Count);
        }

        [Theory]
        [InlineData(false, false, false, Goal.Status.InProgress)]
        [InlineData(false, false, true, Goal.Status.InProgress)]
        [InlineData(false, true, false, Goal.Status.InProgress)]
        [InlineData(false, true, true, Goal.Status.InProgress)]
        [InlineData(true, false, false, Goal.Status.InProgress)]
        [InlineData(true, false, true, Goal.Status.CompletedSuccessfully)]
        [InlineData(true, true, false, Goal.Status.InProgress)]
        [InlineData(true, true, true, Goal.Status.CompletedSuccessfully)]
        
        public void GoalMeasurablesScenario(bool        doInitialMeasurables
                                          , bool        doIncompleteMeasurables
                                          , bool        doCompleteInterimMeasurables
                                          , Goal.Status expectedStatus)
        {
            //Setup
            //Start with fresh DB
            Database.DropTables();
            Database.CreateTables();

            var startingDate  = new DateTime(2023, 6, 3);
            var goalVm        = SetUpGoalViewModel(startingDate);
            var measurablesVm = new MeasurablesViewModel(goalVm.Goal.Id, DataAccessLayer);
            
            SetupAsserts(goalVm, measurablesVm);

            Measurable baselineMeasurable = null;

            //Step2            
            if(doInitialMeasurables) baselineMeasurable = TestInitialMeasurables(startingDate
                                                                               , goalVm
                                                                               , measurablesVm);
            
            //Step2a
            if (doIncompleteMeasurables 
             && baselineMeasurable is not null) TestCompleteInterimMeasurables(baselineMeasurable
                                                                             , startingDate
                                                                             , measurablesVm
                                                                             , goalVm);
            
            //Step3
            if(doCompleteInterimMeasurables
            && baselineMeasurable is not null) TestInterimMeasurables(baselineMeasurable
                                                                    , startingDate
                                                                    , measurablesVm
                                                                    , goalVm);
            //Final Step
            goalVm.CalculateStatus();

            Assert.Equal(expectedStatus, goalVm.Goal.GetStatus());

            PrintReport(goalVm, measurablesVm, "Final Step: Success", "Setting Goal status based on Measurables");
            //TestRecalculatingGoalStatusBasedOnMeasurables(goalVm, measurablesVm);
        }

        private void TestRecalculatingGoalStatusBasedOnMeasurables(GoalViewModel        goalVm
                                                                 , MeasurablesViewModel measurablesVm)
        {
            goalVm.CalculateStatus();

            Assert.Equal(Goal.Status.CompletedSuccessfully, goalVm.Goal.GetStatus());

            PrintReport(goalVm, measurablesVm, "Step4: Success", "Setting Goal status based on Measurables");
        }
        
        private void TestCompleteInterimMeasurables(Measurable           baselineMeasurable
                                                  , DateTime             startingDate
                                                  , MeasurablesViewModel measurablesVm
                                                  , GoalViewModel        goalVm)
        {

            SetIncompleteInterimMeasurables(baselineMeasurable, startingDate);

            measurablesVm.Refresh();

            PrintReport(goalVm, measurablesVm, "Step3: Success", "Interim measurables");
        }
        private void TestInterimMeasurables(Measurable           baselineMeasurable
                                          , DateTime             startingDate
                                          , MeasurablesViewModel measurablesVm
                                          , GoalViewModel        goalVm)
        {

            SetInterimMeasurables(baselineMeasurable, startingDate);

            measurablesVm.Refresh();

            PrintReport(goalVm, measurablesVm, "Step3: Success", "Interim measurables");
        }

        private Measurable TestInitialMeasurables(DateTime             startingDate
                                                , GoalViewModel        goalVm
                                                , MeasurablesViewModel measurablesVm)
        {

            var baselineMeasurable = SetBaselineAndTargetMeasurable(startingDate, goalVm);

            measurablesVm.Refresh();

            Assert.True(measurablesVm.Measurables.Any());

            PrintReport(goalVm, measurablesVm, "Step2: Success.", "Defined Baseline and Target Measurables");

            return baselineMeasurable;
        }

        private void SetupAsserts(GoalViewModel        goalVm
                             , MeasurablesViewModel measurablesVm)
        {
            Assert.Equal(1, goalVm.Goal.Id);
            Assert.True(goalVm.ClientName.HasValue());
            Assert.True(goalVm.ClientNameForDisplay.HasValue());
            Assert.Equal(Goal.Status.InProgress, goalVm.Goal.GetStatus());

            PrintReport(goalVm, measurablesVm, "Step1: Success.", "Client and Goals defined and associated.");
        }

        private static Measurable SetBaselineAndTargetMeasurable(DateTime      startingDate
                                                               , GoalViewModel newGoalVm)
        {

            var baselineMeasurable = new Measurable
                                         {
                                             Variable       = "Weight"
                                           , Value          = 195
                                           , GoalSuccession = Succession.Baseline
                                           , DateTaken      = startingDate
                                           , GoalId         = newGoalVm.Goal.Id
                                           , ClientId       = newGoalVm.Goal.ClientId
                                         };
            var targetMeasurable = new Measurable
                                       {
                                           Variable       = baselineMeasurable.Variable
                                         , Value          = 175
                                         , GoalSuccession = Succession.Target
                                         , DateTaken      = startingDate
                                         , GoalId         = newGoalVm.Goal.Id
                                         , ClientId       = newGoalVm.Goal.ClientId
                                       };
            Database.AddMeasurable(baselineMeasurable);
            Database.AddMeasurable(targetMeasurable);

            return baselineMeasurable;
        }

        private static void SetIncompleteInterimMeasurables(Measurable baselineMeasurable
                                                , DateTime   startingDate)
        {
            var interim1Measurables = new Measurable
                                          {
                                              Variable              = baselineMeasurable.Variable
                                            , GoalId                = baselineMeasurable.GoalId
                                            , ClientId              = baselineMeasurable.ClientId
                                            , GoalSuccession        = Succession.Interim
                                            , DateTaken             = startingDate.AddDays(7)
                                            , Value                 = 194
                                            , Type                  = "Measurement"
                                          };
            Database.AddMeasurable(interim1Measurables);

            var interim2Measurables = new Measurable
                                          {
                                              Variable              = baselineMeasurable.Variable
                                            , GoalId                = baselineMeasurable.GoalId
                                            , ClientId              = baselineMeasurable.ClientId
                                            , GoalSuccession        = Succession.Interim
                                            , DateTaken             = startingDate.AddDays(14)
                                            , Value                 = 192
                                            , Type                  = "Measurement"
                                          };
            
            Database.AddMeasurable(interim2Measurables);

            var interim3Measurables = new Measurable
                                          {
                                              Variable              = baselineMeasurable.Variable
                                            , GoalId                = baselineMeasurable.GoalId
                                            , ClientId              = baselineMeasurable.ClientId
                                            , GoalSuccession        = Succession.Interim
                                            , DateTaken             = startingDate.AddDays(21)
                                            , Value                 = 185
                                            , Type                  = "Measurement"
                                          };
            
            Database.AddMeasurable(interim3Measurables);

        }
        
        private static void SetInterimMeasurables(Measurable baselineMeasurable
                                                , DateTime   startingDate)
        {
            var interim1Measurables = new Measurable
                                          {
                                              Variable              = baselineMeasurable.Variable
                                            , GoalId                = baselineMeasurable.GoalId
                                            , ClientId              = baselineMeasurable.ClientId
                                            , GoalSuccession        = Succession.Interim
                                            , DateTaken             = startingDate.AddDays(7)
                                            , Value                 = 194
                                            , Type                  = "Measurement"
                                          };
            Database.AddMeasurable(interim1Measurables);

            var interim2Measurables = new Measurable
                                          {
                                              Variable              = baselineMeasurable.Variable
                                            , GoalId                = baselineMeasurable.GoalId
                                            , ClientId              = baselineMeasurable.ClientId
                                            , GoalSuccession        = Succession.Interim
                                            , DateTaken             = startingDate.AddDays(14)
                                            , Value                 = 192
                                            , Type                  = "Measurement"
                                          };
            
            Database.AddMeasurable(interim2Measurables);

            var interim3Measurables = new Measurable
                                          {
                                              Variable              = baselineMeasurable.Variable
                                            , GoalId                = baselineMeasurable.GoalId
                                            , ClientId              = baselineMeasurable.ClientId
                                            , GoalSuccession        = Succession.Interim
                                            , DateTaken             = startingDate.AddDays(21)
                                            , Value                 = 185
                                            , Type                  = "Measurement"
                                          };
            
            Database.AddMeasurable(interim3Measurables);

            var interim4Measurables = new Measurable
                                          {
                                              Variable              = baselineMeasurable.Variable
                                            , GoalId                = baselineMeasurable.GoalId
                                            , ClientId              = baselineMeasurable.ClientId
                                            , GoalSuccession        = Succession.Interim
                                            , DateTaken             = startingDate.AddDays(28)
                                            , Value                 = 175
                                            , Type                  = "Measurement"
                                          };
            
            Database.AddMeasurable(interim4Measurables);
        }

        private void PrintReport(GoalViewModel newGoalVm, MeasurablesViewModel measurablesViewModel, string title, string subTitle)
        {
            _testOutputHelper.WriteLine("");
            _testOutputHelper.WriteLine(title);
            _testOutputHelper.WriteLine($"       {subTitle}");

            _testOutputHelper.WriteLine("");
            _testOutputHelper.WriteLine($"  Client: {newGoalVm.ClientName}");
            _testOutputHelper.WriteLine($"    Goal: ");
            _testOutputHelper.WriteLine($"      {newGoalVm.Goal}");
            
            if (measurablesViewModel.Measurables.Any()) _testOutputHelper.WriteLine( "        Measurables:");
            
            foreach (var measurable in measurablesViewModel.Measurables)
            {
                _testOutputHelper.WriteLine($"            {measurable}");
            }

        }

        private static GoalViewModel SetUpGoalViewModel(DateTime startingDate)
        {
            var client = new Client
                             {
                                 Name = "Test Client"
                             };
            var goal = new Goal
                           {
                               Name             = "Lose 20lbs"
                             , DateStarted      = startingDate
                             , TargetComparison = TargetComparisons.CanBeLessThan
                           };


            client.Goals.Add(goal);

            Database.AddJustOneClientWithChildren(client);
            var newGoalVm = new GoalViewModel(goal, DataAccessLayer);

            return newGoalVm;
        }

        #region Helper methods

        private void RefreshDatabase()
        {
            DataAccessLayer.DropTables();
            DataAccessLayer.CreateTables();
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

        private void RefreshDatabaseTest()
        {
            RefreshDatabase();
        }

        #endregion

    }

    public class TestData
    {
        public string ExpectedWorkoutName => "Workout 1";
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
            Name = ExpectedExerciseOneName
                          ,
            Description = $"{ExpectedExerciseOneName} description"
                          ,
            LengthOfTime = "5:00"
                          ,
            Equipment = new List<Equipment>()
                                        {
                                            Rack
                                          , DumbBells
                                        }
                          /*, MuscleGroups = new List<MuscleGroup>()
                                           {
                                               Bicep
                                           }*/
                          ,
            TypesOfExercise = new List<TypeOfExercise>()
                                              {
                                                  OneTypeOfExercise
                                              }
        };

        public Exercise Exercise2 => new()
        {
            Name = ExpectedExerciseTwoName
                              ,
            Description = $"{ExpectedExerciseTwoName} description"
                              ,
            LengthOfTime = "10:00"
                              ,
            Equipment = new List<Equipment>()
                                            {
                                                LargeExerciseBall
                                              , RecoveryBands
                                            }
            /*
            , TypesOfExercise = new List<TypeOfExercise>()
                                {
                                    AnotherTypeOfExercise
                                }*/
        };

        public Workout Workout => new()
        {
            Name = "Workout 1"
                        ,
            CreateDateTime = DateTime.Now
                        ,
            Description = "Workout One description."
                        ,
            Difficulty = 1
        };
    }
}
