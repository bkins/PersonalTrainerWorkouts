using Syncfusion.ListView.XForms;
using Syncfusion.SfSchedule.XForms;
using Xamarin.Forms;
using Syncfusion.SfBusyIndicator.XForms;
using Avails.Xamarin.Controls.CurvedCornersLabel;
using SelectionMode = Syncfusion.SfSchedule.XForms.SelectionMode;
using XForm = Syncfusion.ListView.XForms;
namespace PersonalTrainerWorkouts.Views.Tab_Sessions;

public partial class SessionListPage //UI Components
{
    private readonly ColumnDefinition _columnDefinitionStar = new() { Width  = GridLength.Star };
    private readonly RowDefinition    _rowDefinition        = new() { Height = GridLength.Auto };

    public ToolbarItem     ShowAllToolbarItem          { get; set; }
    public ToolbarItem     TestToolbarItem              { get; set; }
    public ToolbarItem     AddToolbarItem              { get; set; }
    public ToolbarItem     SearchToolbarItem           { get; set; }
    public ToolbarItem     ToggleListViewToolbarItem   { get; set; }
    public Entry           SessionsFilter              { get; set; }
    public Grid            ScheduleViewRadioButtonGrid { get; set; }
    public Grid            SessionsScheduleGrid        { get; set; }
    public SfSchedule      SessionsSchedule            { get; set; }
    public DataTemplate    LeftSwipeTemplate           { get; set; }
    public SfListView      SessionsListView            { get; set; }
    public SfBusyIndicator BusyIndicator               { get; set; }

    private void InitializeComponents()
    {
        Title = "Sessions";

        InitializeToolbarItems();
        InitializeSessionsFilter();
        InitializeSessionScheduleAsCalendar();
        InitializeLeftSwipeTemplate();
        InitializeSessionScheduleAsListView();
        InitializeBusyIndicator();
        InitializeAddControlsToContent();
    }

    private void InitializeLeftSwipeTemplate()
    {

        LeftSwipeTemplate = new DataTemplate(() =>
        {
            var grid = new Grid
                       {
                           VerticalOptions = LayoutOptions.CenterAndExpand
                         , ColumnDefinitions =
                           {
                               new ColumnDefinition { Width = GridLength.Star }
                           }
                       };

            var deleteImage = new Image
                              {
                                  Source          = "delete_black_48.png"
                                , BackgroundColor = Color.Transparent
                                , HeightRequest   = 35
                                , WidthRequest    = 35
                              };

            deleteImage.GestureRecognizers
                       .Add(new TapGestureRecognizer
                            {
                                Command = new Command(DeleteImage_OnTapped)
                            });

            grid.Children.Add(deleteImage);

            var swipeContent = new Grid
                               {
                                   BackgroundColor   = Color.Transparent //Color.FromHex("#DC595F")
                                 , HorizontalOptions = LayoutOptions.Center
                                 , VerticalOptions   = LayoutOptions.Fill
                                 , Children          = { grid }
                               };

            return swipeContent;
        });
    }

    private void InitializeSessionsFilter()
    {
        SessionsFilter = new Entry
                         {
                             Placeholder = "Search Sessions (try d=1 or d>5)"
                           , IsVisible   = false
                         };

        SessionsFilter.TextChanged += SessionsFilter_OnTextChanged;
    }

    private void InitializeAddControlsToContent()
    {

        // Set the created controls as the content of the page
        Content = new StackLayout
                  {
                      VerticalOptions = LayoutOptions.StartAndExpand
                    , Children =
                      {
                          SessionsFilter
                        , SessionsScheduleGrid
                        , SessionsListView
                        , BusyIndicator
                      }
                  };
    }

    private void InitializeBusyIndicator()
    {
        BusyIndicator = new SfBusyIndicator
                        {
                            Title           = "Loading..."
                          , AnimationType   = AnimationTypes.Ball
                          , EnableAnimation = false
                          , IsVisible       = false
                          , BackgroundColor = Color.Transparent
                        };
    }

    private void InitializeSessionScheduleAsListView()
    {
        SessionsListView = new SfListView
                           {
                               VerticalOptions = LayoutOptions.EndAndExpand
                             , AutoFitMode     = AutoFitMode.DynamicHeight
                             , SelectionMode   = (XForm.SelectionMode)SelectionMode.Single
                             , AllowSwiping    = true
                             , IsVisible       = false
                           };
        SessionsListView.SwipeEnded       += ListView_SwipeEnded;
        SessionsListView.SelectionChanged += OnSelectionChanged;

        SessionsListView.ItemTemplate = new DataTemplate(() =>
        {
            var curvedCornersLabel = new CurvedCornersLabel
                                     {
                                         HeightRequest      = 70
                                       , CurvedCornerRadius = 15
                                       , VerticalOptions    = LayoutOptions.StartAndExpand
                                       , Margin             = new Thickness(15, 5, 10, 0)
                                     };
            curvedCornersLabel.SetBinding(Label.TextProperty, ".");
            curvedCornersLabel.SetBinding(Label.TextColorProperty, "Client.TextColor");
            curvedCornersLabel.SetBinding(CurvedCornersLabel.CurvedBackgroundColorProperty
                                        , "Client.BackgroundColorHex");

            var grid = new Grid { RowDefinitions = { _rowDefinition } };
            grid.Children.Add(curvedCornersLabel);

            var viewCell = new ViewCell { View = grid };

            return viewCell;
        });
        SessionsListView.LeftSwipeTemplate = LeftSwipeTemplate;
    }

    private void InitializeSessionScheduleAsCalendar()
    {
        SessionsScheduleGrid = new Grid
                               {
                                   ColumnDefinitions = { _columnDefinitionStar }
                                 , RowDefinitions    = { _rowDefinition }
                                 , VerticalOptions   = LayoutOptions.StartAndExpand
                               };

        SessionsSchedule = new SfSchedule
                           {
                               HeightRequest          = 650
                             , FirstDayOfWeek         = 1
                             , ScheduleView           = ScheduleView.MonthView
                             , ShowAppointmentsInline = true
                             , VerticalOptions        = LayoutOptions.StartAndExpand
                             , BindingContext         = SessionList
                             , MonthViewSettings = new MonthViewSettings
                                                   {
                                                       AppointmentDisplayCount   = 7
                                                     , AppointmentDisplayMode    = AppointmentDisplayMode.Appointment
                                                     , AppointmentIndicatorCount = 7
                                                     , AgendaViewHeight          = 400
                                                     , AgendaViewStyle = new AgendaViewStyle
                                                                         {
                                                                             SubjectFontAttributes = FontAttributes.Bold
                                                                           , TimeFontAttributes    = FontAttributes.Italic
                                                                         }

                                                   }
                           };

        // var dataMapping = new ScheduleAppointmentMapping();
        // dataMapping.SubjectMapping   = nameof(SessionAppointment.SessionSubject);
        // dataMapping.StartTimeMapping = nameof(SessionAppointment.SessionStartTime);
        // dataMapping.EndTimeMapping   = nameof(SessionAppointment.SessionEndTime);
        // dataMapping.ColorMapping     = nameof(SessionAppointment.SessionBackgroundColor);
        // dataMapping.TextColorMapping = nameof(SessionAppointment.SessionTextColor);

        //SessionsSchedule.AppointmentMapping = dataMapping;

        SessionsSchedule.CellTapped                   += SessionsSchedule_OnCellTapped;
        SessionsSchedule.MonthInlineAppointmentTapped += SessionsScheduleOnMonthInlineAppointmentTapped;

        SessionsScheduleGrid.Children.Add(SessionsSchedule);
//move to ,cs
        // SessionsSchedule.DataSource = ViewModel.Appointments;

        /*
         * ScheduleAppointmentMapping dataMapping = new ScheduleAppointmentMapping();
           dataMapping.SubjectMapping = "EventName";
           dataMapping.StartTimeMapping = "From";
           dataMapping.EndTimeMapping = "To";
           dataMapping.ColorMapping = "Color";
           dataMapping.TextColorMapping = "TextColor";
           schedule.AppointmentMapping = dataMapping;
         */
    }

    private void InitializeToolbarItems()
    {
        // Create the toolbar items
        ShowAllToolbarItem         =  new ToolbarItem { Text = "" };
        ShowAllToolbarItem.Clicked += ShowAllToolbarItem_OnClicked;

        TestToolbarItem         =  new ToolbarItem { Text = "Test" };
        TestToolbarItem.Clicked += TestToolbarItem_OnClicked;

        AddToolbarItem = new ToolbarItem
                         {
                             Text            = "Add"
                           , IconImageSource = "baseline_add_circle_outline_black_48.png"
                         };
        AddToolbarItem.Clicked += AddToolbarItem_OnClicked;

        SearchToolbarItem = new ToolbarItem
                            {
                                Text            = "Search"
                              , IconImageSource = "baseline_search_black_48.png"
                              , IsEnabled       = false
                            };
        SearchToolbarItem.Clicked += SearchToolbarItem_OnClicked;

        ToggleListViewToolbarItem = new ToolbarItem
                                    {
                                        Text            = "Toggle List View"
                                      , IconImageSource = "segment_black_48.png"
                                    };
        ToggleListViewToolbarItem.Clicked += ToggleListViewToolbarItem_OnClicked;

        //ToolbarItems.Add(TestToolbarItem);
        ToolbarItems.Add(ShowAllToolbarItem);
        ToolbarItems.Add(AddToolbarItem);
        ToolbarItems.Add(SearchToolbarItem);
        ToolbarItems.Add(ToggleListViewToolbarItem);
    }


}
