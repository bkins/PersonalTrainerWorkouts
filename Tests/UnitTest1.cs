using System;
using System.Collections.Generic;
using System.IO;
using PersonalTrainerWorkouts.Data;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Views;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private static   Database          _database;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public static Database Database => _database ??= new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WorkoutDatabase.db3"));

        [Fact]
        public void Test1()
        {
            var test      = Database.GetWorkoutsAsync();
            var workoutId = "1";

            _testOutputHelper.WriteLine($"{nameof(WorkoutEntryPage)}?{nameof(WorkoutEntryPage.ItemId)}={workoutId}");
            Assert.IsType<List<Workouts>>(test.Result);
            
        }
    }
}
