using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.App;
using Android.Widget;
using System.ComponentModel;
using PersonalTrainerWorkouts.Droid;
using PersonalTrainerWorkouts.Views.CustomControls;

[assembly: ExportRenderer(typeof(NullableDatePicker), typeof(NullableDatePickerRenderer))]
namespace PersonalTrainerWorkouts.Droid
{
    public class NullableDatePickerRenderer: ViewRenderer<NullableDatePicker, EditText>
    {
        private DatePickerDialog _dialog;
        
        protected override void OnElementChanged(ElementChangedEventArgs<NullableDatePicker> e)
        {
            base.OnElementChanged(e);

            SetNativeControl(new EditText(Android.App.Application.Context));
            
            if (Control is null 
             || e.NewElement is null)
                return;

            Control.Click       += OnPickerClick;
            Control.Text        =  Element.Date.ToString(Element.Format);
            Control.KeyListener =  null;
            Control.FocusChange += OnPickerFocusChange;
            Control.Enabled     =  Element.IsEnabled;

        }

        protected override void OnElementPropertyChanged(object                   sender
                                                       , PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender
                                        , e);

            if (e.PropertyName == Xamarin.Forms.DatePicker.DateProperty.PropertyName
             || e.PropertyName == Xamarin.Forms.DatePicker.FormatProperty.PropertyName)
            {
                SetDate(Element.Date);
            }
        }

        private void OnPickerFocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                ShowDatePicker();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Control != null)
            {
                Control.Click       -= OnPickerClick;
                Control.FocusChange -= OnPickerFocusChange;

                CleanupDialog();
            }

            base.Dispose(disposing);
        }

        private void CleanupDialog()
        {
            if (_dialog == null) return;
            
            _dialog.Hide();
            _dialog.Dispose();
            _dialog = null;
        }

        private void OnPickerClick(object    sender
                                 , EventArgs e)
        {
            ShowDatePicker();
        }

        private void SetDate(DateTime date)
        {
            Control.Text = date.ToString(Element.Format);
            Element.Date = date;
        }

        private void ShowDatePicker()
        {
            CreateDatePickerDialog(Element.Date.Year
                                 , Element.Date.Month - 1
                                 , Element.Date.Day);
            _dialog.Show();
        }

        private void CreateDatePickerDialog(int year, int month, int day)
        {
            var view = Element;

            _dialog = new DatePickerDialog(Context
                                         , (o, e) =>
                                           {
                                               view.Date = e.Date;
                                               ((IElementController)view).SetValueFromRenderer
                                               (
                                                   VisualElement.IsFocusedProperty
                                                 , false
                                               );
                                               Control.ClearFocus();
                                               _dialog = null;
                                           }
                                         , year
                                         , month
                                         , day);

            _dialog.SetButton("Done", (sender, e) =>
           {
               SetDate(_dialog.DatePicker.DateTime);
               Element.Format = Element.OriginalFormat;
               Element.AssignValue();
           });

            _dialog.SetButton2("Clear"
                             , (sender, e) =>
                               {
                                   Element.CleanDate();
                                   Control.Text = Element.Format;
                               });
        }
    }
}