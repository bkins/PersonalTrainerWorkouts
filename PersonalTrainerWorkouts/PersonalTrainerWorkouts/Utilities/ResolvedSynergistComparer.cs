using System;
using System.Collections.Generic;
using System.Text;
using PersonalTrainerWorkouts.Models.Intermediates;
using PersonalTrainerWorkouts.ViewModels;

namespace PersonalTrainerWorkouts.Utilities
{
    public class ResolvedSynergistComparer : IEqualityComparer<ResolvedSynergistViewModel>
    {
        
            public int GetHashCode(ResolvedSynergistViewModel synergist)
            {
                return synergist.Id;
            }
		
            public bool Equals(ResolvedSynergistViewModel synergist1
                             , ResolvedSynergistViewModel synergist2)
            {
                return synergist2                     != null
                    && synergist1                     != null
                    && synergist1.PrimaryMuscleGroup  == synergist2.PrimaryMuscleGroup
                    && synergist1.OpposingMuscleGroup == synergist2.OpposingMuscleGroup;
            }
    }
}
