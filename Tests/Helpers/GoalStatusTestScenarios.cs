using System;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;

namespace Tests.Helpers;

public static class GoalStatusTestScenarios
{
    /*
                     todaysDate   startDate    targetDate   completedDate   failed expectedStatus
                     -----------  -----------  -----------  -------------   ------ -----------------------------------
         [InlineData("5/28/2023", "5/27/2023", "5/28/2023", null,           false, Goal.Status.InProgress)]
         [InlineData("5/28/2023", "5/27/2023", "6/27/2023", null,           false, Goal.Status.InProgress)]
         [InlineData("5/28/2023", "5/27/2023", "6/27/2023", "6/10/2023",    false, Goal.Status.CompletedSuccessfully)]
         [InlineData("5/28/2023", "5/27/2023", "6/27/2023", null,           true,  Goal.Status.Failed)]
         [InlineData("5/28/2023", null,        "6/27/2023", "6/10/2023",    false, Goal.Status.NotStarted)]
         [InlineData("5/28/2023", "5/27/2023", "6/27/2023", "6/28/2023",    false, Goal.Status.MissTarget)]
    */

    public static GoalStatusTestObject
        GoalCompletedDateIsAfterTargetDateExpectedToBeMissTarget => new()
                                                                        {
                                                                            ExpectedStatus = Goal.Status.InProgress
                                                                          , TodaysDate     = DateTime.Parse("5/28/2023")
                                                                          , DateStarted    = ParseDateTime("5/27/2023")
                                                                          , TargetDate     = DateTime.Parse("6/27/2023")
                                                                          , DateCompleted  = ParseDateTime("6/28/2023")
                                                                          , Failed         = false
                                                                        };
    public static GoalStatusTestObject
        GoalStartDateIsNullButHasCompletedDatedDateExpectedToBeNotStarted
        => new()
               {
                   ExpectedStatus = Goal.Status.InProgress
                 , TodaysDate     = DateTime.Parse("5/28/2023")
                 , DateStarted    = null
                 , TargetDate     = DateTime.Parse("6/27/2023")
                 , DateCompleted  = ParseDateTime("6/10/2023")
                 , Failed         = true
               };
    
    public static GoalStatusTestObject
        GoalFailedExpectedToBeFailed => new()
                                            {
                                                ExpectedStatus = Goal.Status.InProgress
                                              , TodaysDate     = DateTime.Parse("5/28/2023")
                                              , DateStarted    = ParseDateTime("5/27/2023")
                                              , TargetDate     = DateTime.Parse("6/27/2023")
                                              , DateCompleted  = null
                                              , Failed         = true
                                            };
    public static GoalStatusTestObject
        GoalCompletedExpectedToBeCompletedSuccessfully => new()
                                                              {
                                                                  ExpectedStatus = Goal.Status.InProgress
                                                                , TodaysDate     = DateTime.Parse("5/28/2023")
                                                                , DateStarted    = ParseDateTime("5/27/2023")
                                                                , TargetDate     = DateTime.Parse("6/27/2023")
                                                                , DateCompleted  = DateTime.Parse("6/10/2023")
                                                                , Failed         = false
                                                              };
    public static GoalStatusTestObject
        GoalStartedExpectedToBeInProgress => new()
                                                 {
                                                     ExpectedStatus = Goal.Status.InProgress
                                                   , TodaysDate     = DateTime.Parse("5/28/2023")
                                                   , DateStarted    = ParseDateTime("5/27/2023")
                                                   , TargetDate     = DateTime.Parse("6/27/2023")
                                                   , DateCompleted  = null
                                                   , Failed         = false
                                                 };
    public static GoalStatusTestObject 
        TargetDateIsTodaysDateExpectedToBeInProgress = new()
                                                           {
                                                               ExpectedStatus = Goal.Status.InProgress
                                                             , TodaysDate     = DateTime.Parse("5/28/2023")
                                                             , DateStarted    = ParseDateTime("5/27/2023")
                                                             , TargetDate     = DateTime.Parse("5/28/2023")
                                                             , DateCompleted  = null
                                                             , Failed         = false
                                                           };

    private static DateTime? ParseDateTime(string dateTime)
    {
        return dateTime is null
            ? null
            : DateTime.Parse(dateTime);
    }
}