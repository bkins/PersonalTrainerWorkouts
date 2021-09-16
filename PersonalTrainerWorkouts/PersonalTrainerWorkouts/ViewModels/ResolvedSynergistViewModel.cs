using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.Utilities;

namespace PersonalTrainerWorkouts.ViewModels
{
    //BENDO:  I do not like the naming I chose here.  There should be a ResolvedSynergist class and then this class has a member of that type
    //So, deconstruct this class and create two classes.
    //The first will be the ResolvedSynergist, pulling all members and methods that are responsible for resolving the synergist, and
    //the second will be the ResolvedSynergistViewModel, pull all methods that are responsible for arranging the data for the View.
    public class ResolvedSynergistViewModel : ViewModelBase
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
        }

        public ResolvedSynergistViewModel(Synergist synergist) : this(synergist.Id
                                                                    , synergist.ExerciseId
                                                                    , synergist.MuscleGroupId
                                                                    , synergist.OpposingMuscleGroupId)
        {
            
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
