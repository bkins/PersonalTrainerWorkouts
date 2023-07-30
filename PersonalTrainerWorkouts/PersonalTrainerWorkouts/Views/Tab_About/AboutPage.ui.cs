using static Xamarin.Forms.Color;

namespace PersonalTrainerWorkouts.Views.Tab_About;

using Xamarin.Forms;

public partial class AboutPage : ContentPage
{
    public Grid        MainGrid;
    public Image       Image;
    public StackLayout ContentStackLayout;
    public Label       TableHeaderLabel;
    public ScrollView  TableLabelScrollView;
    public Button      CheckForUpdatesButton;
    public Grid        CheckboxGrid                              { get; set; }
    public CheckBox    AutomaticallyCheckForUpdatesCheckbox      { get; set; }
    public Label       AutomaticallyCheckForUpdatesCheckboxLabel { get; set; }

    public AboutPage()
    {
        Title = "About";
        CreateUi();
    }

    private void CreateUi()
    {
        MainGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
            }
        };

        CreateImage();
        CreateContentStackLayout();

        MainGrid.Children.Add(Image);
        MainGrid.Children.Add(ContentStackLayout, 0, 1);

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

    private void CreateContentStackLayout()
    {
        ContentStackLayout = new StackLayout
                             {
                                 Margin  = new Thickness(20),
                                 Spacing = 20
                             };

        CreateTableHeaderLabel();
        CreateTableLabelScrollView();
        CreateGetForUpdatesButton();
        CreateAutomaticallyCheckForUpdatesCheckbox();

        Grid.SetRow(ContentStackLayout, 1); // Set the Grid.Row property for the StackLayout

        MainGrid.Children.Add(ContentStackLayout); // Add the StackLayout to the Grid

        ContentStackLayout.Children.Add(TableHeaderLabel);
        ContentStackLayout.Children.Add(TableLabelScrollView);
        ContentStackLayout.Children.Add(CheckForUpdatesButton);
        ContentStackLayout.Children.Add(CheckboxGrid);
    }


    private void CreateTableHeaderLabel()
    {
        TableHeaderLabel = new Label
        {
            Text = "Table Info"
        };
        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += TableHeaderLabel_OnTapped;

        TableHeaderLabel.GestureRecognizers.Add(tapGestureRecognizer);
    }

    private void CreateTableLabelScrollView()
    {
        TableLabelScrollView = new ScrollView
        {
            IsVisible = false,
            Content = new Label { Text = "Your table information goes here" }
        };
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
                                                   IsChecked = false
                                               };
        AutomaticallyCheckForUpdatesCheckbox.CheckedChanged += AutomaticallyCheckForUpdatesCheckbox_OnCheckedChanged;
        AutomaticallyCheckForUpdatesCheckboxLabel = new Label
                                               {
                                                   Text            = "Automatically Check For Updates",
                                                   VerticalOptions = LayoutOptions.CenterAndExpand
                                               };
        CheckboxGrid = new Grid
                            {
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = GridLength.Auto }
                                  , new ColumnDefinition
                                    {
                                        Width = new GridLength(1, GridUnitType.Star)
                                    }
                                }
                            };

        // Add the label and checkbox to the checkboxGrid
        CheckboxGrid.Children.Add(AutomaticallyCheckForUpdatesCheckboxLabel);
        CheckboxGrid.Children.Add(AutomaticallyCheckForUpdatesCheckbox, 1, 0);
    }
}
