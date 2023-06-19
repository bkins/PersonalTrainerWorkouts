using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avails.D_Flat.Exceptions;
using Avails.D_Flat.Extensions;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class ExerciseAddEditViewModel : ViewModelBase
    {
        public Exercise Exercise     { get; set; }
        public Workout  Workout      { get; set; }
        public string   LengthOfTime { get; set; }
        public int?     Reps         { get; set; }

        public TypeOfExercise                       TypeOfExercise      { get; set; }
        public ObservableCollection<TypeOfExercise> TypesOfExercise     { get; set; }
        public List<TypeOfExercise>                 TypesOfExerciseList { get; set; }

        public Equipment                       Equipment     { get; set; }
        public ObservableCollection<Equipment> Equipments    { get; set; }
        public List<Equipment>                 EquipmentList { get; set; }

        //public MuscleGroup                       MuscleGroup      { get; set; }
        //public ObservableCollection<MuscleGroup> MuscleGroups     { get; set; }
        //public List<MuscleGroup>                 MuscleGroupsList { get; set; }

        public ExerciseAddEditViewModel()
        {
            Workout  = new Workout();
            Exercise = new Exercise();
        }

        public ExerciseAddEditViewModel(int workoutId
                                      , int exerciseId)
        {
            Workout  = DataAccessLayer.GetWorkout(workoutId);
            Exercise = Workout.Exercises.FirstOrDefault(e => e.Id == exerciseId) ?? new Exercise();

            LengthOfTime = Exercise?.LengthOfTime.ToTime()
                                    .ToShortForm();

            Reps = Exercise?.Reps;

            LoadTheTypesOfExercise();
            LoadTheEquipment();

            //LoadTheMuscleGroups();
        }

        private void LoadTheEquipment()
        {
            var exerciseEquipment = DataAccessLayer.GetAllExerciseEquipment()
                                                   .Where(field => field.ExerciseId == Exercise.Id);

            var allEquipment = DataAccessLayer.GetAllEquipment();

            var equipment = new ObservableCollection<Equipment>(exerciseEquipment.Select(oneEquipment => allEquipment.First(field => field.Id == oneEquipment.EquipmentId))
                                                                                 .ToList());

            Equipments    = equipment;
            EquipmentList = equipment.ToList();
        }

        //private void LoadTheMuscleGroups()
        //{
        //    //There are no ExerciseMuscleGroups?
        //    var exerciseMuscleGroup = DataAccessLayer.GetAllExerciseMuscleGroups()
        //                                             .Where(field => field.ExerciseId == Exercise.Id);

        //    var allMuscleGroups = DataAccessLayer.GetAllMuscleGroups();

        //    var muscleGroup = new ObservableCollection<MuscleGroup>
        //                          (
        //                              exerciseMuscleGroup.Select(oneMuscleGroup => allMuscleGroups.First(field => field.Id == oneMuscleGroup.MuscleGroupId))
        //                                                 .ToList()
        //                          );

        //    MuscleGroups     = muscleGroup;
        //    MuscleGroupsList = muscleGroup.ToList();
        //}

        private void LoadTheTypesOfExercise()
        {
            var exerciseTypes = DataAccessLayer.GetAllExerciseTypes()
                                               .Where(field => field.ExerciseId == Exercise.Id);

            var allTypes = DataAccessLayer.GetAllTypesOfExercise();

            var typesOfExercise = new ObservableCollection<TypeOfExercise>(exerciseTypes.Select(exerciseType => allTypes.First(field => field.Id == exerciseType.TypeId))
                                                                                        .ToList());

            TypesOfExercise     = typesOfExercise;
            TypesOfExerciseList = typesOfExercise.ToList();
        }

        public void UpdateExercise()
        {
            DataAccessLayer.UpdateExercise(Exercise);
        }

        //BENDO: The workoutId, I would presume, is known (Workout property has an instance?). If that is the case, then instead of passing in the workoutId, get it from Workout.Id
        public void SaveExercise(int workoutId)
        {
            if (string.IsNullOrWhiteSpace(Exercise.Name))
                throw new UnnamedEntityException($"{nameof(Exercise)} was not named.  Must be named before attempting to save.");

            AddExerciseToWorkout(workoutId);

            var workoutExercise = CreateNewWorkoutsToExercise(workoutId);

            var workoutExerciseId = DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise);

            ValidateWorkoutsToExerciseWasAdded(workoutExerciseId);
        }

        private LinkedWorkoutsToExercises CreateNewWorkoutsToExercise(int workoutId)
        {
            var exerciseId = DataAccessLayer.AddNewExercise(Exercise);

            var workoutExercise = new LinkedWorkoutsToExercises
                                  {
                                      ExerciseId   = exerciseId
                                    , WorkoutId    = workoutId
                                    , LengthOfTime = Exercise.LengthOfTime
                                  };

            return workoutExercise;
        }

        private void AddExerciseToWorkout(int workoutId)
        {
            var workout = DataAccessLayer.GetWorkout(workoutId);
            workout.Exercises.Add(Exercise);
        }

        private static void ValidateWorkoutsToExerciseWasAdded(int workoutExerciseId)
        {
            if (workoutExerciseId == 0)
            {
                throw new Exception("LinkedWorkoutsToExercises not added");
            }
        }

        public void DeleteExerciseType(int itemToDeleteId)
        {
            DataAccessLayer.DeleteExerciseType(Exercise.Id
                                             , itemToDeleteId);

            LoadTheTypesOfExercise();
        }

        public void DeleteExerciseEquipment(int itemToDeleteId)
        {
            DataAccessLayer.DeleteExerciseEquipment(Exercise.Id
                                                  , itemToDeleteId);

            LoadTheEquipment();
        }

        public void DeleteExerciseMuscleGroup(int itemToDeleteId)
        {
            DataAccessLayer.DeleteExerciseMuscleGroup(Exercise.Id
                                                    , itemToDeleteId);

            //LoadTheMuscleGroups();
        }
    }
}
