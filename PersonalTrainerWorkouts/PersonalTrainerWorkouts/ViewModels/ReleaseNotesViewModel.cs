using Avails.Xamarin.Utilities;

namespace PersonalTrainerWorkouts.ViewModels;

public class ReleaseNotesViewModel : ViewModelBase
{
    private string BuildNumber   { get; set; }
    private string VersionNumber { get; set; }

    public  string ReleaseNotes  { get; set; }

    public ReleaseNotesViewModel(string buildNumber, string versionNumber)
    {
        BuildNumber   = buildNumber;
        VersionNumber = versionNumber;

        SetReleaseNotes();
    }

    private async void SetReleaseNotes()
    {
        ReleaseNotes = await Updater.GetReleaseNotesByBuildAndVersion(BuildNumber
                                                               , VersionNumber)
                                    .ConfigureAwait(false);
    }
}
