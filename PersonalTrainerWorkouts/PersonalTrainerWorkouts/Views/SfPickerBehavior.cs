using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views
{
    public class SfPickerBehavior : Behavior<Syncfusion.SfPicker.XForms.SfPicker>
    {
        protected override void OnDetachingFrom(Syncfusion.SfPicker.XForms.SfPicker bindable)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                bindable.Dispose();
            }

            base.OnDetachingFrom(bindable);
        }
    }
}
