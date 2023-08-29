using System;
using Avails.Xamarin.Utilities;
using Java.Lang;
using Xamarin.Forms;
using Syncfusion.XForms.Accordion;
using PersonalTrainerWorkouts.Views.Behaviors;
using Button = Xamarin.Forms.Button;
using DatePicker = Xamarin.Forms.DatePicker;
using ScrollView = Xamarin.Forms.ScrollView;
using Switch = Xamarin.Forms.Switch; // Make sure to provide the correct namespace for your behaviors

namespace PersonalTrainerWorkouts.Views.Tab_SettingAndTools
{
    public partial class ConfigurationPage : ContentPage
    {
        private Style _editorStyle;
        private Style _stackLayoutStyle;

        private Label _basicFunctionsHeaderLabel;
        private Button _viewLogButton;

        private Label      _dbFunctionsHeaderLabel;
        private Image      _warningImage;
        private Button     _dropTablesButton;
        private Button     _createTablesButton;
        private Button     _createContactTablesButton;
        private Button     _dropContactTablesButton;
        private Button     _backupDatabaseButton;
        private Button     _restoreDatabaseButton;
        private Editor     _tokenEditor;
        private Button     _pasteButton;
        private DatePicker _expirationDatePicker;
        private Button     _applyButton;
        private Label      _applyNewTokenLabel;
        private Switch     _toggleRefreshTokenControlsSwitch;
        private Label      _toggleRefreshTokenControls;
        private Label      _daysToWarnLabel;
        private Entry      _daysToWarnEntry;

        public void InitializeComponent()
        {
            InitializeStyles();
            InitializeControls();

            Title = "Setting & Tools";

            var accordion = new SfAccordion();
            var accordionBehavior = new AccordionBehavior();
            accordion.Behaviors.Add(accordionBehavior);

            var basicFunctionsAccordionItem = CreateBasicFunctionsAccordionItem();
            var dbFunctionsAccordionItem = CreateDbFunctionsAccordionItem();

            accordion.Items.Add(basicFunctionsAccordionItem);
            accordion.Items.Add(dbFunctionsAccordionItem);

            var scrollView = new ScrollView
            {
                Content = accordion
            };

            Content = new StackLayout
            {
                Children = { scrollView }
            };
        }

        private void InitializeStyles()
        {
            _editorStyle = new Style(typeof(Editor))
                           {
                               Setters =
                               {
                                   new Setter
                                   {
                                       Property = BackgroundColorProperty
                                     , Value    = Application.Current
                                                             .Resources["AppBackgroundColor"]
                                   }
                               }
                           };

            _stackLayoutStyle = new Style(typeof(StackLayout))
                                {
                                    Setters =
                                    {
                                        new Setter
                                        {
                                            Property = VisualStateManager.VisualStateGroupsProperty
                                          , Value = new VisualStateGroupList
                                                    {
                                                        new()
                                                        {
                                                            Name = "CommonStates"
                                                          , States =
                                                            {
                                                                new VisualState { Name = "Normal" }
                                                              , new VisualState
                                                                {
                                                                    Name = "Selected"
                                                                  , Setters =
                                                                    {
                                                                        new Setter
                                                                        {
                                                                            Property = BackgroundColorProperty
                                                                          , Value = Application.Current
                                                                                               .Resources["AppPrimaryColor"]
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                        }
                                    }
                                };
        }

        private void InitializeControls()
        {
            _basicFunctionsHeaderLabel = new Label
                                         {
                                             TextColor               = (Color)Application.Current
                                                                                         .Resources["PrimaryBackColor"]
                                           , FontAttributes          = FontAttributes.Bold
                                           , HeightRequest           = 25
                                           , VerticalTextAlignment   = TextAlignment.Center
                                           , HorizontalTextAlignment = TextAlignment.Center
                                         };

            _viewLogButton         =  new Button { Text = "View Log" };
            _viewLogButton.Clicked += ViewLogButtonClicked;

            _toggleRefreshTokenControls = new Label { Text = "Show Refresh Token Settings" };

            _toggleRefreshTokenControlsSwitch         =  new Switch();
            _toggleRefreshTokenControlsSwitch.Toggled += ToggleRefreshTokenControlsSwitchOnToggled;

            _applyNewTokenLabel = new Label
                                  {
                                      Text = GetRefreshTokenText()
                                  };


            _tokenEditor = new Editor
                           {
                               Placeholder   = "paste in token here"
                             , AutoSize      = EditorAutoSizeOption.Disabled
                             , HeightRequest = 25
                           };

            _pasteButton = new Button
                           {
                               Text          = "Paste/Validate"
                             , HeightRequest = 25
                           };

            _pasteButton.Clicked += PasteButtonOnClicked;

            _expirationDatePicker = new DatePicker { TextColor = Color.Black };
            _applyButton = new Button
                           {
                               Text          = "Apply"
                             , HeightRequest = 25
                           };
            _applyButton.Clicked += ApplyButtonOnClicked;

            _daysToWarnLabel = new Label
                               {
                                   Text                  = "Days to warn before expiration date:"
                                 , VerticalTextAlignment = TextAlignment.Center
                               };

            _daysToWarnEntry = new Entry
                               {
                                   Keyboard                = Keyboard.Numeric
                                 , VerticalTextAlignment   = TextAlignment.Center
                                 , HorizontalTextAlignment = TextAlignment.Center
                               };
            _daysToWarnEntry.Unfocused += DaysToWarnEntryOnUnfocused;

            _dbFunctionsHeaderLabel = new Label
                                      {
                                          TextColor = (Color)Application.Current
                                                                        .Resources["PrimaryBackColor"]
                                        , FontAttributes          = FontAttributes.Bold
                                        , HeightRequest           = 25
                                        , VerticalTextAlignment   = TextAlignment.Center
                                        , HorizontalTextAlignment = TextAlignment.Center
                                      };

            _warningImage = new Image
                            {
                                Source = "outline_warning_amber_black_24.png", HorizontalOptions = LayoutOptions.Start
                            };

            _dropTablesButton         =  new Button { Text = "Drop Tables" };
            _dropTablesButton.Clicked += DropTablesButtonClicked;

            _createTablesButton         =  new Button { Text = "Create Tables" };
            _createTablesButton.Clicked += CreateTablesButtonClicked;

            _createContactTablesButton         =  new Button { Text = "Create Contact Tables" };
            _createContactTablesButton.Clicked += CreateContactTablesButton_OnClicked;

            _dropContactTablesButton         =  new Button { Text = "Drop Contact Tables" };
            _dropContactTablesButton.Clicked += DropContactTablesButton_OnClicked;

            _backupDatabaseButton         =  new Button { Text = "Backup Database" };
            _backupDatabaseButton.Clicked += BackupDatabaseButton_OnClicked;

            _restoreDatabaseButton         =  new Button { Text = "Restore Database" };
            _restoreDatabaseButton.Clicked += RestoreDatabaseButton_OnClicked;
        }

        private static string GetRefreshTokenText()
        {
            var text = new StringBuilder();
            text.Append("When the token to get info from the GitHub API has expired");
            text.Append(Environment.NewLine);
            text.Append("  1) copy the new token");
            text.Append(Environment.NewLine);
            text.Append("  2) tap on the PASTE/VALIDATE button");
            text.Append(Environment.NewLine);
            text.Append("  3) set the date of the new expiration date");
            text.Append(Environment.NewLine);
            text.Append("  4) tap on the APPLY button");

            return text.ToString();
        }

        private AccordionItem CreateBasicFunctionsAccordionItem()
        {
            _basicFunctionsHeaderLabel.Text = "Basic Functions";

            var accordionItem = new AccordionItem
                                {
                                    ClassId = "BasicFunctions"
                                  , Header  = new Grid { Children = { _basicFunctionsHeaderLabel } }
                                  , Content = new StackLayout
                                              {
                                                  Children =
                                                  {
                                                      _viewLogButton
                                                    , new Grid
                                                      {
                                                          ColumnDefinitions = new ColumnDefinitionCollection
                                                                              {
                                                                                  UiUtilities.NewColumnDefinition(85
                                                                                                                , GridUnitType.Star)
                                                                                , UiUtilities.NewColumnDefinition(15
                                                                                                                , GridUnitType.Star)
                                                                              }
                                                        , Children =
                                                          {
                                                              { _toggleRefreshTokenControls, 0, 0 }
                                                            , { _toggleRefreshTokenControlsSwitch, 1, 0 }
                                                          }
                                                      }
                                                    , _applyNewTokenLabel
                                                    , new Grid
                                                      {
                                                          ColumnDefinitions = new ColumnDefinitionCollection
                                                                              {
                                                                                  //   new() { Width = GridLength.Star }
                                                                                  // , new() { Width = GridLength.Auto }
                                                                                  UiUtilities.NewColumnDefinition(75, GridUnitType.Star)
                                                                                , UiUtilities.NewColumnDefinition(25, GridUnitType.Star)
                                                                              }
                                                        , Children =
                                                          {
                                                              { _tokenEditor, 0, 0 }, { _pasteButton, 1, 0 }
                                                            , { _expirationDatePicker, 0, 1 }, { _applyButton, 1, 1 }
                                                            , { _daysToWarnLabel, 0, 2 }, { _daysToWarnEntry, 1, 2 }
                                                          }
                                                      }
                                                    , new Label() //Hack to keep the grid above from filling the remainder of the space.
                                                  }
                                              }
                                };

            return accordionItem;
        }

        // private AccordionItem CreateBasicFunctionsAccordionItem()
        // {
        //     _basicFunctionsHeaderLabel.Text = "Basic Functions";
        //
        //     var accordionItem = new AccordionItem
        //                         {
        //                             ClassId = "BasicFunctions"
        //                           , Header  = new Grid { Children = { _basicFunctionsHeaderLabel } }
        //                           , Content = new Grid
        //                                       {
        //                                           RowDefinitions = new RowDefinitionCollection
        //                                                            {
        //                                                                new() { Height = GridLength.Auto }
        //                                                              , new() { Height = GridLength.Auto }
        //                                                            }
        //                                         , ColumnDefinitions = new ColumnDefinitionCollection
        //                                                               {
        //                                                                   new() { Width = GridLength.Star }
        //                                                                 , new() { Width = GridLength.Auto }
        //                                                               }
        //                                         , Children =
        //                                           {
        //                                               { _viewLogButton,        0, 0 }
        //                                             , { _tokenEditor,          0, 1 }, { _pasteButton, 1, 1 }
        //                                             , { _expirationDatePicker, 0, 2 }, { _applyButton, 1, 2 }
        //                                           }
        //                                       }
        //                         };
        //
        //     return accordionItem;
        // }

        private AccordionItem CreateDbFunctionsAccordionItem()
        {
            _dbFunctionsHeaderLabel.Text = "Database Functions";

            var accordionItem = new AccordionItem
                                {
                                    ClassId = "DbFunctions"
                                  , Header = new Grid
                                             {
                                                 ColumnDefinitions = new ColumnDefinitionCollection
                                                                     {
                                                                         new() { Width = GridLength.Star }
                                                                     }
                                               , Children = { _dbFunctionsHeaderLabel, _warningImage }
                                             }
                                  , Content = new Grid
                                              {
                                                  RowDefinitions = new RowDefinitionCollection
                                                                   {
                                                                       new() { Height = GridLength.Auto }
                                                                     , new() { Height = GridLength.Auto }
                                                                     , new() { Height = GridLength.Auto }
                                                                     , new() { Height = GridLength.Auto }
                                                                     , new() { Height = GridLength.Auto }
                                                                     , new() { Height = GridLength.Auto }
                                                                     , new() { Height = GridLength.Auto }
                                                                   }
                                                , Children =
                                                  {
                                                     { _dropTablesButton,           0, 0 }
                                                    , { _createTablesButton,        0, 1 }
                                                    , { _createContactTablesButton, 0, 2 }
                                                    , { _dropContactTablesButton,   0, 3 }
                                                    , { _backupDatabaseButton,      0, 4 }
                                                    , { _restoreDatabaseButton,     0, 5 }
                                                  }
                                              }
                                };

            return accordionItem;
        }
    }
}
