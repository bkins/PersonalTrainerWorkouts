using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ApplicationExceptions;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ExerciseAddEditViewModel  : ViewModelBase 
    {
        public Exercise                             Exercise            { get; set; }
        public Workout                              Workout             { get; set; }
        public string                               LengthOfTime        { get; set; }
        public int?                                 Reps                { get; set; }

        public TypeOfExercise                       TypeOfExercise      { get; set; }
        public ObservableCollection<TypeOfExercise> TypesOfExercise     { get; set; }
        public List<TypeOfExercise>                 TypesOfExerciseList { get; set; }

        public Equipment                       Equipment     { get; set; }
        public ObservableCollection<Equipment> Equipments    { get; set; }
        public List<Equipment>                 EquipmentList { get; set; }

        public IEnumerable<MuscleGroup>        MuscleGroups  { get; set; }

        public ExerciseAddEditViewModel()
        {
            Workout  = new Workout();
            Exercise = new Exercise();
        }

        public ExerciseAddEditViewModel(int workoutId, int exerciseId)
        {
            Workout  = DataAccessLayer.GetWorkout(workoutId);
            Exercise = Workout.Exercises.FirstOrDefault(e => e.Id == exerciseId) ?? new Exercise();

            LengthOfTime = Exercise?.LengthOfTime.ToTime()
                                    .ToShortForm();

            Reps = Exercise?.Reps;

            LoadTheTypesOfExercise();
            LoadTheEquipment();
        }

        private void LoadTheEquipment()
        {
            //var rack = new Equipment
            //           {
            //               Name = "Rack"
            //           };

            //var dumbBells = new Equipment
            //                {
            //                    Name = "Dumb Bells"
            //                };

            //var rackId      = DataAccessLayer.AddNewEquipment(rack);
            //var dumbBellsId = DataAccessLayer.AddNewEquipment(dumbBells);

            //var theRackToAdd = DataAccessLayer.GetAllEquipment()
            //                                  .First(field => field.Id == rackId);

            //var theDumbBellsToAdd = DataAccessLayer.GetAllEquipment()
            //                                       .First(field => field.Id == dumbBellsId);

            //Exercise.Equipment.Add(theRackToAdd);
            //Exercise.Equipment.Add(theDumbBellsToAdd);

            //DataAccessLayer.UpdateExercise(Exercise);

            var exerciseEquipment = DataAccessLayer.GetAllExerciseEquipment()
                                                   .Where(field => field.ExerciseId == Exercise.Id);

            var allTEquipment = DataAccessLayer.GetAllEquipment();

            var equipment = new ObservableCollection<Equipment>
                                (
                                    exerciseEquipment.Select(oneEquipment => allTEquipment.First(field => field.Id == oneEquipment.EquipmentId))
                                                     .ToList()
                                );

            Equipments    = equipment;
            EquipmentList = equipment.ToList();

        }

        private void LoadTheTypesOfExercise()
        {
            var exerciseTypes   = DataAccessLayer.GetAllExerciseTypes()
                                                 .Where(field => field.ExerciseId == Exercise.Id);

            var allTypes        = DataAccessLayer.GetAllTypesOfExercise();

            var typesOfExercise = new ObservableCollection<TypeOfExercise>
                                      (
                                        exerciseTypes.Select(exerciseType => allTypes.First(field => field.Id == exerciseType.TypeId))
                                                     .ToList()
                                      );

            TypesOfExercise     = typesOfExercise;
            TypesOfExerciseList = typesOfExercise.ToList();
        }

        public void UpdateExercise()
        {
            DataAccessLayer.UpdateExercise(Exercise);
        }

        public void SaveExercise(int workoutId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Exercise.Name))
                    throw new UnnamedEntityException($"{nameof(Exercise)} was not named.  Must be named before attempting to save.");
                
                AddExerciseToWorkout(workoutId);

                var workoutExercise = CreateNewWorkoutsToExercise(workoutId);

                var workoutExerciseId = DataAccessLayer.AddLinkedWorkoutsToExercises(workoutExercise);

                ValidateWorkoutsToExerciseWasAdded(workoutExerciseId);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message, Category.Error, e);

                throw;
            }
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
            DataAccessLayer.DeleteExerciseType(Exercise.Id, itemToDeleteId);
            LoadTheTypesOfExercise();
        }

        public void DeleteExerciseEquipment(int itemToDeleteId)
        {
            DataAccessLayer.DeleteExerciseEquipment(Exercise.Id
                                                  , itemToDeleteId);
            LoadTheEquipment();
        }
    }
}