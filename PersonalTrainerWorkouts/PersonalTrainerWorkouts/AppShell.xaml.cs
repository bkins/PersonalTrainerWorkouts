
using PersonalTrainerWorkouts.Models;
using PersonalTrainerWorkouts.Views;
using Xamarin.Forms;
using WorkoutExercisePage = PersonalTrainerWorkouts.Views.WorkoutExercisePage;

namespace PersonalTrainerWorkouts
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(WorkoutListPage),           typeof(WorkoutListPage));
            Routing.RegisterRoute(nameof(WorkoutEntryPage),          typeof(WorkoutEntryPage));
            Routing.RegisterRoute(nameof(ExerciseAddEditPage),       typeof(ExerciseAddEditPage));
            Routing.RegisterRoute(nameof(ExerciseExistingEntryPage), typeof(ExerciseExistingEntryPage));
            Routing.RegisterRoute(nameof(ExerciseListPage),          typeof(ExerciseListPage));
            Routing.RegisterRoute(nameof(MessageLog),                typeof(MessageLog));
            Routing.RegisterRoute(nameof(WorkoutExercisePage),       typeof(WorkoutExercisePage));
            Routing.RegisterRoute(nameof(ExerciseTypeEntryPage),     typeof(ExerciseTypeEntryPage));
        }

    }
}
