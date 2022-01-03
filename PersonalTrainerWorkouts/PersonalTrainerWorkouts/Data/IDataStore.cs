using System.Collections.Generic;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using TypeOfExercise = PersonalTrainerWorkouts.Models.TypeOfExercise;

namespace PersonalTrainerWorkouts.Data
{
    public interface IDataStore
    {
        string DbPath();

        //Adds

        /// <summary>
        /// Will just add the workout passed in, not any children.
        /// </summary>
        /// <param name="workout"></param>
        int AddJustOneWorkout(Workout workout);

        void AddExercise(Exercise                               exercise);
        int  AddJustOneExercise(Exercise                        exercise);
        void AddSynergist(Synergist                             synergist);
        int  AddJustOneMuscleGroup(MuscleGroup                  muscleGroup);
        void AddExerciseType(ExerciseType                       exerciseType);
        int  AddJustOneTypeOfExercise(TypeOfExercise            typeOfExercise);
        int  AddType(TypeOfExercise                             exerciseType);
        void AddExerciseEquipment(ExerciseEquipment             exerciseEquipment);
        int  AddEquipment(Equipment                             equipment);
        int  AddJustOneEquipment(Equipment                      equipment);
        void AddExerciseMuscleGroup(ExerciseMuscleGroup         exerciseMuscleGroup);
        int  AddWorkoutExercise(WorkoutExercise                 workoutExercise);
        int  AddLinkedWorkoutExercise(LinkedWorkoutsToExercises linkedWorkoutsExercises);

        //Updates
        void UpdateWorkout(Workout                                     workout);
        void UpdateWorkoutExercises(WorkoutExercise                    workoutExercise);
        void UpdateLinkedWorkoutsToExercises(LinkedWorkoutsToExercises linkedWorkoutsToExercises);
        void UpdateExercise(Exercise                                   exercise);
        void UpdateExerciseMuscleGroup(ExerciseMuscleGroup             exerciseMuscleGroup);
        void UpdateMuscleGroups(MuscleGroup                            muscleGroup);
        int  UpdateOpposingMuscleGroup(OpposingMuscleGroup             opposingMuscleGroup);
        void UpdateExerciseType(ExerciseType                           exerciseType);
        int  UpdateType(TypeOfExercise                                 exerciseType);
        void UpdateExerciseEquipment(ExerciseEquipment                 exerciseEquipment);
        int  UpdateEquipment(Equipment                                 equipment);

        //Deletes
        int DeleteWorkout(ref                   Workout                   workout);
        int DeleteWorkoutExercises(ref          WorkoutExercise           workoutExercise);
        int DeleteLinkedWorkoutsToExercises(ref LinkedWorkoutsToExercises linkedWorkoutsToExercises);
        int DeleteExercise(ref                  Exercise                  exercise);
        int DeleteExerciseMuscleGroup(ref       ExerciseMuscleGroup       exerciseMuscleGroup);
        int DeleteMuscleGroups(ref              MuscleGroup               muscleGroup);
        int DeleteOpposingMuscleGroup(ref       OpposingMuscleGroup       opposingMuscleGroup);
        int DeleteExerciseType(ref              ExerciseType              exerciseType);
        int DeleteType(ref                      TypeOfExercise            exerciseType);
        int DeleteExerciseEquipment(ref         ExerciseEquipment         exerciseEquipment);
        int DeleteEquipment(ref                 Equipment                 equipment);

        //Gets
        Workout                                GetWorkout(int                       workoutId);
        IEnumerable<Workout>                   GetWorkouts(bool                     forceRefresh = false);
        LinkedWorkoutsToExercises              GetLinkedWorkoutsToExercise(int      linkedWorkoutsToExercisesId);
        IEnumerable<LinkedWorkoutsToExercises> GetAllLinkedWorkoutsToExercises(bool forceRefresh = false);
        IEnumerable<LinkedWorkoutsToExercises> GetAllLinkedWorkoutsToExercises(int  workoutId);

        IEnumerable<LinkedWorkoutsToExercises> GetAllLinkedWorkoutsToExercises(int workoutId
                                                                             , int exerciseId);

        WorkoutExercise              GetWorkoutExercise(int   workoutExerciseId);
        IEnumerable<WorkoutExercise> GetWorkoutExercises(bool forceRefresh = false);
        IEnumerable<WorkoutExercise> GetWorkoutExercises(int  workoutId);

        IEnumerable<WorkoutExercise> GetWorkoutExercises(int workoutId
                                                       , int exerciseId);

        IEnumerable<WorkoutExercise>     GetWorkoutExercisesByWorkout(int        workoutId);
        IEnumerable<WorkoutExercise>     GetWorkoutExercisesByExercise(int       exerciseId);
        Exercise                         GetExercise(int                         exerciseId);
        IEnumerable<Exercise>            GetExercises(bool                       forceRefresh = false);
        ExerciseMuscleGroup              GetExerciseMuscleGroup(int              exerciseMuscleGroupId);
        IEnumerable<ExerciseMuscleGroup> GetExerciseMuscleGroups(bool            forceRefresh = false);
        IEnumerable<ExerciseMuscleGroup> GetExerciseMuscleGroupsByExercise(int   exerciseId);
        MuscleGroup                      GetMuscleGroup(int                      muscleGroupId);
        IEnumerable<MuscleGroup>         GetMuscleGroups(bool                    forceRefresh = false);
        OpposingMuscleGroup              GetOpposingMuscleGroup(int              opposingMuscleGroupId);
        MuscleGroup                      GetOpposingMuscleGroupByMuscleGroup(int muscleGroupId);
        IEnumerable<Synergist>           GetSynergists(bool                      forceRefresh = false);
        IEnumerable<OpposingMuscleGroup> GetOpposingMuscleGroups(bool            forceRefresh = false);
        TypeOfExercise                   GetTypeOfExercise(int                   typeOfExerciseId);
        IEnumerable<ExerciseType>        GetExerciseTypes(bool                   forceRefresh = false);
        TypeOfExercise                   GetType(int                             typeId);
        IEnumerable<TypeOfExercise>      GetTypes(bool                           forceRefresh = false);
        ExerciseEquipment                GetExerciseEquipment(int                exerciseEquipmentId);
        IEnumerable<ExerciseEquipment>   GetExerciseEquipments(bool              forceRefresh = false);
        Equipment                        GetEquipment(int                        equipmentId);
        IEnumerable<Equipment>           GetAllEquipment(bool                    forceRefresh = false);

        int  SaveExercise(Exercise exercise);
        void SaveWorkout(Workout   workout);

        void   CreateTables();
        void   DropTables();
        string GetFilePath();
        string GetFileName();
    }
}
