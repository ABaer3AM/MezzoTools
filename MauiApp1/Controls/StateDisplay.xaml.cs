using System.Diagnostics;

namespace MauiApp1.Controls;

public partial class StateDisplay : ContentView
{
	public static readonly BindableProperty TitleProperty = BindableProperty.Create(
		nameof(Title),
		typeof(string),
		typeof(StateDisplay),
		propertyChanged: (bindable, oldValue, newValue) =>
		{
			var control = (StateDisplay)bindable;

			control.titleText.Text = newValue as string;

        });

    public static readonly BindableProperty FeedbackProperty = BindableProperty.Create(
        nameof(Feedback),
        typeof(string),
        typeof(StateDisplay),
        defaultValue: "",
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (StateDisplay)bindable;

            control.UpdateFeedback(newValue as string);

        });


    public static readonly BindableProperty StatusProperty = BindableProperty.Create(
        nameof(Status),
        typeof(int),
        typeof(StateDisplay),
        defaultValue: 0, // Default to "success" state
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (StateDisplay)bindable;
            // Switch images based on status
            control.UpdateStatus((int)newValue);
        });

    public StateDisplay()
	{
		InitializeComponent();
    }

	public string Title
    {
		get => GetValue(TitleProperty) as string;
		set => SetValue(TitleProperty, value);
	}
    public string Feedback
    {
        get => GetValue(FeedbackProperty) as string;
        set => SetValue(FeedbackProperty, value);
    }

    public int Status
    {
        get => (int)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public async void UpdateFull(int status, string feedback)
    {
        UpdateStatus(status);
        UpdateFeedback(feedback);
        await Task.Yield();
    }
    public void UpdateStatus(int status)
    {
        // Update the image source and background color based on the status value
        if (status == 1)
        {
            statusImage.Source = "check.png";  // Example: change image to checkmark
            imageFrame.BackgroundColor = Color.FromHex("#66E44C"); // Green
        }
        else if(status == -1)
        {
            statusImage.Source = "exclamation.png";  // Example: change image to X mark
            imageFrame.BackgroundColor = Color.FromHex("#E9D75F"); // Yellow
        }
        else
        {
            statusImage.Source = "xmark.png";  // Example: change image to X mark
            imageFrame.BackgroundColor = Color.FromHex("#F94620"); // Red
        }
        Status = status;
    }

    public void UpdateFeedback(string feedback)
    {
        feedbackText.Text = ("(" + feedback + ")");
        Feedback = feedback;
    }
}
