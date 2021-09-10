using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersonalTrainerWorkouts.Models;

namespace PersonalTrainerWorkouts.ViewModels
{
    public class ResolvedSynergistViewModel : ViewModelBase
    {
        public int         Id                  { get; set; }
        public Exercise    Exercise            { get; set; }
        public MuscleGroup PrimaryMuscleGroup  { get; set; }
        public MuscleGroup OpposingMuscleGroup { get; set; }

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
            OpposingMuscleGroup = allMuscleGroups.First(field => field.Id == opposingMuscleGroupId);
        }

        public override string ToString()
        {
            return $"{PrimaryMuscleGroup.Name} {OpposingMuscleGroup.Name}";
        }
    }
}
