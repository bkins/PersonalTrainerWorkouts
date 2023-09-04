using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace PersonalTrainerWorkouts.Views.Popups;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ClientColors : Popup
{
    // public Color                 ClientTextColor       { get; set; }
    // public Color                 ClientBackgroundColor { get; set; }
    public ClientColorsViewModel ClientColorsVm { get; }

    //public IEnumerable<Client>   Clients               { get; set; }

    public ClientColors(ClientListViewModel clientListViewModel)
    {
        try
        {
            InitializeComponent();
            //Clients = clientListViewModel.Clients;
            ClientColorsVm = new ClientColorsViewModel(clientListViewModel.Clients);
            BindingContext = ClientColorsVm;

            SetClientDisplayNameToColorsInUse();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void SetClientDisplayNameToColorsInUse()
    {
        var grid = FindGrid(Content);
        if (grid is null) return;

        foreach (var child in grid.Children)
        {
            if (child is not Label label) continue;

            foreach (var client in ClientColorsVm.Clients)
            {
                if (client.BackgroundColorHex == label.BackgroundColor.ToHex()
                 && client.TextColorHex == label.TextColor.ToHex())
                {
                    label.Text = client.DisplayName;
                }
            }
        }
    }

    private static Grid FindGrid(Element element)
    {
        Layout<View> layout     = null;
        ScrollView   scrollView = null;

        switch (element)
        {
            case Grid grid:
                return grid;

            case Layout<View> layoutElement:
                layout = layoutElement;
                break;

            case ScrollView scrollViewElement:
                scrollView = scrollViewElement;
                break;
        }

        var children = new List<View>();

        if (layout != null)                      { children.AddRange(layout.Children); }
        if (scrollView is { Content: not null }) { children.Add(scrollView.Content); }

        return (from child in children.Where(child => child is not Label)
                where child is not Label
                select FindGrid(child)).FirstOrDefault(result => result is not null);
    }

    private void SetColors(object sender)
    {
        var label = (Label)sender;
        ClientColorsVm.SetColors(label.TextColor, label.BackgroundColor);
        Dismiss("blah");
    }

    private void TapGestureRecognizer_OnTapped(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private async void OpenFontColorPickerDialog(object    sender
                                               , EventArgs e)
    {

    }

    private void OpenBackGroundColorPickerDialog(object    sender
                                               , EventArgs e)
    {

    }

    private void OKButton_Clicked(object    sender
                                , EventArgs e)
    {

    }

    private async void FontColorPicker_OnClicked(object    sender
                                               , EventArgs e)
    {


    }
}
