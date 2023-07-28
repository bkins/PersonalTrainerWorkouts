using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.Models.Intermediates;

using System.Collections.Generic;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;
using TypeOfExercise = PersonalTrainerWorkouts.Models.TypeOfExercise;

namespace PersonalTrainerWorkouts.Data.Interfaces
{
    public interface IDataStore
    {
        string DbPath();

        /// <summary>
        /// Will just add the workout passed in, not any children.
        /// </summary>
        /// <param name="workout"></param>
        int AddJustOneWorkout(Workout workout);
        void AddSessionWithChildren(Session session);
        void AddJustSession(Session session);

        void AddSession(Session       session
                      , Client        client
                      , List<Workout> workout = null);

        int AddJustOneClient(Client client);
        void AddJustOneClientWithChildren(Client client);
        void AddExercise(Exercise exercise);
        int AddJustOneExercise(Exercise exercise);
        void AddSynergist(Synergist synergist);
        int AddJustOneMuscleGroup(MuscleGroup muscleGroup);
        void AddExerciseType(ExerciseType exerciseType);
        int AddJustOneTypeOfExercise(TypeOfExercise typeOfExercise);
        int AddType(TypeOfExercise exerciseType);
        void AddExerciseEquipment(ExerciseEquipment exerciseEquipment);
        int AddEquipment(Equipment equipment);
        int AddJustOneEquipment(Equipment equipment);
        int AddWorkoutExercise(WorkoutExercise workoutExercise);
        int AddLinkedWorkoutExercise(LinkedWorkoutsToExercises linkedWorkoutsExercises);
        int AddContact(AppContact contact);

        //Updates
        void UpdateWorkout(Workout workout);
        void UpdateSession(Session session);
        void UpdateClient(Client client);
        void UpdateWorkoutExercises(WorkoutExercise workoutExercise);
        void UpdateLinkedWorkoutsToExercises(LinkedWorkoutsToExercises linkedWorkoutsToExercises);
        void UpdateExercise(Exercise exercise);
        void UpdateMuscleGroups(MuscleGroup muscleGroup);
        void UpdateType(TypeOfExercise exerciseType);
        void UpdateExerciseEquipment(ExerciseEquipment exerciseEquipment);
        int UpdateEquipment(Equipment equipment);

        //Deletes
        int DeleteWorkout(ref Workout workout);
        int DeleteSession(ref Session session);
        int DeleteClient(ref Client client);
        int DeleteWorkoutExercises(ref WorkoutExercise workoutExercise);
        int DeleteLinkedWorkoutsToExercises(ref LinkedWorkoutsToExercises linkedWorkoutsToExercises);
        int DeleteExercise(ref Exercise exercise);
        int DeleteExerciseMuscleGroup(ref ExerciseMuscleGroup exerciseMuscleGroup);
        int DeleteExerciseType(ref ExerciseType exerciseType);
        int DeleteType(ref TypeOfExercise exerciseType);
        int DeleteExerciseEquipment(ref ExerciseEquipment exerciseEquipment);
        int DeleteEquipment(ref Equipment equipment);

        //Gets
        Workout GetWorkout(int workoutId);

        IEnumerable<Workout> GetWorkouts(bool forceRefresh = false);

        IEnumerable<Session> GetSessions(bool forceRefresh = false);

        IEnumerable<Client> GetClients(bool         forceRefresh = false);
        IEnumerable<Goal> GetGoals(bool             forceRefresh = false);
        IEnumerable<AppContact> GetAppContacts(bool forceRefresh = false);

        LinkedWorkoutsToExercises GetLinkedWorkoutsToExercise(int linkedWorkoutsToExercisesId);

        IEnumerable<LinkedWorkoutsToExercises> GetAllLinkedWorkoutsToExercises(bool forceRefresh = false);

        IEnumerable<LinkedWorkoutsToExercises> GetAllLinkedWorkoutsToExercises(int workoutId);

        IEnumerable<LinkedWorkoutsToExercises> GetAllLinkedWorkoutsToExercises(int workoutId
                                                                             , int exerciseId);

        WorkoutExercise GetWorkoutExercise(int workoutExerciseId);

        IEnumerable<WorkoutExercise> GetWorkoutExercises(bool forceRefresh = false);

        IEnumerable<WorkoutExercise> GetWorkoutExercises(int workoutId);

        IEnumerable<WorkoutExercise> GetWorkoutExercises(int workoutId
                                                       , int exerciseId);

        IEnumerable<WorkoutExercise> GetWorkoutExercisesByWorkout(int workoutId);

        IEnumerable<WorkoutExercise> GetWorkoutExercisesByExercise(int exerciseId);

        Exercise GetExercise(int exerciseId);

        IEnumerable<Exercise> GetExercises(bool forceRefresh = false);

        IEnumerable<ExerciseMuscleGroup> GetExerciseMuscleGroups(bool forceRefresh = false);

        IEnumerable<MuscleGroup> GetMuscleGroups(bool forceRefresh = false);

        MuscleGroup GetOpposingMuscleGroupByMuscleGroup(int muscleGroupId);
        IEnumerable<Synergist> GetSynergists(bool forceRefresh = false);
        TypeOfExercise GetTypeOfExercise(int typeOfExerciseId);
        IEnumerable<ExerciseType> GetExerciseTypes(bool forceRefresh = false);
        TypeOfExercise GetType(int typeId);
        IEnumerable<TypeOfExercise> GetTypes(bool forceRefresh = false);
        ExerciseEquipment GetExerciseEquipment(int exerciseEquipmentId);
        IEnumerable<ExerciseEquipment> GetExerciseEquipments(bool forceRefresh = false);
        Equipment GetEquipment(int equipmentId);
        IEnumerable<Equipment> GetAllEquipment(bool forceRefresh = false);

        IEnumerable<Address> GetAllAddresses(bool forceRefresh = false);
        Address GetAddress(int addressId);
        int DeleteAddress(ref Address address);
        int UpdateAddress(Address address);
        int AddAddress(Address address);

        IEnumerable<PhoneNumber> GetAllPhoneNumbers(bool forceRefresh = false);
        Address GetPhoneNumber(int phoneNumberId);
        int DeletePhoneNumber(ref PhoneNumber phoneNumber);
        int UpdatePhoneNumber(PhoneNumber phoneNumber);
        int AddPhoneNumber(PhoneNumber phoneNumber);

        IEnumerable<Measurable> GetAllMeasurables(bool forceRefresh = false);
        Measurable GetMeasurable(int measurableId);
        int DeleteMeasurable(ref Measurable measurable);
        int UpdateMeasurable(Measurable measurable);
        int AddMeasurable(Measurable measurable);

        int SaveExercise(Exercise exercise);
        void SaveWorkout(Workout workout);

        void CreateTables();
        void DropTables();
        string GetFilePath();
        string GetFileName();

        List<string> GetTables();
        void DropClientTables();
        void CreateClientTables();
        void UpdateGoal(Goal                goal);
        int AddJustOneGoal(Goal             goal);
        Goal GetGoal(int                    goalId);
        int AddJustOneMeasurable(Measurable measurable);
        void InsertConfigurationValues();
        IEnumerable<UnitOfMeasurement> GetUnitOfMeasurements();
        int DeleteGoal(ref Goal goal);
    }
}
