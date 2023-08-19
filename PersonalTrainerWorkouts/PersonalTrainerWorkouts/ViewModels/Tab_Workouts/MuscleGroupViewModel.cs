using System.Collections.Generic;
using System.Linq;
using Avails.D_Flat.Extensions;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    public class MuscleGroupViewModel : ViewModelBase
    {
        private Exercise                            Exercise                { get; set; }
        public  List<ResolvedSynergistViewModel>    Synergists              { get; set; }
        public  ResolvedSynergistViewModel          SelectedSynergist       { get; set; }
        public  IList<ResolvedSynergistViewModel>   SynergistsNotInExercise { get; set; }

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
            var synergists =
                (from dbSynergist
                    in listOfResolvedSynergist
                from exerciseSynergists
                    in Synergists.Where(exerciseSynergists => SynergistIsNotInExercise(dbSynergist, exerciseSynergists))
                select dbSynergist).ToList();
            
            foreach (var dbSynergist in synergists)
            {
                SynergistsNotInExercise.Add(dbSynergist);
            }
        }

        private bool SynergistIsNotInExercise(ResolvedSynergistViewModel dbSynergist
                                            , ResolvedSynergistViewModel exerciseSynergists)
        {
            return ! (dbSynergist.Exercise.Id == exerciseSynergists.Exercise.Id
                   || (   dbSynergist.PrimaryMuscleGroup.Id  == exerciseSynergists.PrimaryMuscleGroup.Id 
                       && dbSynergist.OpposingMuscleGroup.Id == exerciseSynergists.OpposingMuscleGroup.Id)
                   || SynergistsNotInExercise.Any(field=> field.PrimaryMuscleGroup.Id  == dbSynergist.PrimaryMuscleGroup.Id 
                                                       && field.OpposingMuscleGroup.Id == dbSynergist.OpposingMuscleGroup.Id));
        }

        private bool SetSynergistsNotInExerciseWhenExerciseHasNone(IEnumerable<ResolvedSynergistViewModel> listOfResolvedSynergist)
        {
            if (Synergists.Any())
            {
                return false;
            }

            var synergistsToAdd = listOfResolvedSynergist.Select
                                                         (
                                                             dbSynergist => new ResolvedSynergistViewModel(
                                                                 0
                                                               , 0
                                                               , dbSynergist.PrimaryMuscleGroup.Id
                                                               , dbSynergist.OpposingMuscleGroup.Id
                                                             )
                                                         )
                                                         .Where
                                                         (
                                                             synergistToAdd => ! SynergistsNotInExercise.Any
                                                             (
                                                                 field =>
                                                                     field.PrimaryMuscleGroup == synergistToAdd.PrimaryMuscleGroup
                                                                  && field.OpposingMuscleGroup == synergistToAdd.OpposingMuscleGroup
                                                             )
                                                         ).ToList();
            
            foreach (var synergistToAdd in synergistsToAdd)
            {
                SynergistsNotInExercise.Add(synergistToAdd);
            }

            return true;

        }

        public void SetSynergistsInExercise(string exerciseId)
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

        public Synergist SaveNewSynergist(string muscleGroupName
                                   , string opposingMuscleGroupName)
        {
            if (muscleGroupName.IsNullEmptyOrWhitespace()
             || Exercise.Id == 0)
            {
                Logger.WriteLine("Either the muscle does not have a name or the Exercise has not been defined. Synergist was not saved"
                               , Category.Warning);

                return new Synergist();
            }

            var newPrimaryMuscleGroupId  = SaveNewMuscleGroup(muscleGroupName);
            var newOpposingMuscleGroupId = SaveNewMuscleGroup(opposingMuscleGroupName);
            var newSynergist = new Synergist
                               {
                                   ExerciseId            = Exercise.Id
                                 , MuscleGroupId         = newPrimaryMuscleGroupId
                                 , OpposingMuscleGroupId = newOpposingMuscleGroupId
                               };
            DataAccessLayer.AddSynergist(ref newSynergist);

            return newSynergist;
        }

        public Synergist SaveOppositeSynergist(string muscleGroupName
                                        , string opposingMuscleGroupName)
        {
            if (opposingMuscleGroupName.IsNullEmptyOrWhitespace())
            {
                return new Synergist(); //Don't add the opposite synergist if no opposing muscle group is defined
            }

            var allMuscleGroups = DataAccessLayer.GetAllMuscleGroups()
                                                 .ToList();

            //Make the Primary Muscle the opposing muscle group and via versa
            var primaryMuscleGroup  = allMuscleGroups.First(field => field.Name == opposingMuscleGroupName);
            var opposingMuscleGroup = allMuscleGroups.First(field => field.Name == muscleGroupName);

            //Add Synergist without assigning an Exercise
            var newSynergist = new Synergist
                               {
                                   ExerciseId            = 0
                                 , MuscleGroupId         = primaryMuscleGroup.Id
                                 , OpposingMuscleGroupId = opposingMuscleGroup.Id
                               };
            DataAccessLayer.AddSynergist(ref newSynergist);

            return newSynergist;
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

        public int SaveSynergistToExercise()
        {
            if (Exercise.Id == 0)
            {
                Logger.WriteLine("The Exercise was not defined.  Synergist was not saved."
                               , Category.Warning);

                return 0;
            }

            var newSynergist = new Synergist
                               {
                                   ExerciseId            = Exercise.Id
                                 , MuscleGroupId         = SelectedSynergist.PrimaryMuscleGroup.Id
                                 , OpposingMuscleGroupId = SelectedSynergist.OpposingMuscleGroup.Id
                               };
            DataAccessLayer.AddSynergist(ref newSynergist);

            return newSynergist.Id;
        }

    }
}
