namespace MauiApp1.Controls;

public partial class StateDisplay : ContentView
{
	public static readonly BindableProperty TextProperty = BindableProperty.Create(
		nameof(Text),
		typeof(string),
		typeof(StateDisplay),
		propertyChanged: (bindable, oldValue, newValue) =>
		{
			var control = (StateDisplay)bindable;

			control.displayedText.Text = newValue as string;

        });

	public StateDisplay()
	{
		InitializeComponent();
	}

	public string Text
	{
		get => GetValue(TextProperty) as string;
		set => SetValue(TextProperty, value);
	}
}