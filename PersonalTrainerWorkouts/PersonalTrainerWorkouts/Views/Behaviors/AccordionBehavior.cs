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

                    Application.Current.MainPage.DisplayAlert("Waring!"
                                                    , "Use these features at your own risk"
                                                    , "OK");
                    break;
                
                case "Item1":

                    Application.Current.MainPage.DisplayAlert("Information", $"Accordion {item.ClassId} Expanded", "Ok");

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