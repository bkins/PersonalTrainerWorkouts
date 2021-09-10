using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class MuscleGroupViewModel : ViewModelBase
    {
        private Exercise                 Exercise          { get; }
        public  IList<ResolvedSynergistViewModel> Synergists        { get; set; }
        public  ResolvedSynergistViewModel        SelectedSynergist { get; set; }

        public MuscleGroupViewModel(string exerciseId)
        {
            //Use the Exercise to assign the selected Synergist to.
            Exercise   = DataAccessLayer.GetExercise(int.Parse(exerciseId));

            Synergists = new List<ResolvedSynergistViewModel>();
            var synergistsFromDb = DataAccessLayer.GetAllSynergists() 
                                                  .ToList();

            var listOfSynergists = new List<ResolvedSynergistViewModel>();

            foreach (var synergist in synergistsFromDb)
            {
                Synergists.Add(new ResolvedSynergistViewModel(synergist.Id
                                                   , synergist.ExerciseId
                                                   , synergist.MuscleGroupId
                                                   , synergist.OpposingMuscleGroupId));
            } 
        }

        public void SaveNewSynergist(string muscleGroupName, string opposingMuscleGroupName)
        {

            if (muscleGroupName.IsNullEmptyOrWhitespace()
            || Exercise.Id == 0)
            {
                Logger.WriteLine("Either the muscle does not have a name or the Exercise has not been defined. Synergist was not saved"
                               , Category.Warning);
                return;
            }

            var newPrimaryMuscleGroupId  = SaveNewMuscleGroup(muscleGroupName);
            var newOpposingMuscleGroupId = SaveNewMuscleGroup(opposingMuscleGroupName);
            
            DataAccessLayer.AddSynergist(new Synergist
                                         {
                                             ExerciseId            = Exercise.Id
                                           , MuscleGroupId         = newPrimaryMuscleGroupId
                                           , OpposingMuscleGroupId = newOpposingMuscleGroupId
                                         });
        }
        
        public void SaveOppositeSynergist(string muscleGroupName
                                        , string opposingMuscleGroupName)
        {
            var allMuscleGroups     = DataAccessLayer.GetAllMuscleGroups().ToList();
            
            //Make the Primary Muscle the opposing muscle group and via versa
            var primaryMuscleGroup  = allMuscleGroups.First(field => field.Name == opposingMuscleGroupName);
            var opposingMuscleGroup = allMuscleGroups.First(field => field.Name == muscleGroupName);

            //Add Synergist without assigning an Exercise
            DataAccessLayer.AddSynergist(new Synergist
                                         {
                                             ExerciseId            = 0
                                           , MuscleGroupId         = primaryMuscleGroup.Id
                                           , OpposingMuscleGroupId = opposingMuscleGroup.Id
                                         });
        }

        public int SaveNewMuscleGroup(string newMuscleGroupName)
        {
            var newMuscleGroup = new MuscleGroup
                                 {
                                     Name = newMuscleGroupName
                                 };
            
            return DataAccessLayer.AddNewMuscleGroup(newMuscleGroup);
        }

        public void SaveSynergistToExercise()
        {
            if (Exercise.Id == 0)
            {
                Logger.WriteLine("The Exercise was not defined.  Synergist was not saved.", Category.Warning);
                return;
            }

            DataAccessLayer.AddSynergist(new Synergist
                                         {
                                             ExerciseId            = Exercise.Id
                                           , MuscleGroupId         = SelectedSynergist.PrimaryMuscleGroup.Id
                                           , OpposingMuscleGroupId = SelectedSynergist.OpposingMuscleGroup.Id
                                         });
        }

    }
}
