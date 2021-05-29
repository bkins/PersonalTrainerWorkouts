
using PersonalTrainerWorkouts.Views;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(InitialPage), typeof(InitialPage));
            Routing.RegisterRoute(nameof(WorkoutEntryPage), typeof(WorkoutEntryPage));
            Routing.RegisterRoute(nameof(ExerciseNewEntryPage), typeof(ExerciseNewEntryPage));
            Routing.RegisterRoute(nameof(ExerciseExistingEntryPage), typeof(ExerciseExistingEntryPage));
            Routing.RegisterRoute(nameof(ExerciseListPage), typeof(ExerciseListPage));
        }

    }
}
