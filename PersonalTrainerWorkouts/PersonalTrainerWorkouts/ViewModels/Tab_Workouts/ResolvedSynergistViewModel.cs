using System.Linq;
using Avails.D_Flat.Extensions;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.HelperClasses;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Helpers;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Interfaces;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    //BENDO:  I do not like the naming I chose here.  There should be a ResolvedSynergist class and then this class has a member of that type
    //So, deconstruct this class and create two classes.
    //The first will be the ResolvedSynergist, pulling all members and methods that are responsible for resolving the synergist, and
    //the second will be the ResolvedSynergistViewModel, pull all methods that are responsible for arranging the data for the View.
    public class ResolvedSynergistViewModel : ViewModelBase, IViewModelPicker
    {
        public int         Id                  { get; set; }
        public Exercise    Exercise            { get; set; }
        public MuscleGroup PrimaryMuscleGroup  { get; set; }
        public MuscleGroup OpposingMuscleGroup { get; set; }

        public string DisplayedSynergist
        {
            get
            {
                var displayedValue = PrimaryMuscleGroup.Name;

                if (OpposingMuscleGroup.Name.HasValue())
                {
                    displayedValue += $" ({OpposingMuscleGroup.Name})";
                }

                return displayedValue;
            }
        }

        public IOrderedEnumerable<PickerViewModel> ListForDisplay { get; set; }

        public ResolvedSynergistViewModel()
        {
            Id       = 0;
            Exercise = new Exercise();

            PrimaryMuscleGroup = new MuscleGroup
                                 {
                                     Name = "<Add"
                                 };

            OpposingMuscleGroup = new MuscleGroup
                                  {
                                      Name = "New>"
                                  };
            SetListForDisplay();
        }

        public ResolvedSynergistViewModel(Synergist synergist)
            : this(synergist.Id
                 , synergist.ExerciseId
                 , synergist.MuscleGroupId
                 , synergist.OpposingMuscleGroupId)
        {
            SetListForDisplay();
        }

        public ResolvedSynergistViewModel(int synergistId
                                        , int exerciseId
                                        , int primaryMuscleGroupId
                                        , int opposingMuscleGroupId)
        {
            Id = synergistId;

            Exercise = exerciseId > 0 ?
                               DataAccessLayer.GetExercise(exerciseId) :
                               new Exercise();

            var allMuscleGroups = DataAccessLayer.GetAllMuscleGroups()
                                                 .ToList();

            PrimaryMuscleGroup  = allMuscleGroups.First(field => field.Id == primaryMuscleGroupId);

            OpposingMuscleGroup = opposingMuscleGroupId == 0 ?
                                          new MuscleGroup() :
                                          allMuscleGroups.First(field => field.Id == opposingMuscleGroupId);

            SetListForDisplay();
        }

        public ResolvedSynergistViewModel(PickerViewModel newSynergist, int ExerciseId)
        {

            var muscleGroupNames = newSynergist.Name.Split('(');
            if (muscleGroupNames.Length != 2)
            {
                Logger.WriteLineToToastForced($"Something went wrong! Check logs."
                                            , Category.Error
                                            , $"Invalid characters in muscle group name combination: {newSynergist.Name}");
                return;
            }

            var primaryMuscleGroupName  = muscleGroupNames[0].Trim();
            var opposingMuscleGroupName = muscleGroupNames[1].Replace(")", "");
            var allMuscleGroups         = DataAccessLayer.GetAllMuscleGroups().ToList();
            PrimaryMuscleGroup             = allMuscleGroups.FirstOrDefault(prime => prime.Name == primaryMuscleGroupName);
            OpposingMuscleGroup             = allMuscleGroups.FirstOrDefault(other => other.Name == opposingMuscleGroupName);

            var theNewSynergist = new Synergist();

            if (newSynergist.Id == 0)
            {
                if (PrimaryMuscleGroup?.Id == 0)
                {
                    DataAccessLayer.AddNewMuscleGroup(new MuscleGroup
                                                      {
                                                          Name = primaryMuscleGroupName
                                                      });
                    DataAccessLayer.AddNewMuscleGroup(new MuscleGroup
                                                      {
                                                          Name = opposingMuscleGroupName
                                                      });
                    allMuscleGroups = DataAccessLayer.GetAllMuscleGroups().ToList();

                    PrimaryMuscleGroup = allMuscleGroups.FirstOrDefault(muscle => muscle.Name == primaryMuscleGroupName);
                    OpposingMuscleGroup = allMuscleGroups.FirstOrDefault(other => other.Name == opposingMuscleGroupName);
                }

                if (PrimaryMuscleGroup != null
                 && OpposingMuscleGroup != null) //This is mostly to satisfy the IDE.  We just add the groups to the DB or they had an Id.
                {
                    theNewSynergist = new Synergist
                                      {
                                          ExerciseId            = ExerciseId
                                        , MuscleGroupId         = PrimaryMuscleGroup.Id
                                        , OpposingMuscleGroupId = OpposingMuscleGroup.Id
                                      };
                    DataAccessLayer.AddSynergist(ref theNewSynergist);

                    Id         = theNewSynergist.Id;
                    ExerciseId = theNewSynergist.ExerciseId;
                }

            }
            else //newSynergist has an Id (non zero)
            {
                var foundSynergist = DataAccessLayer.GetAllSynergists()
                                                    .FirstOrDefault(synergist => synergist.Id == newSynergist.Id);

                if (foundSynergist is null)
                {
                    Logger.WriteLine("Synergist could not be found."
                                   , Category.Error
                                   , null
                                   , $"Looked for Synergists.Id ({newSynergist.Id}) but it could not be found");
                    Logger.WriteLineToToastForced("Something went wrong. Check logs.", Category.Error);
                    return;
                }

                Id       = foundSynergist.Id;
                Exercise = DataAccessLayer.GetExercise(foundSynergist.ExerciseId);
                PrimaryMuscleGroup = DataAccessLayer.GetAllMuscleGroups()
                                                    .FirstOrDefault(muscle => muscle.Id == foundSynergist.MuscleGroupId);
                OpposingMuscleGroup = DataAccessLayer.GetAllMuscleGroups()
                                                     .FirstOrDefault(muscle => muscle.Id == foundSynergist.OpposingMuscleGroupId);
            }
        }

        private void SetListForDisplay()
        {
            var allSynergists = DataAccessLayer.GetAllSynergists();
            var listForDisplay = (from synergist in allSynergists
                                  let muscleGroup = DataAccessLayer.GetAllMuscleGroups()
                                                                   .FirstOrDefault(muscle => muscle.Id == synergist.MuscleGroupId)
                                  where muscleGroup is not null
                                  let opposingMuscleGroup = DataAccessLayer.GetOpposingMuscleGroupByMuscleGroup(muscleGroup.Id)
                                  select new PickerViewModel
                                         {
                                             Id   = synergist.Id
                                           , Name = $"{muscleGroup.Name} ({opposingMuscleGroup.Name})"
                                         })
                                .ToList();

            ListForDisplay = listForDisplay.Concat(new[]
                                                   {
                                                       new PickerViewModel
                                                       {
                                                           Name = Constants.AddNew
                                                       }
                                                   })
                                           .OrderBy(exerciseChild => exerciseChild.Id);
        }

        public override string ToString()
        {
            if (PrimaryMuscleGroup.Name == "<Add")
            {
                return $"{PrimaryMuscleGroup.Name} {OpposingMuscleGroup.Name}";
            }

            if (OpposingMuscleGroup.Name.HasValue())
            {
                return $"{PrimaryMuscleGroup.Name} ({OpposingMuscleGroup.Name})";
            }

            return PrimaryMuscleGroup.Name;
        }

    }
}
