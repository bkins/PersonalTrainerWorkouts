﻿using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;

namespace Tests.Helpers;

public class GoalStatusTestObject : Goal
{
    public Status ExpectedStatus      { get; set; }
}