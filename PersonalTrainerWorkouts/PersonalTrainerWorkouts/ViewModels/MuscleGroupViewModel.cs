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
        private Exercise                          Exercise                { get; set; }
        public  IList<ResolvedSynergistViewModel> Synergists              { get; set; }
        public  ResolvedSynergistViewModel        SelectedSynergist       { get; set; }
        public  IList<ResolvedSynergistViewModel> SynergistsNotInExercise { get; set; }

        public MuscleGroupViewModel(string exerciseId)
        {
            //Use the Exercise to assign the selected Synergist to.
            SetSynergistsInExercise(exerciseId);

            //BENDO: Not sure if the '.Distinct' in this Get is doing what I intend
            var synergistsFromDb = DataAccessLayer.GetAllSynergists()
                                                  .Distinct()
                                                  .ToList();

            //1. Resolved each synergist in synergistsFromDb
            //BENDO: do I need to resolve these??
            //       Maybe, if I resolve these then I will be comparing apples (ResolvedSynergistViewModel) to apple (ResolvedSynergistViewModel))
            //       It might make it easier, but probably not necessary.

            var listOfResolvedSynergist = synergistsFromDb.Select(synergist => new ResolvedSynergistViewModel(synergist)).ToList();
            
            SynergistsNotInExercise = new List<ResolvedSynergistViewModel>();

            //BENDO: Can SetSynergistsNotInExerciseWhenExerciseHasNone & SetSynergistsNotInExerciseWhenExerciseHasSynergists be combine into 1 method and streamlined?
            //SetSynergistsNotInExerciseWhenExerciseHasNone works as expected
            if (SetSynergistsNotInExerciseWhenExerciseHasNone(listOfResolvedSynergist))
            {
                return;
            }

            //SetSynergistsNotInExerciseWhenExerciseHasSynergists is NOT working
            SetSynergistsNotInExerciseWhenExerciseHasSynergists(listOfResolvedSynergist);
            
        }

        private void SetSynergistsNotInExerciseWhenExerciseHasSynergists(List<ResolvedSynergistViewModel> listOfResolvedSynergist)
        {
            //Synergists has items
            foreach (var dbSynergist in listOfResolvedSynergist)
            {
                //The Exercise has synergists assigned to it
                //Then only get a distinct list of synergist that the Exercise does not have
                foreach (var exerciseSynergists in Synergists)
                {
                    if (dbSynergist.Exercise.Id == exerciseSynergists.Exercise.Id
                     || (dbSynergist.PrimaryMuscleGroup.Id == exerciseSynergists.PrimaryMuscleGroup.Id && dbSynergist.OpposingMuscleGroup.Id == exerciseSynergists.OpposingMuscleGroup.Id))
                    {
                        //exclude
                    }
                    else
                    {
                        SynergistsNotInExercise.Add(dbSynergist);
                    }
                }
            }
        }

        private bool SetSynergistsNotInExerciseWhenExerciseHasNone(List<ResolvedSynergistViewModel> listOfResolvedSynergist)
        {
            if (! Synergists.Any())
            {
                foreach (var dbSynergist in listOfResolvedSynergist)
                {
                    //The Exercise does not have any synergist assigned to it.
                    //Then get a distinct list of all synergists from the DB
                    var synergistToAdd = new ResolvedSynergistViewModel(0
                                                                      , 0
                                                                      , dbSynergist.PrimaryMuscleGroup.Id
                                                                      , dbSynergist.OpposingMuscleGroup.Id);

                    if (! SynergistsNotInExercise.Any(field => field.PrimaryMuscleGroup == synergistToAdd.PrimaryMuscleGroup && field.OpposingMuscleGroup == synergistToAdd.OpposingMuscleGroup))
                    {
                        SynergistsNotInExercise.Add(synergistToAdd);
                    }
                }
                
                return true;
            }

            return false;
        }

        private void SetSynergistsInExercise(string exerciseId)
        {
            Exercise   = DataAccessLayer.GetExercise(int.Parse(exerciseId));
            Synergists = new List<ResolvedSynergistViewModel>();

            if (Exercise.Synergists.Any())
            {
                var exerciseSynergists = Exercise.Synergists;

                foreach (var synergist in exerciseSynergists)
                {
                    Synergists.Add(new ResolvedSynergistViewModel(synergist.Id
                                                                , synergist.ExerciseId
                                                                , synergist.MuscleGroupId
                                                                , synergist.OpposingMuscleGroupId));
                }
            }
        }

        public void SaveNewSynergist(string muscleGroupName
                                   , string opposingMuscleGroupName)
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
            if (opposingMuscleGroupName.IsNullEmptyOrWhitespace())
            {
                return; //Don't add the opposite synergist if no opposing muscle group is defined
            }

            var allMuscleGroups = DataAccessLayer.GetAllMuscleGroups()
                                                 .ToList();

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
            if (newMuscleGroupName.IsNullEmptyOrWhitespace())
            {
                return 0;
            }

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
                Logger.WriteLine("The Exercise was not defined.  Synergist was not saved."
                               , Category.Warning);

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
