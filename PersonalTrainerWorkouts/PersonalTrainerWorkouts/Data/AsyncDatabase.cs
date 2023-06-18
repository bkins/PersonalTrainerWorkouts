using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.Intermediates;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace PersonalTrainerWorkouts.Data
{
    public class AsyncDatabase
    {
        private readonly SQLiteAsyncConnection _asyncDatabase;

        public List<Workout> Workouts { get; set; }

        public List<Exercise> Exercises { get; set; }

        public List<WorkoutExercise> WorkoutExercises { get; set; }

        public AsyncDatabase(string dbPath)
        {
            _asyncDatabase = new SQLiteAsyncConnection(dbPath);

            CreateTables();

            FillModels();
        }

        public void DropTables()
        {
            _asyncDatabase.DropTableAsync<Workout>()
                          .Wait();

            _asyncDatabase.DropTableAsync<Exercise>()
                          .Wait();

            _asyncDatabase.DropTableAsync<WorkoutExercise>()
                          .Wait();

            Logger.WriteLine("Tables dropped"
                           , Category.Information);
        }

        public void CreateTables()
        {
            _asyncDatabase.CreateTableAsync<Workout>()
                          .Wait();

            _asyncDatabase.CreateTableAsync<Exercise>()
                          .Wait();

            _asyncDatabase.CreateTableAsync<WorkoutExercise>()
                          .Wait();

            Logger.WriteLine("Tables created"
                           , Category.Information);
        }

        public async void FillModels()
        {
            //I was having issues with getting values in and related to the WorkoutExercises
            //My workaround is to try to load the data into lists and access those lists
            Workouts = await _asyncDatabase.Table<Workout>()
                                           .ToListAsync();

            Exercises = await _asyncDatabase.Table<Exercise>()
                                            .ToListAsync();

            WorkoutExercises = await _asyncDatabase.Table<WorkoutExercise>()
                                                   .ToListAsync();
        }

        public Task<List<Workout>> GetWorkoutsAsync()
        {
            return _asyncDatabase.Table<Workout>()
                                 .ToListAsync();
        }

        public async Task<Workout> GetWorkoutsAsync(string id)
        {
            return await GetWorkoutsAsync(Convert.ToInt32(id));
        }

        public async Task<Workout> GetWorkoutsAsync(int id)
        {
            try
            {
                var workoutsWithChildren = await _asyncDatabase.GetWithChildrenAsync<Workout>(id).ConfigureAwait(false);

                return workoutsWithChildren;
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                return new Workout();
            }
        }

        public Task<List<Exercise>> GetExercisesInWorkoutAsync(string workoutId)
        {
            return GetExercisesInWorkoutAsync(Convert.ToInt32(workoutId));
        }

        public async Task<List<Exercise>> GetExercisesInWorkoutAsync(int workoutId)
        {
            try
            {
                var workoutsWithChildren = await _asyncDatabase.GetWithChildrenAsync<Workout>(workoutId).ConfigureAwait(false);

                return workoutsWithChildren.Exercises;
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                return null;
            }
        }

        public async Task<WorkoutExercise> GetExercise(int workoutId
                                                     , int exerciseId)
        {
            try
            {
                var allWorkoutExercises = await _asyncDatabase.GetAllWithChildrenAsync<WorkoutExercise>();

                var workoutExercise = allWorkoutExercises.Where(wo => wo.WorkoutId           == workoutId)
                                                         .FirstOrDefault(we => we.ExerciseId == exerciseId);

                return workoutExercise;
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                return null;
            }
        }

        public async Task<ObservableCollection<Exercise>> GetObservableCollectionOfExercisesAsync()
        {
            var exerciseList = await GetListOfExercisesAsync();

            return new ObservableCollection<Exercise>(exerciseList);

            //return new ObservableCollection<Exercises>(exerciseList.Where(field => field.Name == "Exercise 2" || field.Name == "1 Exercise"));
        }

        public async Task<List<Exercise>> GetListOfExercisesAsync()
        {
            try
            {
                //BENDO: I think this defeats the purpose of async.  Put it back after debugging
                var exercises = _asyncDatabase.Table<Exercise>();

                return await exercises.ToListAsync(); //.ToListAsync();
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                return null;
            }
        }

        public async Task<Exercise[]> GetAllExercisesAsync()
        {
            try
            {
                //BENDO: Why is this an array? Why not return a list?  Looks like I tried, but changed it to an array
                var exercises = _asyncDatabase.Table<Exercise>();

                return await exercises.ToArrayAsync(); //.ToListAsync();
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                return null;
            }
        }

        public Task<WorkoutExercise> GetWorkoutExercise(string workoutId
                                                      , string exerciseId)
        {
            return GetWorkoutExercise(Convert.ToInt32(workoutId)
                                    , Convert.ToInt32(exerciseId));
        }

        public Task<WorkoutExercise> GetWorkoutExercisesById(int id)
        {
            //return await _asyncDatabase.Table<WorkoutExercises>().Where(fields => fields.WorkoutId == workoutId).ToListAsync();
            return _asyncDatabase.Table<WorkoutExercise>()
                                 .Where(fields => fields.Id == id)
                                 .FirstOrDefaultAsync();
        }

        public Task<List<WorkoutExercise>> GetAllWorkoutExercisesByWorkout(int workoutId)
        {
            //return await _asyncDatabase.Table<WorkoutExercises>().Where(fields => fields.WorkoutId == workoutId).ToListAsync();
            return _asyncDatabase.Table<WorkoutExercise>()
                                 .Where(fields => fields.WorkoutId == workoutId)
                                 .ToListAsync();
        }

        public async Task<WorkoutExercise> GetWorkoutExercise(int workoutId
                                                            , int exerciseId)
        {
            var workoutExercises = await _asyncDatabase.Table<WorkoutExercise>()
                                                       .ToListAsync();

            if (workoutExercises != null)
            {
                return workoutExercises.FirstOrDefault(item => item.WorkoutId == workoutId & item.ExerciseId == exerciseId);
            }

            return new WorkoutExercise();
        }

        public Task<Exercise> GetExercise(string exerciseId)
        {
            return GetExercise(Convert.ToInt32(exerciseId));
        }

        public async Task<Exercise> GetExercise(int exerciseId)
        {
            try
            {
                var exercises = await GetAllExercisesAsync();

                return exercises.FirstOrDefault(exercise => exercise.Id == exerciseId);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                return null;
            }
        }

        public async Task<Exercise> GetExerciseByName(string exerciseName)
        {
            try
            {
                var exercises = await GetAllExercisesAsync();

                return exercises.FirstOrDefault(exercise => exercise.Name == exerciseName);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                return null;
            }
        }

        public Task SaveExerciseAsync(Exercise exercise)
        {
            if (exercise.Id == 0)
            {
                Task insertTask = _asyncDatabase.InsertAsync(exercise);

                return insertTask;
            }

            Task updateTask = _asyncDatabase.UpdateAsync(exercise);

            return updateTask;
        }

        public Task<Workout> GetWorkoutById(int workoutId)
        {
            Task<Workout> workout;

            try
            {
                workout = _asyncDatabase.Table<Workout>()
                                        .Where(fields => fields.Id == workoutId)
                                        .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.Message
                               , Category.Error
                               , e);

                throw;
            }

            return workout;
        }

        public Task<Workout> GetWorkoutByNameAsync(string name)
        {
            return _asyncDatabase.Table<Workout>()
                                 .Where(fields => fields.Name == name)
                                 .FirstOrDefaultAsync();
        }

        //https://matetiblog.wordpress.com/2018/06/17/xamarin-5-sqlite-1-to-many/
        public Task SaveWorkoutAsync(Workout workout)
        {
            //BENDO:  Refactor to reduce nesting, and make it do one thing

            //Is this a new workout?
            if (workout.Id == 0)
            {
                //if (workout.Exercises.Count > 0)
                //{
                //    _asyncDatabase.InsertAsync(workout);

                //    var exercises = workout.Exercises;

                //    _asyncDatabase.InsertAsync(exercises).Wait();

                //    return _asyncDatabase.UpdateAsync(_asyncDatabase);
                //}
                var task = _asyncDatabase.InsertAsync(workout);

                return task;
            }

            //if (workout.Exercises.Count > 0)
            //{
            //    foreach (var exercise in workout.Exercises)
            //    {
            //        SaveExerciseAsync(exercise);
            //    }
            //}

            //_asyncDatabase.UpdateAsync(workout).Wait();

            //This is an existing workout.  Are there exercises associated with it?
            if (workout.Exercises.Count > 0)
            {
                //foreach (var workoutExercise in workout.Exercises)
                //{
                //    _asyncDatabase.UpdateAsync(workoutExercise).Wait();
                //    //_asyncDatabase.UpdateAsync(new Exercises()
                //    //                                {
                //    //                                      Name          = workoutExercise.Name
                //    //                                    , Description   = workoutExercise.Description
                //    //                                    , LengthOfTime  = workoutExercise.LengthOfTime
                //    //                                }
                //    //                           );
                //    //_asyncDatabase.InsertAsync(workoutExercise).Wait();
                //}

                return _asyncDatabase.UpdateWithChildrenAsync(workout);
            }

            //Update existing workout that has no associated exercises
            return _asyncDatabase.UpdateAsync(workout);
        }

        //public Task SaveWorkoutExerciseAsync(int workoutExerciseId
        //                                   , int lengthOfTime)
        //{
        //    Task<int> returnValue = null;
        //    var workoutExercise = App.DataStore.GetAllWorkoutExercises()
        //                                       .FirstOrDefault( item => item.Id == workoutExerciseId); //GetWorkoutExercise(workoutId, exerciseId).Result;

        //    if (workoutExercise != null)
        //    {
        //        workoutExercise.LengthOfTime = lengthOfTime;

        //        returnValue = _asyncDatabase.UpdateAsync(workoutExercise);

        //    }
        //    return returnValue;
        //}
        //BENDO: SaveNewWorkoutExerciseAsync is very similar to SaveWorkoutExerciseAsync. Refactored these into one.
        //public Task SaveNewWorkoutExerciseAsync(int workoutId
        //                                      , int exerciseId
        //                                      , int lengthOfTime)
        //{
        //    Task<int> returnValue = null;
        //    var workoutExercise = App.DataStore.GetAllWorkoutExercises()
        //                                       .FirstOrDefault(item => item.WorkoutId == workoutId 
        //                                                            && item.ExerciseId == exerciseId); //GetWorkoutExercise(workoutId, exerciseId).Result;

        //    if (workoutExercise != null)
        //    {
        //        workoutExercise.LengthOfTime = lengthOfTime;

        //        returnValue = _asyncDatabase.InsertAsync(workoutExercise);

        //    }
        //    return returnValue;
        //}

        public Task<int> DeleteWorkoutAsync(Workout workout)
        {
            return _asyncDatabase.DeleteAsync(workout);
        }

        public Task<int> DeleteExerciseAsync(Exercise exercise)
        {
            return _asyncDatabase.DeleteAsync(exercise);
        }
    }
}
