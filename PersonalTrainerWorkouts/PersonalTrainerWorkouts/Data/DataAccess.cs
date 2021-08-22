using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.Data
{
    public class DataAccess
    {
        //BENDO: Replace all calls to Database to calls ( not necessarily in Tests ) to calls to this class 

        private IDataStore Database { get; set; }

        public DataAccess(IDataStore database)
        {
            Database = database;
        }
        
    #region Adds

        public int AddNewWorkout(Workout workout)
        {
            var allWorkouts = Database.GetWorkouts();

            ValidateForNoDuplicatedNames(workout.Name
                                       , allWorkouts
                                       , nameof(Workout));

            return Database.AddJustOneWorkout(workout);
        }

        public int AddNewTypeOfExercise(TypeOfExercise typeOfExercise)
        {
            var allTypesOfExercises = Database.GetTypes();

            ValidateForNoDuplicatedNames(typeOfExercise.Name
                                       , allTypesOfExercises
                                       , nameof(TypeOfExercise));

            return Database.AddJustOneTypeOfExercise(typeOfExercise);
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
            var allMuscleGroups = Database.GetMuscleGroups();

            ValidateForNoDuplicatedNames(muscleGroup.Name
                                       , allMuscleGroups
                                       , nameof(MuscleGroup));

            return Database.AddJustOneMuscleGroup(muscleGroup);
        }

        public int AddNewOpposingMuscleGroupRelationship(int muscleGroupId
                                                       , int opposingMuscleGroupId)
        {
            return Database.AddOpposingMuscleGroup(muscleGroupId
                                                 , opposingMuscleGroupId);
        }

        public int AddNewOpposingMuscleGroupRelationship(MuscleGroup muscleGroup
                                                       , MuscleGroup opposingMuscleGroup)
        {
            
            var muscleGroupId         = AddNewMuscleGroup(muscleGroup);
            var opposingMuscleGroupId = AddNewMuscleGroup(opposingMuscleGroup);

            return AddNewOpposingMuscleGroupRelationship(muscleGroupId
                                                       , opposingMuscleGroupId);

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

    #endregion

    #region Gets

        public Workout GetWorkout(int workoutId)
        {
            return Database.GetWorkout(workoutId);
        }
        
        public IEnumerable<WorkoutExercise> GetWorkoutExercises(int workoutId)
        {
            return Database.GetWorkoutExercisesByWorkout(workoutId).OrderBy(field=>field.OrderBy);
        }

        public IEnumerable<LinkedWorkoutsToExercises> GetLinkedWorkoutsToExercises(int workoutId)
        {
            return Database.GetAllLinkedWorkoutsToExercises(workoutId);
        }

        public Exercise GetExercise(int exerciseId)
        {
            return Database.GetExercise(exerciseId);
        }

    #endregion

    #region Updates
        
        public void UpdateWorkout(Workout workout)
        {
            Database.UpdateWorkout(workout);
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

    #endregion
        
    #region Helper methods
        
        public static void ValidateForNoDuplicatedNames(string                 potentialDuplicatedName
                                                       , IEnumerable<BaseModel> listOfModels
                                                       , string                 type)
        {
            var duplicatedWorkout = listOfModels.FirstOrDefault(field => field.Name == potentialDuplicatedName);

            if (duplicatedWorkout != null)
            {
                throw new ApplicationExceptions.AttemptToAddDuplicateEntityException(type
                                                                                   , duplicatedWorkout
                                                                                   , nameof(potentialDuplicatedName));
            }
        }

    #endregion

    }
}
