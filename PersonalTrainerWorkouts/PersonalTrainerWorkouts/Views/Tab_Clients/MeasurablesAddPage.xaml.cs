using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Avails.D_Flat.Extensions;
using Avails.Xamarin.Logger;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Tab_Clients;
[QueryProperty(nameof(ClientId)
             , nameof(ClientId))]
[QueryProperty(nameof(GoalId)
             , nameof(GoalId))]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MeasurablesAddPage : ContentPage
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
        set => LoadGoal(value);
    }

    public MeasurablesAddPage()
    {
        InitializeComponent();
    }
    
    public void ApplyQueryAttributes(IDictionary<string, string> query)
    {
        try
        {
            ClientId             = HttpUtility.UrlDecode(query[nameof(ClientId)]);
            GoalId               = HttpUtility.UrlDecode(query[nameof(GoalId)]);
            MeasurablesViewModel = new MeasurablesViewModel(GoalId.ToSafeInt());

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
}