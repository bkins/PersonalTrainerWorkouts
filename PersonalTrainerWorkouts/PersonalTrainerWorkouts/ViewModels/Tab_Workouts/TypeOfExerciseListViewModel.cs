using System.Collections.Generic;
using System.Linq;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.HelperClasses;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Helpers;
using PersonalTrainerWorkouts.ViewModels.Tab_Workouts.Interfaces;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Workouts
{
    internal class TypeOfExerciseListViewModel : ViewModelBase, IViewModelPicker
    {
        public IEnumerable<TypeOfExercise>         ListOfAllExerciseTypes => DataAccessLayer.GetAllTypesOfExercise();
        public TypeOfExercise                      SelectedTypeOfExercise { get; set; }
        public IOrderedEnumerable<PickerViewModel> ListForDisplay         { get; set; }

        public TypeOfExerciseListViewModel()
        {
            var allExerciseChildren = ListOfAllExerciseTypes;

            ListForDisplay = allExerciseChildren.Select(exerciseChild => new PickerViewModel
                                                                         {
                                                                             Id   = exerciseChild.Id
                                                                           , Name = exerciseChild.Name
                                                                         })
                                                .ToList()
                                                .Concat(new[]
                                                        {
                                                            new PickerViewModel
                                                            {
                                                                Name = Constants.AddNew
                                                            }
                                                        })
                                                .OrderBy(exerciseChild => exerciseChild.Id);
        }

        public void SaveExerciseType(int exerciseId)
        {
            if (SelectedTypeOfExercise == null)
            {
                return;
            }

            DataAccessLayer.AddExerciseType(exerciseId
                                          , SelectedTypeOfExercise.Id);
        }

        public void SetSelectedTypeOfExercise(int selectedId)
        {
            SelectedTypeOfExercise = DataAccessLayer.GetAllTypesOfExercise().FirstOrDefault(type => type.Id == selectedId);

            if (SelectedTypeOfExercise is null)
            {
                Logger.WriteLineToToastForced("Something went wrong! Check logs"
                                            , Category.Error
                                            , $"Looked for TypeOfExercise.Id ({selectedId}), but nothing was found.");
            }
        }
    }
}
