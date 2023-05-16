using Avails.D_Flat.Exceptions;
using Avails.Xamarin.Logger;

using PersonalTrainerWorkouts.Data.Interfaces;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.Models.Intermediates;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalTrainerWorkouts.Data
{
    public partial class DataAccess
    {
        private IDataStore         Database          { get; set; }
        private IContactsDataStore ContactsDataStore { get; set; }
        public  string             DatabaseLocation  => GetDatabaseLocation();
        public  string             DatabaseFileName  => GetDatabaseFileName();

        public DataAccess(IDataStore database, IContactsDataStore contactsDataStore)
        {
            Database          = database;
            ContactsDataStore = contactsDataStore;
        }

        private string GetDatabaseLocation()
        {
            return Database.GetFilePath();
        }

        private string GetDatabaseFileName()
        {
            return Database.GetFileName();
        }

        public void CreateTables()
        {
            Database.CreateTables();
        }

        public void DropTables()
        {
            Database.DropTables();
        }

        public void CreateContactTables()
        {
            Database.CreateContactTables();
        }

        public void DropContactTables()
        {
            Database.DropContactTables();
        }

        public string GetDatabasePath()
        {
            return Database.DbPath();
        }

        //NotImplemented: This method is INCOMPLETE.
        //This method is to complete the Gets in the (Database class) that take the param of 'forceRefresh' that return a Workout
        //BENDO: Instead of having this method rebuild all object within the Workout, have a 'Refresh" method for each object that can be in
        //       a workout (e.g. RefreshExercise, RefreshEquipment, RefreshMuscleGroup, etc.).
        public Workout RefreshWorkoutData(Workout workout)
        {
            //Workout is empty
            if (workout == null)
            {
                return null;
            }

            //Workout does not have any exercises, just refresh the workout object
            if (workout.Exercises == null)
            {
                return GetWorkout(workout.Id);
            }

            var workoutsToExercises = GetLinkedWorkoutsToExercises(workout.Id);

            foreach (var workoutsToExercise in workoutsToExercises)
            {
                workout.Exercises.Add(GetExercise(workoutsToExercise.ExerciseId));
            }

            bool exerciseHasAnySynergists = workout.Exercises.Any(field => field.Synergists.Any());
            bool exerciseHasAnyEquipment = workout.Exercises.Any(field => !field.Equipment.Any());
            bool exerciseHasAnyTypes = workout.Exercises.Any(field => !field.TypesOfExercise.Any());

            if (!exerciseHasAnySynergists
             && !exerciseHasAnyEquipment
             && !exerciseHasAnyTypes)
            {
                //Exercise has no children, nothing left to refresh
                return workout;
            }

            if (exerciseHasAnySynergists)
            {
                var listOfExercisesWithMuscleGroups = new List<Exercise>();

                foreach (var exercise in workout.Exercises)
                {
                    var opposingMuscleGroup = GetOpposingMuscleGroupByMuscleGroup(exercise.Id);
                }
            }

            return null;
        }

    }

    public partial class DataAccess //Helper methods
    {
        public static bool ValidateForNoDuplicatedNames(string potentialDuplicatedName
                                                      , IEnumerable<BaseModel> listOfModels
                                                      , string type)
        {
            var duplicatedWorkout = listOfModels.FirstOrDefault(field => field.Name == potentialDuplicatedName);

            if (duplicatedWorkout != null)
            {
                throw new AttemptToAddDuplicateEntityException(type
                                                             , duplicatedWorkout
                                                             , nameof(potentialDuplicatedName));
            }
            return true;

        }

        public List<string> GetTables()
        {
            return Database.GetTables();
        }
    }

    public partial class DataAccess //Deletes
    {
        public void DeleteExerciseType(int exerciseId
                                     , int typeOfExerciseId)
        {
            var typeOfExerciseToDelete = Database.GetExerciseTypes()
                                                 .First(field => field.ExerciseId == exerciseId && field.TypeId == typeOfExerciseId);

            Database.DeleteExerciseType(ref typeOfExerciseToDelete);
        }

        public void DeleteExerciseEquipment(int exerciseId
                                          , int equipmentId)
        {
            var equipmentToDelete = Database.GetExerciseEquipments()
                                            .First(field => field.ExerciseId == exerciseId && field.EquipmentId == equipmentId);

            Database.DeleteExerciseEquipment(ref equipmentToDelete);
        }

        public void DeleteExerciseMuscleGroup(int exerciseId
                                            , int muscleGroupId)
        {
            try
            {
                var muscleGroupToDelete = Database.GetExerciseMuscleGroups()
                                                  .First(field => field.ExerciseId == exerciseId
                                                               && field.MuscleGroupId == muscleGroupId);

                Database.DeleteExerciseMuscleGroup(ref muscleGroupToDelete);

                Logger.WriteLine($"The Muscle Group '{muscleGroupId.ToString()}' was removed from the Exercise '{exerciseId.ToString()}'"
                               , Category.Information);
            }
            catch (SequenceContainsNoElementsException e)
            {
                Logger.WriteLine($"Could not find the Muscle Group with the Id of '{muscleGroupId.ToString()}'"
                               , Category.Error
                               , e);
            }
            catch (Exception exception)
            {
                Logger.WriteLine($"Something unexpected happened: {exception.Message}", Category.Error, exception);
            }
        }

        public int DeleteSession(Session session)
        {
            try
            {
                return Database.DeleteSession(ref session);
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Something unexpected happened while deleting {nameof(Session)}: {e.Message}", Category.Error, e);

                return 0;
            }
        }

        public int DeleteClient(Client client)
        {
            try
            {
                return Database.DeleteClient(ref client);
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Something unexpected happened while deleting {nameof(Client)}: {e.Message}", Category.Error, e);

                return 0;
            }
        }
    }

    public partial class DataAccess //Updates
    {
        public void UpdateWorkout(Workout workout)
        {
            Database.UpdateWorkout(workout);
        }

        public void UpdateSession(Session session)
        {
            Database.UpdateSession(session);
        }

        public void UpdateClient(Client client)
        {
            client.SetMainNumber(); //Contact data is not being saved when saving the client
            Database.UpdateClient(client);
        }

        public void UpdateGoal(Goal goal)
        {
            Database.UpdateGoal(goal);
        }

        public void UpdateExercise(Exercise exercise)
        {
            Database.UpdateExercise(exercise);
        }

        public void UpdateWorkoutExercise(WorkoutExercise workoutExercise)
        {
            Database.UpdateWorkoutExercises(workoutExercise);
        }

        public void UpdateLinkedWorkoutsToExercises(LinkedWorkoutsToExercises linkedWorkoutsToExercises)
        {
            Database.UpdateLinkedWorkoutsToExercises(linkedWorkoutsToExercises);
        }
    }

    public partial class DataAccess //Gets
    {
        public Workout GetWorkout(int workoutId)
        {
            return Database.GetWorkout(workoutId) ?? new Workout();
        }

        public IEnumerable<Workout> GetWorkouts()
        {
            return Database.GetWorkouts() ?? new List<Workout>();
        }

        public IEnumerable<Session> GetSessions()
        {
            var allSessions = Database.GetSessions().ToList();
            var allClients = GetClients().ToList();

            foreach (var session in allSessions)
            {
                session.Client = allClients.FirstOrDefault(client => client.Id == session.ClientId) ?? new Client();
            }
            return allSessions;
        }

        public IEnumerable<Client> GetClients()
        {
            var allClients = Database.GetClients().ToList();
            var allAppContacts = GetAppContacts().ToList();

            foreach (var client in allClients.Where(client => allAppContacts.Any(contact => contact.ClientId == client.Id)))
            {
                client.AppContact = allAppContacts.FirstOrDefault(contact => contact.ClientId == client.Id);
                client.SetMainNumber();

                client.Contact ??= client.AppContact?.ToContact(ContactsDataStore, client.AppContact);
            }

            return allClients;
        }

        public IEnumerable<AppContact> GetAppContacts()
        {
            return Database.GetAppContacts() ?? new List<AppContact>();
        }

        public IEnumerable<WorkoutExercise> GetWorkoutExercises(int workoutId)
        {
            return Database.GetWorkoutExercisesByWorkout(workoutId)
                           .OrderBy(field => field.OrderBy);
        }

        public IEnumerable<LinkedWorkoutsToExercises> GetLinkedWorkoutsToExercises(int workoutId)
        {
            return Database.GetAllLinkedWorkoutsToExercises(workoutId) ?? new List<LinkedWorkoutsToExercises>();
        }

        public IEnumerable<LinkedWorkoutsToExercises> GetAllLinkedWorkoutsToExercises()
        {
            return Database.GetAllLinkedWorkoutsToExercises() ?? new List<LinkedWorkoutsToExercises>();
        }

        public LinkedWorkoutsToExercises GetLinkedWorkoutsToExercise(int linkedWorkoutsToExercisesId)
        {
            return Database.GetLinkedWorkoutsToExercise(linkedWorkoutsToExercisesId) ?? new LinkedWorkoutsToExercises();
        }

        public Exercise GetExercise(int exerciseId)
        {
            return exerciseId == 0 ?
                           new Exercise() :
                           Database.GetExercise(exerciseId);
        }

        public IEnumerable<Exercise> GetExercises()
        {
            return Database.GetExercises() ?? new List<Exercise>();
        }

        public IEnumerable<ExerciseType> GetAllExerciseTypes()
        {
            return Database.GetExerciseTypes() ?? new List<ExerciseType>();
        }

        public IEnumerable<TypeOfExercise> GetAllTypesOfExercise()
        {
            return Database.GetTypes() ?? new List<TypeOfExercise>();
        }

        public IEnumerable<ExerciseEquipment> GetAllExerciseEquipment()
        {
            return Database.GetExerciseEquipments() ?? new List<ExerciseEquipment>();
        }

        public IEnumerable<Synergist> GetAllSynergists(bool forceRefresh = false)
        {
            return Database.GetSynergists() ?? new List<Synergist>();
        }

        public IEnumerable<Equipment> GetAllEquipment()
        {
            return Database.GetAllEquipment() ?? new List<Equipment>();
        }

        public IEnumerable<MuscleGroup> GetAllMuscleGroups()
        {
            return Database.GetMuscleGroups() ?? new List<MuscleGroup>();
        }

        public MuscleGroup GetOpposingMuscleGroupByMuscleGroup(int muscleGroupId)
        {
            return Database.GetOpposingMuscleGroupByMuscleGroup(muscleGroupId);
        }

        public Goal GetGoal(int goalId)
        {
            return Database.GetGoal(goalId);
        }
    }

    public partial class DataAccess //Adds
    {
        public int AddNewWorkout(Workout workout)
        {
            var allWorkouts = Database.GetWorkouts();

            ValidateForNoDuplicatedNames(workout.Name
                                       , allWorkouts
                                       , nameof(Workout));

            return Database.AddJustOneWorkout(workout);
        }

        public void AddNewSession(Session session)
        {
            Database.AddJustSession(session);
        }

        public int AddNewPhone(PhoneNumber phoneNumber)
        {
            return Database.AddPhoneNumber(phoneNumber);
        }
        
        public int AddNewClient(Client client)
        {
            return Database.AddJustOneClient(client);
        }

        public void AddNewClientWithChildren(Client client)
        {
            Database.AddJustOneClientWithChildren(client);
        }

        public int AddNewGoal(Goal goal)
        {
            return Database.AddJustOneGoal(goal);
        }
        public int AddNewTypeOfExercise(TypeOfExercise typeOfExercise)
        {
            var allTypesOfExercises = Database.GetTypes();
            var typesOfExercises = allTypesOfExercises.ToList();

            if (!typesOfExercises.Any())
                return Database.AddJustOneTypeOfExercise(typeOfExercise);

            var validNewType = ValidateForNoDuplicatedNames(typeOfExercise.Name
                                                          , typesOfExercises
                                                          , nameof(TypeOfExercise));

            if (!validNewType)
            {
                return -1;
            }

            return Database.AddJustOneTypeOfExercise(typeOfExercise);
        }

        public void AddExerciseType(int exerciseId
                                  , int typeOfExerciseId)
        {
            var existingExerciseTypes = Database.GetExerciseTypes()
                                                .Where(field => field.ExerciseId == exerciseId && field.TypeId == typeOfExerciseId);

            if (existingExerciseTypes.Any())
            {
                //BENDO:  This is being thrown when adding an existing type to a new exercise.  Though it will adds the type
                throw new EntityRelationAlreadyExistsException(
                "You cannot add an Exercise Type that is already associated with this Exercise.\r\nPlease select different type.");
            }

            Database.AddExerciseType(new ExerciseType
            {
                ExerciseId = exerciseId
                                       ,
                TypeId = typeOfExerciseId
            });
        }

        public void AddExerciseEquipment(int exerciseId
                                       , int equipmentId)
        {
            var existingExerciseEquipment = Database.GetExerciseEquipments()
                                                    .Where(field => field.ExerciseId == exerciseId && field.EquipmentId == equipmentId);

            if (existingExerciseEquipment.Any())
            {
                throw new EntityRelationAlreadyExistsException(
                "You cannot add Equipment that is already associated with this Exercise.\r\nPlease select different equipment.");
            }

            Database.AddExerciseEquipment(new ExerciseEquipment
            {
                ExerciseId = exerciseId
                                            ,
                EquipmentId = equipmentId
            });
        }

        public int AddNewExercise(Exercise exercise)
        {
            var allExercises = Database.GetExercises();

            ValidateForNoDuplicatedNames(exercise.Name
                                       , allExercises
                                       , nameof(Exercise));

            return Database.AddJustOneExercise(exercise);
        }

        public int AddNewEquipment(Equipment equipment)
        {
            var allEquipment = Database.GetAllEquipment();

            ValidateForNoDuplicatedNames(equipment.Name
                                       , allEquipment
                                       , nameof(Equipment));

            return Database.AddJustOneEquipment(equipment);
        }

        public int AddNewMuscleGroup(MuscleGroup muscleGroup)
        {
            //Muscle is already in DB at this point
            var allMuscleGroups = Database.GetMuscleGroups();

            ValidateForNoDuplicatedNames(muscleGroup.Name
                                       , allMuscleGroups
                                       , nameof(MuscleGroup));

            return Database.AddJustOneMuscleGroup(muscleGroup);
        }

        public void AddSynergist(Synergist newSynergist)
        {
            var existingSynergist = Database.GetSynergists()
                                            .Where(field => field.ExerciseId == newSynergist.ExerciseId
                                                         && field.MuscleGroupId == newSynergist.MuscleGroupId
                                                         && field.OpposingMuscleGroupId == newSynergist.OpposingMuscleGroupId);

            if (existingSynergist.Any())
            {
                throw new EntityRelationAlreadyExistsException(
                "You cannot add this Synergist.\r\nIt already exists in this Exercise\r\nPlease select/Add a different Synergist.");
            }

            Database.AddSynergist(new Synergist
            {
                ExerciseId = newSynergist.ExerciseId
                                    ,
                MuscleGroupId = newSynergist.MuscleGroupId
                                    ,
                OpposingMuscleGroupId = newSynergist.OpposingMuscleGroupId
            });
        }

        public int AddWorkoutExercise(WorkoutExercise workoutExercise)
        {
            var newWorkoutExerciseId = Database.AddWorkoutExercise(workoutExercise);

            return newWorkoutExerciseId;
        }

        public int AddLinkedWorkoutsToExercises(LinkedWorkoutsToExercises linkedWorkoutsToExercises)
        {
            var newLinkedWorkoutsToExercises = Database.AddLinkedWorkoutExercise(linkedWorkoutsToExercises);

            return newLinkedWorkoutsToExercises;
        }

        public int AddNewContact(AppContact contact)
        {
            var newContactId = Database.AddContact(contact);

            return newContactId;
        }
    }
}
