using PersonalTrainerWorkouts.Views;
using Xamarin.Forms;
using WorkoutExercisePage = PersonalTrainerWorkouts.Views.WorkoutExercisePage;

namespace PersonalTrainerWorkouts
{
    public partial class AppShell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(WorkoutListPage)
                                , typeof(WorkoutListPage));

            Routing.RegisterRoute(nameof(WorkoutEntryPage)
                                , typeof(WorkoutEntryPage));

            Routing.RegisterRoute(nameof(ExerciseAddEditPage)
                                , typeof(ExerciseAddEditPage));

            Routing.RegisterRoute(nameof(ExerciseExistingEntryPage)
                                , typeof(ExerciseExistingEntryPage));

            Routing.RegisterRoute(nameof(ExerciseListPage)
                                , typeof(ExerciseListPage));

            // Routing.RegisterRoute(nameof(MessageLog)
            //                     , typeof(MessageLog));

            Routing.RegisterRoute(nameof(Avails.Xamarin.Views.LoggingPage.MessageLog)
                                , typeof(Avails.Xamarin.Views.LoggingPage.MessageLog));

            Routing.RegisterRoute(nameof(WorkoutExercisePage)
                                , typeof(WorkoutExercisePage));

            Routing.RegisterRoute(nameof(TypeOfExerciseEntryPage)
                                , typeof(TypeOfExerciseEntryPage));

            Routing.RegisterRoute(nameof(TypeOfExerciseListPage)
                                , typeof(TypeOfExerciseListPage));

            Routing.RegisterRoute(nameof(EquipmentListPage)
                                , typeof(EquipmentListPage));

            Routing.RegisterRoute(nameof(MuscleGroupListPage)
                                , typeof(MuscleGroupListPage));
            
            Routing.RegisterRoute(nameof(SessionListPage), typeof(SessionListPage));
            Routing.RegisterRoute(nameof(SessionEditPage), typeof(SessionEditPage));
            
            Routing.RegisterRoute(nameof(ClientListPage), typeof(ClientListPage));
            Routing.RegisterRoute(nameof(ClientEditPage), typeof(ClientEditPage));
        }
    }
}
