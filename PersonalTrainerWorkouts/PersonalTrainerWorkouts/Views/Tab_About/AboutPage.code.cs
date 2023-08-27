using System;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using Avails.Xamarin.Utilities;
using PersonalTrainerWorkouts.Utilities;
using PersonalTrainerWorkouts.ViewModels.Tab_About;
using Xamarin.Forms;
using static Avails.Xamarin.Configuration;

namespace PersonalTrainerWorkouts.Views.Tab_About;

public partial class AboutPage
{
    public  AboutViewModel AboutViewModel { get; set; }
    private Updater        AppUpdater     { get; set; }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        AppUpdater = new Updater(App.InternetData);

        AutomaticallyCheckForUpdatesCheckbox.IsChecked = AutomaticallyCheckForUpdates;
    }

    private void TableHeaderLabel_OnTapped(object sender, EventArgs eventArgs)
    {
        TableLabelScrollView.IsVisible = ! TableLabelScrollView.IsVisible;
    }

    private async void CheckForUpdates_OnButtonClicked(object    sender
                                                     , EventArgs e)
    {
        var isThereAnUpdate = await AppUpdater.IsThereAnUpdate()
                                              .ConfigureAwait(false);
        if (isThereAnUpdate)
        {
            Device.BeginInvokeOnMainThread(AskToUpdate);
        }
    }

    private async void AskToUpdate()
    {
        var update = await DisplayAlert($"New Version: {AppUpdater.ReleaseInfo?.TagName}"
                                      , "There is a new version.  Would you like to install it?"
                                      , "Yes"
                                      , "No").ConfigureAwait(false);

        if (! update) return;

        // Logger.WriteLineToToastForced("Downloading and installing... please be patient."
        //                             , Category.Information);
        // await DisplayAlert("Updating"
        //                  , "The latest version of the app is ready to download. "
        //                  + "You will be redirected to your browser to download the update. "
        //                  + "Please follow the instructions to complete the update."
        //                  , "OK").ConfigureAwait(false);
        await AppUpdater.Update();
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

    private void ReleaseNotesButtonOnClicked(object    sender
                                           , EventArgs e)
    {
        var instance = new ReleaseBodyPage(AboutViewModel.CurrentBuild
                                         , AboutViewModel.CurrentVersion);
        PageNavigation.NavigateTo(instance);
    }


}
