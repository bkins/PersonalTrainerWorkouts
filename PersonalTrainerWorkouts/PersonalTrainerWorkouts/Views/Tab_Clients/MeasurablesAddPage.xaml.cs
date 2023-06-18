﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avails.ApplicationExceptions;
using Avails.D_Flat.Extensions;
using Avails.Xamarin;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.Models.ContactsAndClients.Goals;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
    
    public void ApplyQueryAttributes(IDictionary<string, string> query)
    {
        try
        {
            ClientId             = HttpUtility.UrlDecode(query[nameof(ClientId)]);
            GoalId               = HttpUtility.UrlDecode(query[nameof(GoalId)]);
            MeasurablesViewModel = new MeasurablesViewModel(GoalId.ToSafeInt(), 0); //TODO: If this page is used to edit Measurables then I
                                                                                      //      will need to pass in the MeasurableId
            if (MeasurablesViewModel.Measurables
                                    .All(measurable => measurable.GoalSuccession != Succession.Target))
            {
                DisplayAlert("Create a Target first"
                           , "Before you take any measurements for this goal, we need to set a target."
                           , "Ok");
                MeasurablesViewModel.NewMeasurable ??= new Measurable();
                MeasurablesViewModel.NewMeasurable.GoalSuccession = Succession.Target;

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

        try
        {
            var continueBack = true;
            if (MeasurablesViewModel.GoalSuccession == Succession.Interim)
            {
                continueBack = SetInterimMeasurableFromPageFields();
            }

            if (MeasurablesViewModel.GoalSuccession == Succession.Target)
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
                                                                         , DateTakePicker.Date
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
                                                 , DateTakePicker.Date
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
}