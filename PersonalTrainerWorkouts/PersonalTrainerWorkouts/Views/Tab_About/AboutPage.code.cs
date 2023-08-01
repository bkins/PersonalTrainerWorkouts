using System;
using Avails.Xamarin.Logger;
using Avails.Xamarin.Utilities;
using PersonalTrainerWorkouts.ViewModels.Tab_About;
using Xamarin.Forms;
using static Avails.Xamarin.Configuration;

namespace PersonalTrainerWorkouts.Views.Tab_About;

public partial class AboutPage
{
    public AboutViewModel AboutViewModel { get; set; }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        AutomaticallyCheckForUpdatesCheckbox.IsChecked = AutomaticallyCheckForUpdates;
    }

    private void TableHeaderLabel_OnTapped(object sender, EventArgs eventArgs)
    {
        TableLabelScrollView.IsVisible = ! TableLabelScrollView.IsVisible;
    }

    private async void CheckForUpdates_OnButtonClicked(object    sender
                                                     , EventArgs e)
    {
        var isThereAnUpdate = await Updater.IsThereAnUpdate()
                                           .ConfigureAwait(false);
        if (isThereAnUpdate)
        {
            Device.BeginInvokeOnMainThread(AskToUpdate);
        }
    }

    private async void AskToUpdate()
    {
        var update = await DisplayAlert($"New Version: {Updater.ReleaseInfo.TagName}"
                                      , "There is a new version.  Would you like to install it?"
                                      , "Yes"
                                      , "No").ConfigureAwait(false);

        if (! update) return;

        Logger.WriteLineToToastForced("Downloading and installing... please be patient."
                                    , Category.Information);

        Updater.Update();
    }


    private void AutomaticallyCheckForUpdatesCheckbox_OnCheckedChanged(object                  sender
                                                                     , CheckedChangedEventArgs e)
    {
        AutomaticallyCheckForUpdates = AutomaticallyCheckForUpdatesCheckbox.IsChecked;
    }
    private string GetBuildName(string buildNumber)
    {
        return buildNumber switch
               {
                   "1" => "Alpha"
                 , "2" => "Beta"
                 , "3" => "RC"
                 , "4" => "Prod"
                 , _ => "Unknown"
               };
    }


}
