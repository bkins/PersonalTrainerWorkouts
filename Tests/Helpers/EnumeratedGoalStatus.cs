using System;
using System.Collections;
using System.Collections.Generic;
using PersonalTrainerWorkouts.Models;

namespace Tests.Helpers;

public class EnumeratedGoalStatus : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
                         {
                             new List<GoalStatusTestObject>
                                 {
                                     GoalStatusTestScenarios.TargetDateIsTodaysDateExpectedToBeInProgress
                                   , GoalStatusTestScenarios.GoalStartedExpectedToBeInProgress
                                   , GoalStatusTestScenarios.GoalCompletedExpectedToBeCompletedSuccessfully
                                   , GoalStatusTestScenarios.GoalFailedExpectedToBeFailed
                                   , GoalStatusTestScenarios.GoalCompletedDateIsAfterTargetDateExpectedToBeMissTarget
                                   , GoalStatusTestScenarios
                                         .GoalStartDateIsNullButHasCompletedDatedDateExpectedToBeNotStarted
                                 }
                         };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
}