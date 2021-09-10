using System.Collections.Generic;
using System.Linq;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.Data
{
    public class DataAccess
    {
        private IDataStore Database { get; set; }

        public DataAccess(IDataStore database)
        {
            Database = database;
        }
        
        public void CreateTables()
        {
            Database.CreateTables();
        }

        public void DropTables()
        {
            Database.DropTables();
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
            bool exerciseHasAnyEquipment    = workout.Exercises.Any(field => ! field.Equipment.Any());
            bool exerciseHasAnyTypes        = workout.Exercises.Any(field => ! field.TypesOfExercise.Any());

            if (! exerciseHasAnySynergists
             && ! exerciseHasAnyEquipment
             && ! exerciseHasAnyTypes)
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
        
        public void AddExerciseType(int exerciseId
                                  , int typeOfExerciseId)
        {
            var existingExerciseTypes = Database.GetExerciseTypes()
                                                .Where(field => field.ExerciseId == exerciseId 
                                                             && field.TypeId == typeOfExerciseId);

            if (existingExerciseTypes.Any())
            {
                throw new ApplicationExceptions.EntityRelationAlreadyExistsException("You cannot add an Exercise Type that is already associated with this Exercise.\r\nPlease select different type.");
            }

            Database.AddExerciseType(new ExerciseType
                                     {
                                         ExerciseId = exerciseId
                                       , TypeId     = typeOfExerciseId
                                     });
        }
        
        public void AddExerciseEquipment(int exerciseId
                                       , int equipmentId)
        {
            var existingExerciseEquipment = Database.GetExerciseEquipments()
                                                    .Where(field => field.ExerciseId  == exerciseId 
                                                                 && field.EquipmentId == equipmentId);

            if (existingExerciseEquipment.Any())
            {
                throw new ApplicationExceptions.EntityRelationAlreadyExistsException("You cannot add Equipment that is already associated with this Exercise.\r\nPlease select different equipment.");
            }

            Database.AddExerciseEquipment(new ExerciseEquipment
                                          {
                                              ExerciseId  = exerciseId
                                            , EquipmentId = equipmentId
                                          });
        }
        
        public void AddExerciseMuscleGroup(int exerciseId
                                         , int muscleGroupId)
        {
            var existingExerciseMuscleGroup = Database.GetExerciseMuscleGroups()
                                                      .Where(field => field.ExerciseId    == exerciseId 
                                                                   && field.MuscleGroupId == muscleGroupId);

            if (existingExerciseMuscleGroup.Any())
            {
                throw new ApplicationExceptions.EntityRelationAlreadyExistsException("You cannot add Muscle Group that is already associated with this Exercise.\r\nPlease select different muscle group.");
            }

            Database.AddExerciseMuscleGroup(new ExerciseMuscleGroup
                                            {
                                                ExerciseId    = exerciseId
                                              , MuscleGroupId = muscleGroupId
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
                throw new ApplicationExceptions.EntityRelationAlreadyExistsException("You cannot add this Synergist.\r\nIt already exists in this Exercise\r\nPlease select/Add a different Synergist.");
            }

            Database.AddSynergist(new Synergist
                                  {
                                      ExerciseId            = newSynergist.ExerciseId
                                    , MuscleGroupId         = newSynergist.MuscleGroupId
                                    , OpposingMuscleGroupId = newSynergist.OpposingMuscleGroupId
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

    #endregion

    #region Gets

        public Workout GetWorkout(int workoutId)
        {
            return Database.GetWorkout(workoutId) ?? new Workout();
        }
        
        public IEnumerable<Workout> GetWorkouts()
        {
            return Database.GetWorkouts() ?? new List<Workout>();
        }

        public IEnumerable<WorkoutExercise> GetWorkoutExercises(int workoutId)
        {
            return Database.GetWorkoutExercisesByWorkout(workoutId)
                           .OrderBy(field=>field.OrderBy);
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
            return Database.GetExercise(exerciseId) ?? new Exercise();
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
        public IEnumerable<ExerciseMuscleGroup> GetAllExerciseMuscleGroups()
        {
            return Database.GetExerciseMuscleGroups() ?? new List<ExerciseMuscleGroup>();
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

    #region Deletes

        public void DeleteExerciseType(int exerciseId
                                     , int typeOfExerciseId)
        {
            var typeOfExerciseToDelete = Database.GetExerciseTypes()
                                                 .First(field => field.ExerciseId == exerciseId 
                                                              && field.TypeId == typeOfExerciseId);

            Database.DeleteExerciseType(ref typeOfExerciseToDelete);
        }
        
        public void DeleteExerciseEquipment(int exerciseId
                                          , int equipmentId)
        {
            var equipmentToDelete = Database.GetExerciseEquipments()
                                            .First(field => field.ExerciseId == exerciseId 
                                                         && field.EquipmentId == equipmentId);

            Database.DeleteExerciseEquipment(ref equipmentToDelete);
        }

        public void DeleteExerciseMuscleGroup(int exerciseId
                                            , int muscleGroupId)
        {
            var muscleGroupToDelete = Database.GetExerciseMuscleGroups()
                                              .First(field => field.ExerciseId == exerciseId 
                                                           && field.MuscleGroupId == muscleGroupId);

            Database.DeleteExerciseMuscleGroup(ref muscleGroupToDelete);
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
