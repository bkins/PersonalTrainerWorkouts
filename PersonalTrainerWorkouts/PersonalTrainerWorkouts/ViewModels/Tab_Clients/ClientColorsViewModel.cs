using System.Collections.Generic;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.ViewModels.DeleteThese;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.ViewModels.Tab_Clients;

public class ClientColorsViewModel : BaseViewModel
{
    private Color _selectedTextColor;
    private Color _selectedBackgroundColor;

    public Color SelectedTextColor
    {
        get => _selectedTextColor;
        set => SetProperty(ref _selectedTextColor
                         , value);
    }

    public Color SelectedBackgroundColor
    {
        get => _selectedBackgroundColor;
        set => SetProperty(ref _selectedBackgroundColor
                         , value);
    }

    public IEnumerable<Client> Clients { get; }

    public ClientColorsViewModel(IEnumerable<Client> clients)
    {
        Clients = clients;
    }

    public void SetColors(Color textColor
                        , Color backgroundColor)
    {
        SelectedTextColor       = textColor;
        SelectedBackgroundColor = backgroundColor;
    }
}
