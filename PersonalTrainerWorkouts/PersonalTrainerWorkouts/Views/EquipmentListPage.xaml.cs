using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ApplicationExceptions;
using Avails.D_Flat.Exceptions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.ViewModels;
using Xamarin.Forms;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views
{
    [QueryProperty(nameof(ExerciseId)
                 , nameof(ExerciseId))]
    public partial class EquipmentListPage : ContentPage
                                           , IQueryAttributable
    {
        private EquipmentListViewModel ViewModel { get; set; }

        public string ExerciseId { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                // The query parameter requires URL decoding.
                ExerciseId = HttpUtility.UrlDecode(query[nameof(ExerciseId)]);

                ViewModel = new EquipmentListViewModel();

                LoadData();
            }
            catch (Exception e)
            {
                Logger.WriteLine("Failed initiate ExerciseAddEditPage."
                               , Category.Error
                               , e);

                throw;
            }
        }

        private void LoadData()
        {
            EquipmentPicker.ItemsSource = ViewModel.ListOfAllExerciseEquipment.Concat(new[]
                                                                                      {
                                                                                          new Equipment()
                                                                                          {
                                                                                              Name = "<New>"
                                                                                          }
                                                                                      })
                                                   .OrderBy(field => field.Id);
        }

        public EquipmentListPage()
        {
            InitializeComponent();
        }

        private async void EquipmentPicker_OnOkButtonClicked(object                    sender
                                                           , SelectionChangedEventArgs e)
        {
            var selected = (Equipment)EquipmentPicker.SelectedItem;

            if (selected.Name == "<New>")
            {
                await SaveNewExerciseEquipment();
            }
            else if (ExerciseId != "0")
            {
                ViewModel.SelectedEquipment = selected;

                await TryToSaveExerciseEquipment();
            }
        }

        private async Task TryToSaveExerciseEquipment()
        {
            try
            {
                ViewModel.SaveExerciseEquipment(int.Parse(ExerciseId));

                PageNavigation.NavigateBackwards();
            }
            catch (EntityRelationAlreadyExistsException alreadyExistsException)
            {
                await DisplayAlert(Category.Warning.ToString()
                                 , alreadyExistsException.Message
                                 , "OK").ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Logger.WriteLine(exception.Message
                               , Category.Error
                               , exception);
            }
        }

        private async Task SaveNewExerciseEquipment()
        {
            var saved = false;

            while (! saved)
            {
                var equipmentName = await DisplayPromptAsync("New Equipment"
                                                           , "Enter the name of the new Equipment:"
                                                           , "OK"
                                                           , "Cancel"
                                                           , "Name"
                                                           , -1
                                                           , Keyboard.Create(KeyboardFlags.CapitalizeWord)
                                                           , "");

                var viewModel = new EquipmentViewModel(ExerciseId);

                try
                {
                    viewModel.SaveEquipment(equipmentName);

                    saved = true;

                    PageNavigation.NavigateBackwards();
                }
                catch (AttemptToAddDuplicateEntityException)
                {
                    await DisplayAlert("Equipment Already Exists"
                                     , $"The Equipment '{equipmentName}' already exists.  Name the Equipment with a different name."
                                     , "OK");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Something went wrong while trying to save the Equipment.  View the log for more details"
                                   , Category.Error
                                   , ex);
                }
            }
        }

        private void EquipmentPicker_OnCancelButtonClicked(object                    sender
                                                               , SelectionChangedEventArgs e)
        {
            PageNavigation.NavigateBackwards();
        }
    }
}
