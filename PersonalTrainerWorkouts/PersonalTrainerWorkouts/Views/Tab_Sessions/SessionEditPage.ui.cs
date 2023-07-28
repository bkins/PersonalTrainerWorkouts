using Avails.Xamarin.Extensions;
using PersonalTrainerWorkouts.ViewModels.Tab_Sessions;
using Syncfusion.SfPicker.XForms;
using Syncfusion.XForms.ComboBox;
using Syncfusion.XForms.RichTextEditor;
using Xamarin.Forms;
using static Avails.Xamarin.Utilities.UiUtilities;

namespace PersonalTrainerWorkouts.Views.Tab_Sessions;

public partial class NewSessionEditPage : ContentPage
{
    public ContentPage      ContentPage           { get; set; }
    public Grid             ClientGrid            { get; set; }
    public ListView         ClientListView        { get; set; }
    public Label            ClientPickerLabel     { get; set; }
    public ImageButton      EditClientImageButton { get; set; }
    public Label            NameLabel             { get; set; }
    public SfPicker         ClientPicker          { get; set; }
    public Grid             DateGrid              { get; set; }
    public Label            StartDateLabel        { get; set; }
    public DatePicker       StartDatePicker       { get; set; }
    public TimePicker       StartTimePicker       { get; set; }
    public Label            EndDateLabel          { get; set; }
    public DatePicker       EndDatePicker         { get; set; }
    public TimePicker       EndTimePicker         { get; set; }
    public SfComboBox       WorkoutComboBox       { get; set; }
    public ImageButton      AddRemoveButton       { get; set; }
    public ToolbarItem      SaveToolbarItem       { get; set; }
    public Grid             NotesGrid             { get; set; }
    public SfRichTextEditor SessionNotesRtEditor  { get; set; }
    public SfPicker         WorkoutsPicker        { get; set; }

    private void InitializeComponent()
    {
        var resources   = new ContentPage();
        AddStackLayoutStyleToResources(resources);

        var itemTemplate = BuildItemTemplate();

        resources.Resources.Add("ItemTemplate"
                              , itemTemplate);

        ContentPage = new ContentPage
                      {
                          BindingContext = new SessionViewModel()
                        , Resources      = resources.Resources
                      };

        BuildToolbarItems();
        BuildClientGrid();
        BuildDateGrid();
        BuildNotesGrid();
        BuildWorkoutsPicker();
        BuildStackLayout();

        Content = ContentPage.Content;
    }

    private void BuildStackLayout()
    {
        var stackLayout = new StackLayout();
        stackLayout.Children.Add(ClientGrid);
        stackLayout.Children.Add(ClientPicker);
        stackLayout.Children.Add(DateGrid);
        stackLayout.Children.Add(WorkoutComboBox);
        stackLayout.Children.Add(NotesGrid);
        stackLayout.Children.Add(WorkoutsPicker);

        ContentPage.Content = stackLayout;
    }

    private void BuildWorkoutsPicker()
    {
        WorkoutsPicker = new SfPicker
                         {
                             HeaderText = "Workouts"
                           , ShowFooter = true
                           , IsVisible  = false
                         };
        WorkoutsPicker.ItemsSource         =  new SessionViewModel().ClientListViewModel.Clients;
        WorkoutsPicker.OkButtonClicked     += WorkoutsPicker_OnOkButtonClicked;
        WorkoutsPicker.CancelButtonClicked += WorkoutsPicker_OnCancelButtonClicked;
    }

    private void BuildNotesGrid()
    {
        NotesGrid = new Grid();
        NotesGrid.RowDefinitions.Add(NewRowDefinition(1, GridUnitType.Star));
        NotesGrid.ColumnDefinitions.Add(NewColumnDefinition(1, GridUnitType.Star));

        SessionNotesRtEditor = new SfRichTextEditor
                               {
                                   BackgroundColor   = Color.Beige
                                 , PlaceHolder       = "Session notes"
                                 , VerticalOptions   = LayoutOptions.FillAndExpand
                                 , HorizontalOptions = LayoutOptions.FillAndExpand
                               };
        // SessionNotesRtEditor.SetBinding(SfRichTextEditor.TextProperty
        //                               , "NewSession.StartDate.Date");
        // Grid.SetRow(SessionNotesRtEditor, 0);
        // Grid.SetColumn(SessionNotesRtEditor, 0);
        NotesGrid.Children.Add(SessionNotesRtEditor, new Point(0, 0));
    }

    private void BuildDateGrid()
    {
        DateGrid = new Grid();
        DateGrid.ColumnDefinitions.Add(NewColumnDefinition(50, GridUnitType.Star));
        DateGrid.ColumnDefinitions.Add(NewColumnDefinition(50, GridUnitType.Star));

        DateGrid.RowDefinitions.Add(NewRowDefinition(1, GridUnitType.Star));
        DateGrid.RowDefinitions.Add(NewRowDefinition(1, GridUnitType.Star));
        DateGrid.RowDefinitions.Add(NewRowDefinition(1, GridUnitType.Star));
        DateGrid.RowDefinitions.Add(NewRowDefinition(1, GridUnitType.Star));
        DateGrid.RowDefinitions.Add(NewRowDefinition(1, GridUnitType.Star));

        StartDateLabel = new Label
                         {
                             Text                  = "Start:"
                           , VerticalTextAlignment = TextAlignment.Center
                           , FontSize              = FontSizes<Label>.Medium
                         };
        // Grid.SetColumn(StartDateLabel, 0);
        // Grid.SetRow(StartDateLabel, 0);
        DateGrid.Children.Add(StartDateLabel, new Point(0, 0));

        StartDatePicker = new DatePicker { TextColor = Color.Black };
        // Grid.SetColumn(StartDatePicker, 1);
        // Grid.SetRow(StartDatePicker, 0);
        StartDatePicker.DateSelected += StartDatePickerOnDateSelected;
        DateGrid.Children.Add(StartDatePicker, new Point(0, 1));

        StartTimePicker = new TimePicker { TextColor = Color.Black };

        // Grid.SetColumn(StartTimePicker, 2);
        // Grid.SetRow(StartTimePicker, 0);
        DateGrid.Children.Add(StartTimePicker, new Point(1, 1));

        EndDateLabel = new Label
                       {
                           Text                  = "End:"
                         , VerticalTextAlignment = TextAlignment.Center
                         , FontSize              = FontSizes<Label>.Medium
                       };
        // Grid.SetColumn(EndDateLabel, 0);
        // Grid.SetRow(EndDateLabel, 1);
        DateGrid.Children.Add(EndDateLabel, new Point(0, 2));

        EndDatePicker = new DatePicker { TextColor = Color.Black };
        // Grid.SetColumn(EndDatePicker, 1);
        // Grid.SetRow(EndDatePicker, 1);
        DateGrid.Children.Add(EndDatePicker, new Point(0, 3));

        EndTimePicker = new TimePicker { TextColor = Color.Black };
        // Grid.SetColumn(EndTimePicker, 2);
        // Grid.SetRow(EndTimePicker, 1);
        DateGrid.Children.Add(EndTimePicker, new Point(1, 3));

        WorkoutComboBox = new SfComboBox
                          {
                              HeightRequest                    = 40
                            , DropDownItemHeight               = 50
                            , DisplayMemberPath                = "Name"
                            , MultiSelectMode                  = MultiSelectMode.Token
                            , TokensWrapMode                   = TokensWrapMode.Wrap
                            , EnableAutoSize                   = true
                            , IsSelectedItemsVisibleInDropDown = false
                            , Watermark                        = "Workouts for this session"
                            , DataSource                       = new SessionViewModel().WorkoutsCollection
                            , ItemsSource                      = new SessionViewModel().SelectedWorkouts
                          };
        WorkoutComboBox.SelectionChanged += WorkoutComboBox_OnSelectionChanged;
        // Grid.SetColumn(WorkoutComboBox, 0);
        // Grid.SetRow(WorkoutComboBox, 2);
        // Grid.SetColumnSpan(WorkoutComboBox, 3);
        DateGrid.Children.Add(WorkoutComboBox, new Point(0, 4), new Point(2, -1));
        DateGrid.Margin = new Thickness(10, 0, 10, 0);
    }


    private void BuildClientGrid()
    {
        ClientGrid = new Grid();
        ClientGrid.ColumnDefinitions
                  .Add(NewColumnDefinition(85, GridUnitType.Star));

        ClientGrid.ColumnDefinitions
                  .Add(NewColumnDefinition(15, GridUnitType.Star));

        ClientPickerLabel = new Label
                            {
                                HorizontalTextAlignment = TextAlignment.Start
                              , FontSize                = FontSizes<Label>.Medium
                            };
        //Text is set in LoadData() in the .code.cs:
        ClientPickerLabel.GestureRecognizers
                         .Add(new TapGestureRecognizer
                              {
                                  Command = new Command(SelectClient_OnTapped)
                              });

        EditClientImageButton = new ImageButton
                                {
                                    Source            = "edit_black_48.png"
                                  , HorizontalOptions = LayoutOptions.End
                                  , VerticalOptions   = LayoutOptions.Center
                                  , BackgroundColor   = Color.Transparent
                                  , HeightRequest     = 20
                                };
        EditClientImageButton.GestureRecognizers
                             .Add(new TapGestureRecognizer
                                  {
                                      Command = new Command(EditClientImageButton_Tapped)
                                  });

        ClientGrid.Margin = new Thickness(10, 0, 10, 0);

        ClientGrid.Children.AddColumn(ClientPickerLabel, 0);
        ClientGrid.Children.AddColumn(EditClientImageButton, 1);
        // Grid.SetColumn(ClientPickerLabel, 0);
        // ClientGrid.Children.Add(ClientPickerLabel);
        // Grid.SetColumn(EditClientImageButton, 1);
        // ClientGrid.Children.Add(EditClientImageButton);

        ContentPage.Content = ClientGrid;

        ClientPicker = new SfPicker
                       {
                           HeaderText   = "Clients"
                         , ShowFooter   = true
                         , IsVisible    = false
                         , PickerMode   = PickerMode.Default
                         , WidthRequest = 120
                       };
        Grid.SetColumnSpan(ClientPicker, 2);

        ClientPicker.OkButtonClicked     += ClientPicker_OnOkButtonClicked;
        ClientPicker.CancelButtonClicked += ClientPicker_OnCancelButtonClicked;
    }

    private void BuildToolbarItems()
    {
        SaveToolbarItem = new ToolbarItem
                          {
                              Text            = "Save"
                            , IconImageSource = "save_black_48.png"
                          };
        SaveToolbarItem.Clicked += SaveButton_OnClicked;
        ToolbarItems.Add(SaveToolbarItem);
    }

    private static void AddStackLayoutStyleToResources(ContentPage resources)
    {
        var editorStyle = new Style(typeof(Editor));

        editorStyle.Setters.Add(new Setter
                                {
                                    Property = BackgroundColorProperty
                                  , Value    = (Color)Application.Current.Resources["AppBackgroundColor"]
                                });
        resources.Resources = new ResourceDictionary { editorStyle };

        var stackLayoutStyle  = new Style(typeof(StackLayout));
        var visualStateGroups = new VisualStateGroupList();
        var commonStates      = new VisualStateGroup { Name = "CommonStates" };
        var normalState       = new VisualState      { Name = "Normal" };
        var selectedState     = new VisualState      { Name = "Selected" };
        var selectedStateBackgroundSetter = new Setter
                                            {
                                                Property = BackgroundColorProperty
                                              , Value    = (Color)Application.Current.Resources["AppPrimaryColor"]
                                            };

        selectedState.Setters.Add(selectedStateBackgroundSetter);
        commonStates.States.Add(normalState);
        commonStates.States.Add(selectedState);
        visualStateGroups.Add(commonStates);
        stackLayoutStyle.Setters.Add(new Setter
                                     {
                                         Property = VisualStateManager.VisualStateGroupsProperty
                                       , Value    = visualStateGroups
                                     });
        resources.Resources.Add(stackLayoutStyle);
    }

    private DataTemplate BuildItemTemplate()
    {
        var itemTemplate = new DataTemplate(() =>
        {
            var grid = new Grid();

            grid.ColumnDefinitions.Add(NewColumnDefinition(1, GridUnitType.Star));
            grid.ColumnDefinitions.Add(NewColumnDefinition(1, GridUnitType.Auto));

            NameLabel = new Label
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Medium
                                                         , typeof(Label))
                          , FontAttributes = FontAttributes.Bold
                          , Margin = new Thickness(10, 0, 0, 0)
                        };
            NameLabel.SetBinding(Label.TextProperty, "Name");
            // Grid.SetColumn(NameLabel, 0);
            grid.Children.AddColumn(NameLabel, 0);

            AddRemoveButton         =  new ImageButton { Source = "baseline_add_circle_outline_black_48.png" };
            AddRemoveButton.Clicked += AddRemoveButton_OnClicked;
            // Grid.SetColumn(AddRemoveButton, 1);
            grid.Children.AddColumn(AddRemoveButton, 1);

            return new ViewCell { View = grid };
        });

        return itemTemplate;
    }
}
