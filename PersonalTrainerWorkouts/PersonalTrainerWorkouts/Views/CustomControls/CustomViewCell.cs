using System;
using Avails.Xamarin.Utilities;
using Xamarin.Forms;

namespace PersonalTrainerWorkouts.Views.CustomControls;

public class CustomViewCell: ViewCell
{
    public event EventHandler ImageButtonClick;

    public CustomViewCell()
    {
        var label = new Label
                    {
                        VerticalOptions = LayoutOptions.Center,
                        FontSize        = UiUtilities.FontSizes<Label>.Medium // Use the predefined font size
                    };

        var imageButton = new ImageButton
                          {
                              HorizontalOptions = LayoutOptions.End,
                              BackgroundColor   = Color.Transparent
                              // Set other properties of the ImageButton as needed
                          };

        // Bind the label's text to a property in your data model
        label.SetBinding(Label.TextProperty, "LabelText");

        // Add the label and image button to a layout (e.g., StackLayout)
        var layout = new StackLayout
                     {
                         Orientation = StackOrientation.Horizontal,
                         Children    = { label, imageButton }
                     };

        // Set the View property of the ViewCell
        View = layout;

        // Handle the ImageButton's clicked event
        imageButton.Clicked += OnImageButtonClick;
    }

    private void OnImageButtonClick(object sender, EventArgs e)
    {
        // Raise the custom event when the ImageButton is clicked
        ImageButtonClick?.Invoke(this, EventArgs.Empty);
    }
}
