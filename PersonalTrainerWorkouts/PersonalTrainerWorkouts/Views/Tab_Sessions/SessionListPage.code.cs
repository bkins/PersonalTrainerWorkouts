﻿using System;
using System.Threading.Tasks;
using Avails.D_Flat.Extensions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Models.HelperClasses;
using PersonalTrainerWorkouts.ViewModels.Tab_Sessions;
using Syncfusion.ListView.XForms;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SwipeEndedEventArgs = Syncfusion.ListView.XForms.SwipeEndedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Sessions;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SessionListPage : ContentPage
{
    public int SwipedItem { get; set; }

    public SessionListViewModel                      SessionList    { get; set; }
    //public ObservableCollection<ScheduleAppointment> Appointments { get; set; }

    public SessionListPage()
    {
        ShowBusyIndicator(true
                        , "Initialing...");

        InitializeComponents();

        //LoadViewModelData();
    }

    private void LoadViewModelData()
    {
        SessionList = new SessionListViewModel();

        SessionsSchedule.DataSource  = SessionList.Appointments;

        SessionsListView.ItemsSource = SessionList.ObservableListOfSessions;

        //Appointments = ViewModel.Appointments;
        //SessionsListView.ItemsSource = Appointments;

        //SessionsSchedule.DataSource = ViewModel.Appointments;
    }

    protected override void OnAppearing()
    {
        ShowBusyIndicator(true
                        , "Initialing...");
        LoadViewModelData();

        ShowBusyIndicator(false);
    }

    private void AddToolbarItem_OnClicked(object    sender
                                        , EventArgs e)
    {
        // var instance = new SessionEditPage(sessionId: "0");
        var instance = new NewSessionEditPage(sessionId: "0");
        PageNavigation.NavigateTo(instance);
        // PageNavigation.NavigateTo(nameof(SessionEditPage)
        //                         , nameof(SessionEditPage.SessionId)
        //                         , "0");
    }

    private void SearchToolbarItem_OnClicked(object    sender
                                           , EventArgs e)
    {
        SessionsFilter.IsVisible = ! SessionsFilter.IsVisible;
    }

    private void SessionsFilter_OnTextChanged(object               sender
                                            , TextChangedEventArgs e)
    {
        SessionsListView.ItemsSource = SessionList.SearchSessions(SessionsFilter.Text);
    }

    public void OnSelectionChanged(object                        sender
                                  , ItemSelectionChangedEventArgs e)
    {
        if (e.AddedItems == null)
            return;

        //The commented out line below returns the same as the line below it.
        //var item = (Session)e.AddedItems.FirstOrDefault();
        var item = (Session)((SfListView)sender).CurrentItem;
        if (item == null)
        {
            return;
        }

        // var instance = new SessionEditPage(item.Id.ToString());
        var instance = new NewSessionEditPage(item.Id.ToString());
        PageNavigation.NavigateTo(instance);

        // PageNavigation.NavigateTo(nameof(SessionEditPage)
        //                         , nameof(SessionEditPage.SessionId)
        //                         , item.Id.ToString());

        SessionsListView.SelectedItems.Clear();

    }

    public void ListView_SwipeEnded(object              sender
                                   , SwipeEndedEventArgs e)
    {
        SwipedItem = e.ItemIndex;
    }

    public void LeftImage_BindingContextChanged(object    sender
                                               , EventArgs e)
    {
        // UiUtilities.AddCommandToGesture(sender, Delete);
    }
    private void Delete()
    {
        var itemDeleted = SessionList.Delete(SwipedItem);

        if (!itemDeleted.success)
        {
            Logger.WriteLine("Session could not be deleted.  Please try again."
                           , Category.Warning);
        }

        SessionsListView.ItemsSource = SessionList.ObservableListOfSessions;

        Logger.WriteLine($"Deleted Session: {itemDeleted.item} deleted."
                       , Category.Information);

        SessionsListView.ResetSwipe();
    }

    private async void ShowBusyIndicator(bool show, string titleMessage = "")
    {
        if (!show)
            await Task.Delay(600);

        Device.BeginInvokeOnMainThread(() =>
        {
            if (titleMessage.HasValue()) BusyIndicator.Title = titleMessage;

            BusyIndicator.EnableAnimation = show;
            BusyIndicator.IsVisible       = show;
        });
    }

    private void SessionsSchedule_OnCellTapped(object              sender
                                             , CellTappedEventArgs e)
    {
        var s    = sender;
        var args = e;
    }

    private void SessionsScheduleOnMonthInlineAppointmentTapped(object                                sender
                                                              , MonthInlineAppointmentTappedEventArgs e)
    {
        var appointment     = (SessionAppointment)e.Appointment;
        // var sessionEditPage = new SessionEditPage(appointment.SessionId.ToString());
        var sessionEditPage = new NewSessionEditPage(appointment.SessionId.ToString());
        PageNavigation.NavigateTo(sessionEditPage);
    }

    private void ToggleListViewToolbarItem_OnClicked(object    sender
                                                   , EventArgs e)
    {
        var listViewIsVisualWhenClicked = SessionsListView.IsVisible;

        SessionsListView.IsVisible = ! listViewIsVisualWhenClicked;
        ShowAllToolbarItem.Text    = ! listViewIsVisualWhenClicked ? "All" : "";
        SessionsSchedule.IsVisible = listViewIsVisualWhenClicked;

        ToggleListViewToolbarItem.IconImageSource = listViewIsVisualWhenClicked
                                                        ? "segment_black_48.png"
                                                        : "calendar_month_black_48.png";
        SearchToolbarItem.IsEnabled = SessionsListView.IsVisible;
    }

    public void DeleteImage_OnTapped()
    {
        Delete();
    }

    private void ShowAllToolbarItem_OnClicked(object    sender
                                            , EventArgs e)
    {
        var allToolbarItem = sender as ToolbarItem;

        if (allToolbarItem?.Text == "All")
        {
            allToolbarItem.Text          = "Week";
            SessionsListView.ItemsSource = SessionList.ObservableListOfSessions;

            return;
        }
        allToolbarItem!.Text         = "All";
        SessionsListView.ItemsSource = SessionList.GetSessionsForTheWeek();
    }

    private void TestToolbarItem_OnClicked(object    sender
                                         , EventArgs e)
    {
        //PageNavigation.NavigateTo(nameof(TestSessionsPage));
        //PageNavigation.NavigateTo(nameof(AppointmentEditor));
        var instance = new NewSessionEditPage( "0");

        PageNavigation.NavigateTo(instance);
    }
}