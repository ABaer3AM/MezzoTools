namespace MauiApp1
{
    public partial class App : Application
    {
        private CancellationTokenSource _cancellationTokenSource;

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }
    }
}
