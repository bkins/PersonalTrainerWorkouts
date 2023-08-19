using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avails.ApplicationExceptions;
using Avails.D_Flat.Extensions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;
using PersonalTrainerWorkouts.ViewModels.HelperClasses;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SelectionChangedEventArgs = Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;

namespace PersonalTrainerWorkouts.Views.Tab_Clients;

[QueryProperty(nameof(ClientId)
             , nameof(ClientId))]
[QueryProperty(nameof(GoalId)
             , nameof(GoalId))]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MeasurablesAddPage : ContentPage, IQueryAttributable
{
    public MeasurablesViewModel MeasurablesViewModel { get; set; }

    // ReSharper disable InconsistentNaming
    private string _clientId { get; set; }
    public string ClientId
    {
        get => _clientId;
        set => _clientId = value;
    }

    private string _goalId { get; set; }
    public string GoalId
    {
        get => _goalId;
        set => _goalId = value;
    }

    public MeasurablesAddPage()
    {
        InitializeComponent();
       // MeasurablesViewModel = new MeasurablesViewModel();
    }

    public MeasurablesAddPage(string clientId, string goalId)
    {
        ClientId = clientId;
        GoalId   = goalId;
    }
    public void ApplyQueryAttributes(IDictionary<string, string> query)
    {
        try
        {
            ClientId             = HttpUtility.UrlDecode(query[nameof(ClientId)]);
            GoalId               = HttpUtility.UrlDecode(query[nameof(GoalId)]);
            MeasurablesViewModel = new MeasurablesViewModel(GoalId.ToSafeInt(), 0); //TODO: If this page is used to edit Measurables then I
                                                                                      //      will need to pass in the MeasurableId
            if (MeasurablesViewModel.Measurables
                                    .All(measurable => measurable.GoalSuccession != Enums.Succession.Target))
            {
                DisplayAlert("Create a Target first"
                           , "Before you take any measurements for this goal, we need to set a target."
                           , "Ok");
                MeasurablesViewModel.NewMeasurable ??= new Measurable();
                MeasurablesViewModel.NewMeasurable.GoalSuccession = Enums.Succession.Target;

                Title = "Target";
            }

            BindingContext = MeasurablesViewModel;
        }
        catch (Exception e)
        {
            Logger.WriteLine($"Failed initiate {nameof(MeasurablesAddPage)}."
                           , Category.Error
                           , e);
            throw;
        }
    }

    protected override void OnAppearing()
    {
        DisplayAlert("Under Construction"
                   , "There is a problem with Measurables. It is broken in this version. It will be fixed in the next update."
                   , "OK");

        DateTakenPicker.Date = DateTakenPicker.Date == DateTime.MinValue ? DateTime.Today : DateTakenPicker.Date;
        base.OnAppearing();
    }

    private void LoadClient(string value)
    {
        
    }

    private void LoadGoal(string value)
    {
        
    }

    private void MaxRadioButton_OnCheckedChanged(object                  sender
                                               , CheckedChangedEventArgs e)
    {
        
    }

    private void MeasurementRadioButton_OnCheckedChanged(object                  sender
                                                       , CheckedChangedEventArgs e)
    {
        
    }

    private void SaveButton_OnClicked(object    sender
                                    , EventArgs e)
    {
        DisplayAlert("Under Construction"
                   , "There is a problem with Measurables. It is broken in this version. It will be fixed in the next update."
                   , "OK");
        return;

        try
        {
            var continueBack = true;
            if (MeasurablesViewModel.GoalSuccession == Enums.Succession.Interim)
            {
                continueBack = SetInterimMeasurableFromPageFields();
            }

            if (MeasurablesViewModel.GoalSuccession == Enums.Succession.Target)
            {
                SetTargetMeasurableFromPageFields();
            }

            // MeasurablesViewModel.ValidateMeasurable();
            // MeasurablesViewModel.Save();
            if (continueBack)
            {
                PageNavigation.NavigateBackwards();
            }
        }
        catch (InvalidMeasurable invalidMeasurable)
        {
            Logger.WriteLineToToastForced("Measurable was not saved. See Logs for details."
                                        , Category.Error
                                        , invalidMeasurable.StackTrace
                                        , invalidMeasurable);
        }
        catch (Exception exception)
        {
            Logger.WriteLine("Error while saving Measurable."
                           , Category.Error
                           , exception
                           , exception.StackTrace);
        }
    }

    private bool SetInterimMeasurableFromPageFields()
    {
        var reasonCouldNotSave = MeasurablesViewModel.AddInterimMeasurable(null
                                                                         , ValueEditor.Text.ToSafeDouble()
                                                                         , DateTakenPicker.Date
                                                                         , GetMeasurableType()
                                                                         , VariableEditor.Text
                                                                         , ClientId.ToSafeInt()
                                                                         , "Pounds"
                                                                         , GoalId.ToSafeInt());

        if ( reasonCouldNotSave.Reason.IsNullEmptyOrWhitespace()) return true;

        DisplayAlert("Could not save"
                   , reasonCouldNotSave.Reason
                   , "Ok");
        VariableEditor.Text = reasonCouldNotSave.VariableShouldBeUsed;

        return false;
    }
    private void SetTargetMeasurableFromPageFields()
    {
        MeasurablesViewModel.AddBaselineMeasurable(VariableEditor.Text
                                                 , DateTakenPicker.Date
                                                 , GetMeasurableType()
                                                 , ClientId.ToSafeInt()
                                                 , "pounds"); //TODO: Implement UOM Picker
        MeasurablesViewModel.AddTargetMeasurable(MeasurablesViewModel.NewMeasurable
                                                , ValueEditor.Text.ToSafeDouble());
    }
    private string GetMeasurableType()
    {
        if (MaxRadioButton.IsChecked)
        {
            return "Max";
        }

        if (MeasurementRadioButton.IsChecked)
        {
            return "Measurement";
        }

        return string.Empty;
    }

    private void DateTakenPicker_OnDateSelected(object               sender
                                              , DateChangedEventArgs e)
    {

    }

    private void TapGestureRecognizer_OnTapped_UomEditor(object    sender
                                                       , EventArgs e)
    {
        UnitOfMeasurementPicker.IsOpen = true;
    }


    private void UnitOfMeasurementPicker_OnOkButtonClicked(object                    sender
                                                         , SelectionChangedEventArgs e)
    {
        UnitOfMeasurementEditor.Text   = UnitOfMeasurementPicker.SelectedItem.ToString();
        UnitOfMeasurementPicker.IsOpen = false;
    }

    private void UnitOfMeasurementPicker_OnCancelButtonClicked(object                    sender
                                                             , SelectionChangedEventArgs e)
    {
        UnitOfMeasurementPicker.IsOpen = false;
    }

    private void TapGestureRecognizer_OnTapped_UomLabel(object    sender
                                             , EventArgs e)
    {
        UnitOfMeasurementPicker.IsOpen = true;
    }
}
