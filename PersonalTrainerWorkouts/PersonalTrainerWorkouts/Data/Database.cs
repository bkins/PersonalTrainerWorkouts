using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Data.Interfaces;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.AppContacts;
using PersonalTrainerWorkouts.Models.Intermediates;

using SQLite;

using SQLiteNetExtensions.Extensions;

using Syncfusion.DataSource.Extensions;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using InvalidOperationException = System.InvalidOperationException;
using SequenceContainsNoElementsException = Avails.D_Flat.Exceptions.SequenceContainsNoElementsException;
using TypeOfExercise = PersonalTrainerWorkouts.Models.TypeOfExercise;

namespace PersonalTrainerWorkouts.Data
{
    public partial class Database : IDataStore
    {
        //BENDO:  Implement the use of forceRefresh in methods that use it
        //(and add to methods that it makes sense to add it to)
        private readonly SQLiteConnection _database;
        private readonly string _path;

        public Database(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            _path = dbPath;

            var databaseVersion = GetDatabaseVersion();

            CreateTables();
            Check();
        }

        private int GetDatabaseVersion()
        {
            return _database.ExecuteScalar<int>("pragma user_version");
        }

        public string DbPath()
        {
            return _database.DatabasePath;
        }

        public void CreateTables()
        {
            try
            {
                _database.CreateTable<Workout>();
                _database.CreateTable<Exercise>();
                _database.CreateTable<TypeOfExercise>();
                _database.CreateTable<Equipment>();
                _database.CreateTable<MuscleGroup>();
                _database.CreateTable<Synergist>();
                _database.CreateTable<OpposingMuscleGroup>();

                CreateContactTables();

                _database.CreateTable<ExerciseType>();
                _database.CreateTable<ExerciseEquipment>();
                _database.CreateTable<ExerciseMuscleGroup>();
                _database.CreateTable<WorkoutExercise>();
                _database.CreateTable<LinkedWorkoutsToExercises>();
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);
            }
        }

        public void DropTables()
        {
            _database.DropTable<Workout>();
            _database.DropTable<Exercise>();
            _database.DropTable<TypeOfExercise>();
            _database.DropTable<Equipment>();
            _database.DropTable<MuscleGroup>();
            _database.DropTable<Synergist>();
            _database.DropTable<OpposingMuscleGroup>();

            DropContactTables();

            _database.DropTable<ExerciseType>();
            _database.DropTable<ExerciseEquipment>();
            _database.DropTable<ExerciseMuscleGroup>();
            _database.DropTable<WorkoutExercise>();
            _database.DropTable<LinkedWorkoutsToExercises>();
        }

        public void CreateContactTables()
        {
            _database.CreateTable<Client>();
            _database.CreateTable<Session>();
            _database.CreateTable<Address>();
            _database.CreateTable<PhoneNumber>();
            _database.CreateTable<Measurable>();
            _database.CreateTable<UnitOfMeasurement>();
            _database.CreateTable<Goal>();

            _database.CreateTable<AppContact>();
            _database.CreateTable<AppContactPhone>();
            _database.CreateTable<AppContactEmail>();
        }

        public void DropContactTables()
        {
            _database.DropTable<Client>();
            _database.DropTable<Session>();
            _database.DropTable<Address>();
            _database.DropTable<PhoneNumber>();
            _database.DropTable<Measurable>();
            _database.DropTable<UnitOfMeasurement>();
            _database.DropTable<Goal>();

            _database.DropTable<AppContact>();
            _database.DropTable<AppContactPhone>();
            _database.DropTable<AppContactEmail>();


        }

        public string GetFilePath()
        {
            return _path;
        }

        public string GetFileName()
        {
            var fileName = new FileInfo(_path).Name;

            return fileName;
        }

        public bool Check()
        {
            var mappings = _database.TableMappings;

            return false;

            //return _database.Query("integrity_check");

            ////("PRAGMA tempstore = 2; " 
            ////        + " CREATE TEMP TABLE _Variables " 
            ////        + " ( id INTEGER PRIMARY KEY " 
            ////        + ",[@igroupId] INTEGER " 
            ////        + ",[@icount] INTEGER " 
            ////        + ",[@imessageId] BLOB " 
            ////        + ",[@itakePrevious] INTEGER " 
            ////        + ",[@vmessageDate] TEXT); " 
            ////        + " INSERT INTO _Variables(id)VALUES(1); ");
        }



        public List<string> GetTables()
        {
            // const string query = "SELECT name FROM sqlite_master " +
            //                      "WHERE type = 'table'" +
            //                      "ORDER BY 1";

            var result = new List<string>();

            try
            {
                result = _database.GetAllWithChildren<Table>().ToList().ConvertAll(table => table.ToString());
            }
            catch (Exception e)
            {
                Logger.WriteLine("Could not get values from sqlite_master.", Category.Error, e);
            }

            return result;
        }

        private static string GetNoElementsExceptionMessage(string nameOfIdVariable
                                                          , int valueOfVariable)
        {
            return $"Record(s) could not be found. No records with {nameOfIdVariable} of {valueOfVariable}.";
        }

        private static string GetNoElementsExceptionMessage(string nameOfFirstIdVariable
                                                          , string nameOfSecondIdVariable
                                                          , int valueOfFirstIdVariable
                                                          , int valueOfSecondIdVariable
                                                          , char pluralChar = '\0')
        {
            return $"Record{pluralChar} could not be found. No record{pluralChar} with {nameOfFirstIdVariable} of {valueOfFirstIdVariable} and {nameOfSecondIdVariable} of {valueOfSecondIdVariable}.";
        }

    }

    public partial class Database //Gets
    {
        public IEnumerable<PhoneNumber> GetAllPhoneNumbers(bool forceRefresh = false) => throw new NotImplementedException();

        public IEnumerable<Address> GetAllAddresses(bool forceRefresh = false) => throw new NotImplementedException();

        public Address GetAddress(int addressId) => throw new NotImplementedException();

        public Address GetPhoneNumber(int phoneNumberId) => throw new NotImplementedException();

        public IEnumerable<Measurable> GetAllMeasurables(bool forceRefresh = false) => throw new NotImplementedException();

        public Address GetMeasurable(int measurableId) => throw new NotImplementedException();

        public Client GetClient(int clientId)
        {
            try
            {
                var client = _database.GetWithChildren<Client>(clientId);

                return client;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(clientId)
                                                                , clientId)
                                                            , typeof(Client).ToString()
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }
        //BENDO: For all methods that take forceRefresh, call method that will rebuild the object by calling the database to get current values
        //       necessary to fill the object.
        public Workout GetWorkout(int workoutId)
        {
            try
            {
                var workout = _database.GetWithChildren<Workout>(workoutId);
                var workoutsToExercises = GetAllLinkedWorkoutsToExercises(workoutId);

                foreach (var workoutsToExercise in workoutsToExercises)
                {
                    workout.Exercises.Add(GetExercise(workoutsToExercise.ExerciseId));
                }

                return workout;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(workoutId)
                                                                                          , workoutId)
                                                            , typeof(Workout).ToString()
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<Workout> GetWorkouts(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<Workout>();
        }

        public IEnumerable<Session> GetSessions(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<Session>();
        }

        public IEnumerable<Client> GetClients(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<Client>();
        }

        public LinkedWorkoutsToExercises GetLinkedWorkoutsToExercise(int linkedWorkoutsToExercisesId)
        {
            try
            {
                var linkedWorkoutsToExercises = _database.Get<LinkedWorkoutsToExercises>(linkedWorkoutsToExercisesId);

                return linkedWorkoutsToExercises;
            }

            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(linkedWorkoutsToExercisesId)
                                                                                          , linkedWorkoutsToExercisesId)
                                                            , nameof(LinkedWorkoutsToExercises)
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public WorkoutExercise GetWorkoutExercise(int workoutExerciseId)
        {
            try
            {
                var workoutExercise = _database.GetWithChildren<WorkoutExercise>(workoutExerciseId);

                //BENDO:  Build a complete Workout by following the example in ThingsToRemember: SetJournalsEntriesWithMoods(journal);
                //Get all entities that are associated with this (workoutExerciseId) WorkoutExercise

                return workoutExercise;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(workoutExerciseId)
                                                                                          , workoutExerciseId)
                                                            , nameof(WorkoutExercise)
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<LinkedWorkoutsToExercises> GetAllLinkedWorkoutsToExercises(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<LinkedWorkoutsToExercises>();
        }

        public IEnumerable<LinkedWorkoutsToExercises> GetAllLinkedWorkoutsToExercises(int workoutId)
        {
            try
            {
                var linkedWorkoutsToExercises = _database.GetAllWithChildren<LinkedWorkoutsToExercises>()
                                                         .Where(item => item.WorkoutId == workoutId);

                return linkedWorkoutsToExercises;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(workoutId)
                                                                                          , workoutId)
                                                            , typeof(LinkedWorkoutsToExercises).ToString()
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<WorkoutExercise> GetWorkoutExercises(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<WorkoutExercise>();
        }

        public IEnumerable<WorkoutExercise> GetWorkoutExercises(int workoutId)
        {
            try
            {
                var workoutExercises = _database.GetAllWithChildren<WorkoutExercise>()
                                                .Where(item => item.WorkoutId == workoutId);

                return workoutExercises;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(workoutId)
                                                                                          , workoutId)
                                                            , typeof(ExerciseEquipment).ToString()
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<LinkedWorkoutsToExercises> GetAllLinkedWorkoutsToExercises(int workoutId
                                                                                    , int exerciseId)
        {
            try
            {
                var linkedWorkoutsToExercises = _database.GetAllWithChildren<LinkedWorkoutsToExercises>()
                                                         .Where(item => item.WorkoutId == workoutId && item.ExerciseId == exerciseId);

                return linkedWorkoutsToExercises;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(workoutId)
                                                                                          , nameof(exerciseId)
                                                                                          , workoutId
                                                                                          , exerciseId
                                                                                          , 's')
                                                            , typeof(LinkedWorkoutsToExercises).ToString()
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<WorkoutExercise> GetWorkoutExercises(int workoutId
                                                              , int exerciseId)
        {
            try
            {
                var workoutExercises = _database.GetAllWithChildren<WorkoutExercise>()
                                                .Where(item => item.WorkoutId == workoutId && item.ExerciseId == exerciseId);

                //BENDO:  Build a complete Workout by following the example in ThingsToRemember: SetJournalsEntriesWithMoods(journal);
                //Get all entities that are associated with this (workoutId) journal

                return workoutExercises;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(workoutId)
                                                                                          , nameof(exerciseId)
                                                                                          , workoutId
                                                                                          , exerciseId
                                                                                          , 's')
                                                            , typeof(ExerciseEquipment).ToString()
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<LinkedWorkoutsToExercises> GetLinkedWorkoutsToExercisesByWorkouts(int workoutId)
        {
            try
            {
                var linkedWorkoutExercises = _database.GetAllWithChildren<LinkedWorkoutsToExercises>()
                                                      .Where(field => field.WorkoutId == workoutId);

                return linkedWorkoutExercises;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(workoutId)
                                                                                          , workoutId)
                                                            , nameof(ExerciseEquipment)
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<WorkoutExercise> GetWorkoutExercisesByWorkout(int workoutId)
        {
            try
            {
                var allWorkoutExercises = _database.GetAllWithChildren<WorkoutExercise>();

                var workoutExercises = allWorkoutExercises.Where(item => item.WorkoutId == workoutId);
                /*
                //BENDO:  Build a complete Workout by following the example in ThingsToRemember: SetJournalsEntriesWithMoods(journal);
                //Get all entities that are associated with this (workoutId) journal
                   var entries = GetEntries()
                         .ToList()
                         .Where(item => item.JournalId == journal.Id);

                   journal.Entries = entries.ToList(); //Overwrite entries.  For they will not have Moods
                */

                return workoutExercises;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(workoutId)
                                                                                          , workoutId)
                                                            , nameof(ExerciseEquipment)
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<WorkoutExercise> GetWorkoutExercisesByExercise(int exerciseId)
        {
            try
            {
                var workoutExercises = _database.GetAllWithChildren<WorkoutExercise>()
                                                .Where(item => item.WorkoutId == exerciseId);

                //BENDO:  Build a complete Workout by following the example in ThingsToRemember: SetJournalsEntriesWithMoods(journal);
                //Get all entities that are associated with this (workoutId) journal

                return workoutExercises;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(exerciseId)
                                                                                          , exerciseId)
                                                            , nameof(ExerciseEquipment)
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public Exercise GetExercise(int exerciseId)
        {
            try
            {
                var exercise = _database.GetWithChildren<Exercise>(exerciseId);

                return exercise;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(exerciseId)
                                                                                          , exerciseId)
                                                            , nameof(ExerciseEquipment)
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<Exercise> GetExercises(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<Exercise>();
        }

        public ObservableCollection<Exercise> GetObservableExercises(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<Exercise>()
                            .ToObservableCollection();
        }

        public IEnumerable<Synergist> GetSynergists(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<Synergist>();
        }

        public IEnumerable<ExerciseMuscleGroup> GetExerciseMuscleGroups(bool forceRefresh = false)
        {
            var exerciseMuscleGroups = _database.GetAllWithChildren<ExerciseMuscleGroup>();

            if (!exerciseMuscleGroups.Any())
            {
                throw new SequenceContainsNoElementsException($"No {nameof(ExerciseMuscleGroup)} founds.  No Records in the table '{nameof(ExerciseMuscleGroup)}'."
                                                            , nameof(ExerciseMuscleGroup)
                                                            , null);
            }

            return exerciseMuscleGroups;
        }

        public IEnumerable<MuscleGroup> GetMuscleGroups(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<MuscleGroup>();
        }

        public MuscleGroup GetOpposingMuscleGroupByMuscleGroup(int muscleGroupId)
        {
            try
            {
                var opposingMuscleGroup = GetMuscleGroups()
                       .First(field => field.Id == muscleGroupId);

                return opposingMuscleGroup;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(muscleGroupId)
                                                                                          , muscleGroupId)
                                                            , nameof(ExerciseEquipment)
                                                            , operationException);
            }
        }

        public TypeOfExercise GetTypeOfExercise(int typeOfExerciseId)
        {
            try
            {
                var exerciseType = _database.GetWithChildren<TypeOfExercise>(typeOfExerciseId);

                return exerciseType;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(typeOfExerciseId)
                                                                                          , typeOfExerciseId)
                                                            , nameof(ExerciseEquipment)
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<ExerciseType> GetExerciseTypes(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<ExerciseType>();
        }

        public TypeOfExercise GetType(int typeId)
        {
            try
            {
                var type = _database.GetWithChildren<TypeOfExercise>(typeId);

                return type;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(typeId)
                                                                                          , typeId)
                                                            , nameof(ExerciseEquipment)
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<TypeOfExercise> GetTypes(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<TypeOfExercise>();
        }

        public ExerciseEquipment GetExerciseEquipment(int exerciseEquipmentId)
        {
            try
            {
                var equipment = _database.GetWithChildren<ExerciseEquipment>(exerciseEquipmentId);

                return equipment;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(exerciseEquipmentId)
                                                                                          , exerciseEquipmentId)
                                                            , nameof(ExerciseEquipment)
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<ExerciseEquipment> GetExerciseEquipments(bool forceRefresh = false)
        {
            return _database.GetAllWithChildren<ExerciseEquipment>();
        }

        public Equipment GetEquipment(int equipmentId)
        {
            try
            {
                var equipment = _database.GetWithChildren<Equipment>(equipmentId);

                return equipment;
            }
            catch (InvalidOperationException operationException)
            {
                throw new SequenceContainsNoElementsException(GetNoElementsExceptionMessage(nameof(equipmentId)
                                                                                          , equipmentId)
                                                            , nameof(Equipment)
                                                            , operationException);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }
        }

        public IEnumerable<Equipment> GetAllEquipment(bool forceRefresh = false)
        {
            return _database.Table<Equipment>()
                            .ToList();
        }
    }

    public partial class Database //Deletes
    {
        public int DeleteClient(ref Client client)
        {
            var numberDeleted = _database.Delete(client);
            client = null;

            return numberDeleted;
        }

        /// <summary>
        /// Generic Delete TODO: Need to test to ensure it works as intended.
        /// </summary>
        /// <param name="record">An instance of the type (T) that is being deleted</param>
        /// <typeparam name="T">The type of the instance being deleted</typeparam>
        /// <returns>The number of records affected</returns>
        public int Delete<T>(ref T record)
        {
            var numberDeleted = _database.Delete<T>(record);
            record = default(T);

            return numberDeleted;
        }


        public int DeleteWorkout(ref Workout workout)
        {
            var numberDeleted = _database.Delete(workout);
            workout = null;

            return numberDeleted;
        }

        public int DeleteSession(ref Session session)
        {
            var numberAffected = _database.Delete(session);
            session = null;

            return numberAffected;
        }
        public int DeleteWorkoutExercises(ref WorkoutExercise workoutExercise)
        {
            var workoutExerciseId = _database.Delete(workoutExercise);
            workoutExercise = null;

            return workoutExerciseId;
        }

        public int DeleteLinkedWorkoutsToExercises(ref LinkedWorkoutsToExercises linkedWorkoutsToExercises)
        {
            var linkedWorkoutsToExercisesId = _database.Delete(linkedWorkoutsToExercises);
            linkedWorkoutsToExercises = null;

            return linkedWorkoutsToExercisesId;
        }

        public int DeleteExercise(ref Exercise exercise)
        {
            var exerciseId = _database.Delete(exercise);
            exercise = null;

            return exerciseId;
        }

        public int DeleteExerciseMuscleGroup(ref ExerciseMuscleGroup exerciseMuscleGroup)
        {
            var exerciseMuscleGroupId = _database.Delete(exerciseMuscleGroup);
            exerciseMuscleGroup = null;

            return exerciseMuscleGroupId;
        }

        public int DeleteExerciseType(ref ExerciseType exerciseType)
        {
            var exerciseTypeId = _database.Delete(exerciseType);
            exerciseType = null;

            return exerciseTypeId;
        }

        public int DeleteType(ref TypeOfExercise exerciseType)
        {
            var typeId = _database.Delete(exerciseType);
            exerciseType = null;

            return typeId;
        }

        public int DeleteExerciseEquipment(ref ExerciseEquipment exerciseEquipment)
        {
            var exerciseEquipmentId = _database.Delete(exerciseEquipment);
            exerciseEquipment = null;

            return exerciseEquipmentId;
        }

        public int DeleteEquipment(ref Equipment equipment)
        {
            var equipmentId = _database.Delete(equipment);
            equipment = null;

            return equipmentId;
        }
    }

    public partial class Database //Updates
    {
        public int UpdateMeasurable(Measurable measurable) => throw new NotImplementedException();

        public int DeleteAddress(ref Address address) => throw new NotImplementedException();

        public int DeletePhoneNumber(ref PhoneNumber phoneNumber) => throw new NotImplementedException();

        public int DeleteMeasurable(ref Measurable measurable) => throw new NotImplementedException();

        public int UpdateAddress(Address address)
        {
            return _database.Update(address);
        }

        public int UpdatePhoneNumber(PhoneNumber phoneNumber)
        {
            return _database.Update(phoneNumber);
        }

        public void UpdateSession(Session session)
        {
            _database.UpdateWithChildren(session);
        }

        public void UpdateClient(Client client)
        {
            _database.UpdateWithChildren(client);
        }

        public void UpdateWorkout(Workout workout)
        {
            _database.UpdateWithChildren(workout);
        }

        public void UpdateSion(Session session)
        {
            _database.UpdateWithChildren(session);
        }

        public void UpdateWorkoutExercises(WorkoutExercise workoutExercise)
        {
            _database.UpdateWithChildren(workoutExercise);
        }

        public void UpdateLinkedWorkoutsToExercises(LinkedWorkoutsToExercises linkedWorkoutsToExercises)
        {
            _database.Update(linkedWorkoutsToExercises);
        }

        public void UpdateExercise(Exercise exercise)
        {
            _database.UpdateWithChildren(exercise);
        }

        public void UpdateMuscleGroups(MuscleGroup muscleGroup)
        {
            _database.UpdateWithChildren(muscleGroup);
            _database.Commit();
        }

        public void UpdateType(TypeOfExercise exerciseType)
        {
            _database.UpdateWithChildren(exerciseType);
        }

        public void UpdateExerciseEquipment(ExerciseEquipment exerciseEquipment)
        {
            _database.UpdateWithChildren(exerciseEquipment);
        }

        public int UpdateEquipment(Equipment equipment)
        {
            return _database.Insert(equipment);
        }

        public int SaveExercise(Exercise exercise)
        {
            if (exercise.Id == 0)
            {
                int insertedExerciseId = _database.Insert(exercise);

                return insertedExerciseId;
            }

            int updatedExerciseId = _database.Update(exercise);

            return updatedExerciseId;
        }

        //https://matetiblog.wordpress.com/2018/06/17/xamarin-5-sqlite-1-to-many/
        public void SaveWorkout(Workout workout)
        {
            //BENDO:  Refactor to reduce nesting, and make it do one thing

            //Is this a new workout?
            if (workout.Id == 0)
            {
                _database.Insert(workout);

                return;
            }

            //This is an existing workout.  Are there exercises associated with it?
            if (workout.Exercises.Count > 0)
            {
                _database.UpdateWithChildren(workout);

                return;
            }

            //Update existing workout that has no associated exercises
            _database.Update(workout);
        }
    }

    public partial class Database //Adds
    {
        public int AddAddress(Address address) => throw new NotImplementedException();

        public int AddPhoneNumber(PhoneNumber phoneNumber) => throw new NotImplementedException();

        public int AddMeasurable(Measurable measurable) => throw new NotImplementedException();

        public int AddContact(AppContact contact)
        {
            _database.InsertWithChildren(contact);

            return contact.Id;
        }

        public void AddWorkoutWithExercises(Workout workout)
        {
            _database.InsertWithChildren(workout);
        }

        public int AddLinkedWorkoutExercise(LinkedWorkoutsToExercises linkedWorkoutsExercises)
        {
            var linkedWorkoutExercisesInWorkout = GetLinkedWorkoutsToExercisesByWorkouts(linkedWorkoutsExercises.WorkoutId)
            .ToList();

            var maxOrderBy = -1;

            if (linkedWorkoutExercisesInWorkout.Any())
            {
                maxOrderBy = linkedWorkoutExercisesInWorkout.Max(field => field.OrderBy);
            }

            linkedWorkoutsExercises.OrderBy = maxOrderBy + 1;
            _database.Insert(linkedWorkoutsExercises);

            return linkedWorkoutsExercises.Id;
        }

        public int AddWorkoutExercise(WorkoutExercise workoutExercise)
        {
            var workoutExercisesInWorkout = GetWorkoutExercisesByWorkout(workoutExercise.WorkoutId)
            .ToList();

            var maxOrderBy = -1;

            if (workoutExercisesInWorkout.Any())
            {
                maxOrderBy = workoutExercisesInWorkout.Max(field => field.OrderBy);
            }

            workoutExercise.OrderBy = maxOrderBy + 1;

            _database.Insert(workoutExercise);

            return workoutExercise.Id;
        }

        /// <summary>
        /// Adds just one Workout without any children.
        /// Use case:  Add new workout -> SaveWorkoutsToExercise -> Add exercise
        /// </summary>
        /// <param name="workout"></param>
        public int AddJustOneWorkout(Workout workout)
        {
            return _database.Insert(workout) == 1
            ? workout.Id
            : 0; //Nothing was inserted
        }

        public int AddJustOneSession(Session session)
        {
            return _database.Insert(session) == 1
            ? session.Id
            : 0;
        }

        public int AddJustOneClient(Client client)
        {
            return _database.Insert(client) == 1
            ? client.ClientId
            : 0;
        }

        public void AddJustOneClientWithChildren(Client client)
        {
            _database.InsertWithChildren(client);
        }

        public void AddExercise(Exercise exercise)
        {
            _database.InsertWithChildren(exercise);
        }

        public int AddJustOneExercise(Exercise exercise)
        {
            return _database.Insert(exercise) == 1 ?
            exercise.Id :
            0; //Nothing was inserted
        }

        public void AddSynergist(Synergist synergist)
        {
            _database.InsertWithChildren(synergist);
        }

        public int AddJustOneMuscleGroup(MuscleGroup muscleGroup)
        {
            return _database.Insert(muscleGroup) == 1 ?
            muscleGroup.Id :
            0;
        }

        public void AddExerciseType(ExerciseType exerciseType)
        {
            _database.InsertWithChildren(exerciseType);
        }

        public int AddJustOneTypeOfExercise(TypeOfExercise typeOfExercise)
        {
            return _database.Insert(typeOfExercise) == 1
            ? typeOfExercise.Id
            : 0;
        }

        public int AddType(TypeOfExercise exerciseType)
        {
            return _database.Insert(exerciseType) == 1 ?
            exerciseType.Id :
            0;
        }

        public void AddExerciseEquipment(ExerciseEquipment exerciseEquipment)
        {
            _database.InsertWithChildren(exerciseEquipment);
        }

        public int AddEquipment(Equipment equipment)
        {
            return _database.Insert(equipment) == 1 ?
            equipment.Id :
            0;
        }

        //BENDO: The method above (AddEquipment) is the same as the one below (AddJustOneEquipment)
        //        But both have references.  Remove one and referencing code to the one that is left
        public int AddJustOneEquipment(Equipment equipment)
        {
            return _database.Insert(equipment) == 1 ?
            equipment.Id :
            0;
        }

        public void AddClient(Client client
                            , List<Address> addresses = null
                            , List<PhoneNumber> phoneNumbers = null)
        {
            _database.Insert(client);

            client.Addresses = addresses ?? new List<Address>();
            client.PhoneNumbers = phoneNumbers ?? new List<PhoneNumber>();

            UpdateClient(client);
        }

        public void AddSession(Session session
                             , Client client
                             , List<Workout> workouts = null)
        {
            _database.Insert(session);

            session.Client = client;
            session.Workouts = workouts ?? new List<Workout>();

            UpdateSession(session);
        }

        public void AddPhoneNumber(PhoneNumber phoneNumber
                                 , Client client)
        {
            _database.Insert(phoneNumber);

            client.PhoneNumbers.Add(phoneNumber);

            UpdateClient(client);
        }

        public void AddAddress(Address address
                             , Client client)
        {
            _database.Insert(address);

            client.Addresses.Add(address);

            UpdateClient(client);
        }

    }

}
