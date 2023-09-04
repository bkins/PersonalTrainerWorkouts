using System;
using Avails.Xamarin.Extensions;
using Avails.Xamarin.Logger;
using Avails.Xamarin.Utilities;
using PersonalTrainerWorkouts.ViewModels.Tab_About;
using static Xamarin.Forms.Color;

namespace PersonalTrainerWorkouts.Views.Tab_About;

using Xamarin.Forms;

public partial class AboutPage : ContentPage
{
    public Grid        MainGrid;
    public Image       Image;
    public StackLayout ContentStackLayout;
    public Label       TableHeaderLabel;
    public Label       CopyToClipboardLabel;
    public ScrollView  TableLabelScrollView;
    public Editor      TableEditor;
    public Button      CheckForUpdatesButton;
    public Button      ReleaseNotesButton                        { get; set; }
    public Grid        CheckboxGrid                              { get; set; }
    public CheckBox    AutomaticallyCheckForUpdatesCheckbox      { get; set; }
    public Label       AutomaticallyCheckForUpdatesCheckboxLabel { get; set; }
    public Grid        VersionInfoGrid                           { get; set; }
    public Label       CurrentBuildValue                         { get; set; }
    public Label       CurrentVersionValue                       { get; set; }
    public Label       CurrentBuildLabel                         { get; set; }
    public Label       CurrentVersionLabel                       { get; set; }

    public AboutPage()
    {
        Title          = "About";
        AboutViewModel = new AboutViewModel();

        CreateUi();
    }

    private void CreateContentStackLayout()
    {
        ContentStackLayout = new StackLayout
                             {
                                 Margin  = new Thickness(20),
                                 Spacing = 10
                             };

        CreateTableHeaderLabel(); // Move the TableHeaderLabel here
        ContentStackLayout.Children.Add(CheckForUpdatesButton);
        ContentStackLayout.Children.Add(CheckboxGrid);
        ContentStackLayout.Children.Add(ReleaseNotesButton);
        ContentStackLayout.Children.Add(VersionInfoGrid);

        // Move the TableLabelScrollView here, after it has been created
        ContentStackLayout.Children.Add(TableLabelScrollView);

        Grid.SetRow(ContentStackLayout, 1); // Set the Grid.Row property for the StackLayout

        MainGrid.Children.Add(ContentStackLayout); // Add the StackLayout to the Grid
    }

    private void CreateUi()
    {
        MainGrid = new Grid
                   {
                       RowDefinitions =
                       {
                           // Version Info row
                           new RowDefinition { Height = GridLength.Auto }
                         , // Content row
                           new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                         , // Table Info row
                           new RowDefinition { Height = GridLength.Auto }
                         , // "Table List" row
                           new RowDefinition { Height = GridLength.Auto }
                       }
                   };
        //NotesGrid.ColumnDefinitions.Add(NewColumnDefinition(1, GridUnitType.Star));
        MainGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));
        MainGrid.ColumnDefinitions.Add(UiUtilities.NewColumnDefinition(GridLength.Star));

        CreateVersionInformation();
        CreateGetForUpdatesButton();
        CreateAutomaticallyCheckForUpdatesCheckbox();
        CreateReleaseNotesButton();
        CreateTableLabelScrollView();
        CreateCopyToClipboardLabel();

        CreateContentStackLayout();

        Grid.SetRow(VersionInfoGrid, 0); // Put VersionInfoGrid at the top of the page ("page "header")
        Grid.SetRow(ContentStackLayout, 1); // The ContentStackLayout will have the "body" of the page.
                                            // Put all (most) buttons, checkboxes, other info here.

        //This is for the Table List at the bottom of the page.  This mostly for verifying that tables are setup correctly.
        //This can be removed or hidden in Prod release.
        Grid.SetRow(TableLabelScrollView, 2); // Set the Grid.Row property for the TableLabelScrollView
        Grid.SetRow(TableHeaderLabel, 3); // Set the Grid.Row property for the "Table List" label

        MainGrid.Children.Add(VersionInfoGrid, 0, 2, 0, 1); //);
        MainGrid.Children.Add(ContentStackLayout, 0, 2, 1, 2); //, 0, 1);
        MainGrid.Children.Add(TableLabelScrollView, 0, 2, 2, 3); //, 0, 2);
        MainGrid.Children.Add(TableHeaderLabel, 0, 2); //, 0, 3); // Add "Table List" label at the bottom row
        MainGrid.Children.Add(CopyToClipboardLabel, 1, 2); //, 1, 3);

        Content = MainGrid;
    }

    private void CreateImage()
    {
        Image = new Image
        {
            Source = "Running.ico",
            BackgroundColor = FromHex("#2196F3"),
            VerticalOptions = LayoutOptions.Center,
            HeightRequest = 64
        };
    }

    private void CreateCopyToClipboardLabel()
    {
        CopyToClipboardLabel = new Label
                               {
                                   Text                    = "Tap here to copy"
                                 , HorizontalOptions       = LayoutOptions.Start
                                 , HorizontalTextAlignment = TextAlignment.Start
                               };
        var tapGestureRecognizer = new TapGestureRecognizer();

        tapGestureRecognizer.Tapped += OnTapGestureRecognizerOnTapped;

        CopyToClipboardLabel.GestureRecognizers.Add(tapGestureRecognizer);
    }


    private void CreateTableHeaderLabel()
    {
        TableHeaderLabel = new Label
                           {
                               Text            = "Table List"
                             , FontAttributes  = FontAttributes.Bold
                             , VerticalOptions = LayoutOptions.FillAndExpand
                           };
        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += (s, e) =>
        {
            AboutViewModel.IsTableLabelScrollViewVisible = ! AboutViewModel.IsTableLabelScrollViewVisible;
            TableLabelScrollView.IsVisible               = AboutViewModel.IsTableLabelScrollViewVisible;
            CheckForUpdatesButton.IsVisible              = ! AboutViewModel.IsTableLabelScrollViewVisible;
            CheckboxGrid.IsVisible                       = ! AboutViewModel.IsTableLabelScrollViewVisible;
            ReleaseNotesButton.IsVisible                 = ! AboutViewModel.IsTableLabelScrollViewVisible;
        };

        TableHeaderLabel.GestureRecognizers.Add(tapGestureRecognizer);
    }
    private void CreateTableLabelScrollView()
    {
        TableLabelScrollView = new ScrollView
                               {
                                   IsVisible         = false,
                                   VerticalOptions   = LayoutOptions.FillAndExpand,
                                   HorizontalOptions = LayoutOptions.FillAndExpand,
                                   Margin            = new Thickness(20)
                               };

        TableEditor = new Editor
                          {
                              BackgroundColor = Color.Transparent
                            , IsReadOnly      = false
                            , FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Editor))
                            , Text = string.Join(Environment.NewLine
                                               , AboutViewModel.TableList)
                          };

        TableLabelScrollView.Content = TableEditor;

    }
    private async void OnTapGestureRecognizerOnTapped(object    sender
                                                          , EventArgs e)
    {
        await UiUtilities.SetClipboardValueAsync(TableEditor.Text).ConfigureAwait(false);

        Logger.WriteLineToToastForced("Copied to clipboard."
                                    , Category.Information);
    }
    private void CreateGetForUpdatesButton()
    {
        CheckForUpdatesButton = new Button
                                {
                                    Text = "Check for updates"
                                };
        CheckForUpdatesButton.Clicked += CheckForUpdates_OnButtonClicked;
    }

    private void CreateAutomaticallyCheckForUpdatesCheckbox()
    {
        AutomaticallyCheckForUpdatesCheckbox = new CheckBox
                                               {
                                                   IsChecked       = false
                                                 , VerticalOptions = LayoutOptions.Center
                                               };
        AutomaticallyCheckForUpdatesCheckbox.CheckedChanged += AutomaticallyCheckForUpdatesCheckbox_OnCheckedChanged;
        AutomaticallyCheckForUpdatesCheckboxLabel = new Label
                                                    {
                                                        Text            = "Automatically Check For Updates"
                                                      , VerticalOptions = LayoutOptions.Center
                                                    };
        CheckboxGrid = new Grid
                            {
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = GridLength.Auto }
                                  , new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                                }
                            };

        // Add the label and checkbox to the checkboxGrid
        CheckboxGrid.Children.Add(AutomaticallyCheckForUpdatesCheckboxLabel);
        CheckboxGrid.Children.Add(AutomaticallyCheckForUpdatesCheckbox, 1, 0);
    }

    private void CreateReleaseNotesButton()
    {
        ReleaseNotesButton = new Button
                             {
                                 Text = "Release Notes"
                             };
        ReleaseNotesButton.Clicked += ReleaseNotesButtonOnClicked;
    }

    private void CreateVersionInformation()
    {
        // Create the Version Information Grid
        VersionInfoGrid = new Grid
                          {
                              RowDefinitions =
                              {
                                  new RowDefinition { Height = GridLength.Auto }
                                , new RowDefinition { Height = GridLength.Auto }
                              }
                            , ColumnDefinitions =
                              {
                                  new ColumnDefinition { Width = GridLength.Auto }
                                , new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                              }
                            , Margin          = new Thickness(0, 20, 0, 0)
                            , VerticalOptions = LayoutOptions.FillAndExpand
                          };

        CurrentVersionLabel = new Label
                              {
                                  Text     = "Current Version:"
                                , FontSize = 18
                              };

        CurrentBuildLabel = new Label
                                {
                                    Text     = "Current Build:",
                                    FontSize = 18
                                };

        CurrentVersionValue = new Label
                              {
                                  Text     = AboutViewModel.CurrentVersion
                                , FontSize = 16
                              };

        CurrentBuildValue = new Label
                            {
                                Text     = AboutViewModel.CurrentBuild
                              , FontSize = 16
                            };

        VersionInfoGrid.Children.Add(CurrentVersionLabel, 0, 0);
        VersionInfoGrid.Children.Add(CurrentVersionValue, 1, 0);
        VersionInfoGrid.Children.Add(CurrentBuildLabel, 0, 1);
        VersionInfoGrid.Children.Add(CurrentBuildValue, 1, 1);
    }


}
