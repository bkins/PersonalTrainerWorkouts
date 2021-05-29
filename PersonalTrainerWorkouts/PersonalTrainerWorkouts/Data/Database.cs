using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Models;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using Xamarin.Forms.Internals;

namespace PersonalTrainerWorkouts.Data
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            
            CreateTables();
        }

        public void DropTables()
        {
            _database.DropTableAsync<Workouts>().Wait();
            _database.DropTableAsync<Exercises>().Wait();
            _database.DropTableAsync<WorkoutExercises>().Wait();
        }

        public void CreateTables()
        {
            _database.CreateTableAsync<Workouts>().Wait();
            _database.CreateTableAsync<Exercises>().Wait();
            _database.CreateTableAsync<WorkoutExercises>().Wait();
        }

        public Task<List<Workouts>> GetWorkoutsAsync()
        {
            //Get all notes.

            return _database.Table<Workouts>().ToListAsync();
        }

        public async Task<Workouts> GetWorkoutsAsync(string id)
        {
            return await GetWorkoutsAsync(Convert.ToInt32(id));
        }

        public async Task<Workouts> GetWorkoutsAsync(int id)
        {
            try
            {
                var workoutsWithChildren = _database.GetWithChildrenAsync<Workouts>(id).GetAwaiter().GetResult();
                return workoutsWithChildren;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
            // Get a specific note.
            //return _database.Table<Workouts>()
            //                .Where(i => i.Id == id)
            //                .FirstOrDefaultAsync();
        }
        
        public async Task<List<Exercises>> GetExercisesInWorkoutAsync(int workoutId)
        {
            try
            {
                var workoutsWithChildren = _database.GetWithChildrenAsync<Workouts>(workoutId).GetAwaiter().GetResult();
                return workoutsWithChildren.Exercises;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }
       
        public async Task<List<Exercises>> GetAllExercisesAsync()
        {
            try
            {
                return await _database.Table<Exercises>().ToListAsync();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<Exercises> GetExercise(int id)
        {
            try
            {
                var exercises = await GetAllExercisesAsync();

                return exercises.FirstOrDefault(exercise => exercise.Id == id);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }
        public async Task<Exercises> GetExerciseByName(string exerciseName)
        {
            try
            {
                var exercises = await GetAllExercisesAsync();

                return exercises.FirstOrDefault(exercise => exercise.Name == exerciseName);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }
        //https://matetiblog.wordpress.com/2018/06/17/xamarin-5-sqlite-1-to-many/
        public Task SaveWorkoutAsync(Workouts workout)
        {
            if (workout.Id == 0)
            {
                if (workout.Exercises.Count > 0)
                {
                    _database.InsertAsync(workout);
                    var exercises = workout.Exercises;
                    _database.InsertAsync(exercises)
                        .Wait();
                    return _database.UpdateWithChildrenAsync(_database);

                }
                else
                {
                    return _database.InsertAsync(workout);
                }
            }
            else
            {
                _database.UpdateAsync(workout)
                    .Wait();
                if (workout.Exercises.Count > 0)
                {
                    foreach (var workoutExercise in workout.Exercises)
                    {
                        _database.UpdateAsync(new Exercises()
                        {
                              Name = workoutExercise.Name
                            , Description = workoutExercise.Description
                            , LengthOfTime = workoutExercise.LengthOfTime
                            , PushPull = workoutExercise.PushPull
                        });
                        _database.InsertAsync(workoutExercise)
                            .Wait();
                    }

                    return _database.UpdateWithChildrenAsync(workout);
                }
                else
                {
                    return _database.UpdateAsync(workout);
                }
            }
        }

        public Task<int> DeleteWorkoutAsync(Workouts workout)
        {
            return _database.DeleteAsync(workout);
        }

        public Task<int> DeleteExerciseAsync(Exercises exercise)
        {
            return _database.DeleteAsync(exercise);
        }

    }
}
