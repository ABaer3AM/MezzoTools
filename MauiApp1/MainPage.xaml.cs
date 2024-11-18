using MauiApp1.Pages;  // Ensure this namespace is included to access TileTester

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void redirectToTileTester(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TileTester());
        }
    }

}
