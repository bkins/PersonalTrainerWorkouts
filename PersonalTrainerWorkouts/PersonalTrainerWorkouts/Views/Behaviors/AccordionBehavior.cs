using Syncfusion.XForms.Accordion;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views.Behaviors
{
    public class AccordionBehavior : Behavior<SfAccordion>
    {
        private SfAccordion _accordion;
        
        protected override void OnAttachedTo(SfAccordion bindable)
        {
            _accordion           =  bindable;
            _accordion.Expanded  += Bindable_Expanded;
            _accordion.Collapsed += Bindable_Collapsed;
            base.OnAttachedTo(bindable);
        }
 
        private void Bindable_Expanded(object sender, ExpandedAndCollapsedEventArgs e)
        {
            var index = e.Index;
            var item  = _accordion.Items[index];
            
            switch (item.ClassId)
            {
                case "DbFunctions":

                    App.Current.MainPage.DisplayAlert("Waring!"
                                                    , "Use these features at your own risk"
                                                    , "OK");
                    break;
                
                case "Item1":

                    App.Current.MainPage.DisplayAlert("Information", "Accordion Item1 Expanded", "Ok");

                    break;

                case "Item2":

                    App.Current.MainPage.DisplayAlert("Information", "Accordion Item2 Expanded", "Ok");

                    break;

                case "Item3":

                    App.Current.MainPage.DisplayAlert("Information", "Accordion Item3 Expanded", "Ok");

                    break;

                case "Item4":

                    App.Current.MainPage.DisplayAlert("Information", "Accordion Item4 Expanded", "Ok");

                    break;
            }
        }
 
        private void Bindable_Collapsed(object sender, ExpandedAndCollapsedEventArgs e)
        {
        }
 
        protected override void OnDetachingFrom(SfAccordion bindable)
        {
            _accordion.Expanded  -= Bindable_Expanded;
            _accordion.Collapsed -= Bindable_Collapsed;
            _accordion           =  null;
            base.OnDetachingFrom(bindable);
        }
    }
}