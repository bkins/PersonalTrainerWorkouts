using System;
using System.Collections.Generic;
using PersonalTrainerWorkouts.Models.ContactsAndClients;
using PersonalTrainerWorkouts.ViewModels.Tab_Clients;
using Syncfusion.DataSource.Extensions;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PersonalTrainerWorkouts.Views.Popups;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ClientColors : Popup
{
    public Color ClientTextColor       { get; set; }
    public Color ClientBackgroundColor { get; set; }

    public IEnumerable<Client> Clients               { get; set; }

    public ClientColors(ClientListViewModel clientListViewModel)
    {
        try
        {
            InitializeComponent();
            Clients = clientListViewModel.Clients;

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

            foreach (var client in Clients)
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
        switch (element)
        {
            case Grid grid:
                return grid;

            case Layout<View> layout:
            {
                foreach (var child in layout.Children)
                {
                    var result = FindGrid(child);
                    if (result is not null)
                    {
                        return result;
                    }
                }

                break;
            }
        }

        return null;
    }

    private void SetColors(object sender)
    {
        var label = (Label)sender;
        ClientTextColor       = label.TextColor;
        ClientBackgroundColor = label.BackgroundColor;
        Dismiss("blah");
    }

    private void TapGestureRecognizer_OnTapped_Label00(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label01(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label02(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label03(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label04(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label10(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label11(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label12(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label13(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label14(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label20(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label21(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label22(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label23(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label24(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label30(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label31(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label32(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label33(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label34(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label40(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label41(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label42(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label43(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }

    private void TapGestureRecognizer_OnTapped_Label44(object    sender
                                                     , EventArgs e)
    {
        SetColors(sender);
    }
}
