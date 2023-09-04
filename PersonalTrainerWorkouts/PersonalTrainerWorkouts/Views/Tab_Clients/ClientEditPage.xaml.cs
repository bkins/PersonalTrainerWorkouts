using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Avails.D_Flat.Extensions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.ViewModels.HelperClasses;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;
using PersonalTrainerWorkouts.Views.Popups;
using Syncfusion.SfRadialMenu.XForms;
using Syncfusion.XForms.RichTextEditor;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemTappedEventArgs = Syncfusion.SfRadialMenu.XForms.ItemTappedEventArgs;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Clients
{
    [QueryProperty(nameof(ClientId)
                 , nameof(ClientId))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientEditPage : IQueryAttributable
    {
        private const string TapToChange = "Tap to change";
        private const string TapWhenDone = "Tap when done";

        public        string          ClientId { get; set; }
        public        ClientViewModel ClientVm { get; set; }
        public        GoalViewModel   GoalVm   { get; set; }

        public ClientEditPage()
        {
            InitializeComponent();

            var collection = new ObservableCollection<object>();
            collection.Add(ToolbarOptions.Italic);
            collection.Add(ToolbarOptions.Underline);
            collection.Add(ToolbarOptions.Bold);

            collection.Add(ToolbarOptions.NumberList);
            collection.Add(ToolbarOptions.BulletList);

            ClientNoteRte.ToolbarItems = collection;

            ColorValueLabel.Text = TapToChange;
        }

        public ClientEditPage(string clientId)
        {
            ClientId = clientId;
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                ClientId = HttpUtility.UrlDecode(query[nameof(ClientId)]);
                ClientVm = new ClientViewModel(ClientId);

                LoadData();
            }
            catch (Exception e)
            {
                Logger.WriteLine($"Failed to initiate {nameof(ClientEditPage)}."
                               , Category.Error
                               , e);
            }
        }

        private void LoadData()
        {
            ClientMainPhoneValueLabel.Text  = ClientVm.Client.MainNumber;
            ClientNoteRte.Text              = ClientVm.Client.Notes;
            GoalVm                          = new GoalViewModel(ClientId.ToSafeInt());
            BindingContext                  = GoalVm;
            GoalsCollectionView.ItemsSource = ClientVm.Goals;

            MeasurablesCollectionView.ItemsSource = ClientVm.Measurables
                                                            .Where(measurable => measurable.GoalSuccession != Enums.Succession.Baseline)
                                                            .OrderBy(measurable => measurable.Variable)
                                                            .ThenByDescending(measurable => measurable.GoalSuccession)
                                                            .ThenBy(measurable => measurable.DateTaken);

            PhoneNumberPicker.ItemsSource   = ClientVm.PhoneNumbers;
            Title                           = ClientVm.Client.DisplayName;

            ColorValueLabel.TextColor       = ClientVm.Client.TextColor;
            ColorValueLabel.BackgroundColor = ClientVm.Client.BackgroundColor;

            FontColorPickerRadialMenu.CenterButtonBackgroundColor       = ColorValueLabel.TextColor;
            BackgroundColorPickerRadialMenu.CenterButtonBackgroundColor = ColorValueLabel.BackgroundColor;
        }

        private static string GetDisplayMessage(GoalViewModel goalVm)
        {
            var displayMessage = goalVm.GoalStatusDescription;
            if (goalVm.Goal.SuccessfullyCompleted)
            {
                displayMessage = $"Completed on {goalVm.GoalCompletedDateForDisplay}";
            }

            return displayMessage;
        }

        #region Events

        private void OnAccordionItemTapped(object sender, EventArgs e)
        {
            if (ClientAccordionItem.IsExpanded)
            {
                ClientAccordionItem.HeightRequest = -1;
                ClientNoteRte.HeightRequest       = -1;
            }
            else
            {
                ClientAccordionItem.HeightRequest = Application.Current.MainPage.Height;
                ClientNoteRte.HeightRequest       = Application.Current.MainPage.Height;
            }

            ClientAccordionItem.IsExpanded = ! ClientAccordionItem.IsExpanded;
        }

        private void SaveButton_OnClicked(object    sender
                                        , EventArgs e)
        {
            //ViewModel.Client.DisplayName = ClientNameEntry.Text;
            var displayName = ClientVm.Client.ToString();

            //TODO: Update properties unique to the Client class, not in Contact
            ClientVm.Client.Notes = ClientNoteRte.HtmlText;
            try
            {
                ClientVm.Save();
            }
            catch (Exception exception)
            {
                Logger.WriteLine("Client could not be saved"
                               , Category.Error
                               , exception
                               , $"Client: {displayName}");
            }

            PageNavigation.NavigateBackwards();
        }

        private void LinkContactToolbarItem_OnClicked(object    sender
                                                    , EventArgs e)
        {
            ClientVm.LinkContactToClient().ConfigureAwait(false);
        }

        private void ClientMainPhoneValueLabel_OnTapped(object    sender
                                                      , EventArgs e)
        {
            try
            {
                PhoneDialer.Open(ClientMainPhoneValueLabel.Text);
            }
            catch (FeatureNotSupportedException ex)
            {
                Logger.WriteLineToToastForced("This feature is not supported on your phone"
                                            , Category.Information
                                            , ex);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Problem while trying to dial number. "
                               , Category.Error
                               , ex
                               , ClientVm.Client.DisplayName);
            }
        }

        private void ClientMainPhoneLabel_OnTapped(object    sender
                                                 , EventArgs e)
        {
            
        }

        private void PhoneNumberPicker_OnOkButtonClicked(object                    sender
                                                       , SelectionChangedEventArgs e)
        {
            PhoneNumberPicker.IsOpen    = false;
            PhoneNumberPicker.IsVisible = false;

            var selectedNumber = PhoneNumberPicker.SelectedItem as PhoneNumber;
            
            ClientVm.SetNewMainNumber(selectedNumber);
            ClientMainPhoneValueLabel.Text = selectedNumber?.Number ?? "<main number not set>";
        }

        private void PhoneNumberPicker_OnCancelButtonClicked(object                    sender
                                                           , SelectionChangedEventArgs e)
        {
            PhoneNumberPicker.IsOpen    = false;
            PhoneNumberPicker.IsVisible = false;
        }

        private void AddNewGoalButton_OnClicked(object    sender
                                              , EventArgs e)
        {
            var instance = new GoalsAddEditPage(ClientId, "0");

            PageNavigation.NavigateTo(instance);
        }

        private void GoalsCollectionView_OnSelectionChanged(object                                  sender
                                                          , Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var goal = (GoalViewModel)e.CurrentSelection.FirstOrDefault();

            if (goal == null) return;

            var instance = new GoalsAddEditPage(ClientId
                                              , goal.Goal.Id.ToString());

            PageNavigation.NavigateTo(instance);
        }

        private void ChangeMainNumberButton_OnClicked(object    sender
                                                    , EventArgs e)
        {
            //Bring up picker of a list of number that are in the Client to reassign the main number
            PhoneNumberPicker.IsOpen    = true;
            PhoneNumberPicker.IsVisible = true;
        }

        private void AddNewMeasurableButton_OnClicked(object    sender
                                                    , EventArgs e)
        {
            var instance = new MeasurablesAddPage(ClientId
                                                , "0");
            PageNavigation.NavigateTo(instance);
        }

        private void MeasurablesCollectionView_OnSelectionChanged(object                                  sender
                                                                , Xamarin.Forms.SelectionChangedEventArgs e)
        {

        }

        private void TapGestureRecognizer_OnTapped_GoalStatusImage(object    sender
                                                 , EventArgs e)
        {
            var goalVm         = (GoalViewModel)((Image)sender).BindingContext;
            var displayMessage = GetDisplayMessage(goalVm);

            Navigation.ShowPopup(new ToolTipPopup(displayMessage));
        }

        private async void ColorValueLabel_OnTapped(object    sender
                                                  , EventArgs e)
        {
            ColorPickersGrid.IsVisible = ! ColorPickersGrid.IsVisible;
            ColorValueLabel.Text       = ColorValueLabel.Text == TapToChange
                                                            ? TapWhenDone
                                                            : TapToChange;

            // var clientColorsPopup = new ClientColors(new ClientListViewModel());
            //
            // await Navigation.ShowPopupAsync(clientColorsPopup)
            //                 .ContinueWith(SetClientColors
            //                             , TaskContinuationOptions.OnlyOnRanToCompletion);
            //
            // return;
            //
            // void SetClientColors(Task<object> _)
            // {
            //     Device.BeginInvokeOnMainThread(() =>
            //     {
            //         ColorValueLabel.TextColor       = clientColorsPopup.ClientColorsVm.SelectedTextColor;
            //         ColorValueLabel.BackgroundColor = clientColorsPopup.ClientColorsVm.SelectedBackgroundColor;
            //     });
            //
            //     ClientVm.Client.BackgroundColorHex = clientColorsPopup.ClientColorsVm.SelectedBackgroundColor.ToHex();
            //     ClientVm.Client.TextColorHex       = clientColorsPopup.ClientColorsVm.SelectedTextColor.ToHex();
            // }
        }

        private void SfRadialMenuItemFont_OnItemTapped(object              sender
                                                 , ItemTappedEventArgs e)
        {
            var selectedColor = ((SfRadialMenuItem)sender).BackgroundColor;
            FontColorPickerRadialMenu.CenterButtonBackgroundColor = selectedColor;
            ColorValueLabel.TextColor                             = selectedColor;
            ClientVm.Client.TextColorHex                          = selectedColor.ToHex();
        }

        private void SfRadialMenuItemBackGround_OnItemTapped(object              sender
                                                           , ItemTappedEventArgs e)
        {
            var selectedColor = ((SfRadialMenuItem)sender).BackgroundColor;
            BackgroundColorPickerRadialMenu.CenterButtonBackgroundColor = selectedColor;
            ColorValueLabel.BackgroundColor                             = selectedColor;
            ClientVm.Client.BackgroundColorHex                          = selectedColor.ToHex();
        }

        #endregion //Events

    }
}
