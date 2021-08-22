
using PersonalTrainerWorkouts.Views;
using PersonalTrainerWorkouts.Views.Debugging;
using Xamarin.Forms;
using WorkoutExercisePage = PersonalTrainerWorkouts.Views.WorkoutExercisePage;

namespace PersonalTrainerWorkouts
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(InitialPage),               typeof(InitialPage));
            Routing.RegisterRoute(nameof(WorkoutEntryPage),          typeof(WorkoutEntryPage));
            Routing.RegisterRoute(nameof(ExerciseNewEntryPage),      typeof(ExerciseNewEntryPage));
            Routing.RegisterRoute(nameof(ExerciseExistingEntryPage), typeof(ExerciseExistingEntryPage));
            Routing.RegisterRoute(nameof(ExerciseListPage),          typeof(ExerciseListPage));
            Routing.RegisterRoute(nameof(MessageLog),                typeof(MessageLog));
            Routing.RegisterRoute(nameof(WorkoutExercisePage),       typeof(WorkoutExercisePage));
            Routing.RegisterRoute(nameof(WorkoutExerciseDebugPage),  typeof(WorkoutExerciseDebugPage));
            Routing.RegisterRoute(nameof(ExercisesDebugPage),        typeof(ExercisesDebugPage));
        }

    }
}
