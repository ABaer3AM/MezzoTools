using MauiApp1.Controls;
using System.Diagnostics;
using System.Threading;

namespace MauiApp1.Pages;

public partial class TileTester : ContentPage
{
    private CancellationTokenSource _cancellationTokenSource;
    public static string[] StringArray = new string[] { "testServiceStatusView"};


    public TileTester()
    {
        InitializeComponent();

        // display navbar
        NavigationPage.SetHasNavigationBar(this, true);
    }

    private async void fetchTileStatus(object sender, EventArgs e)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        Debug.WriteLine("Fetching Tile Statuses...");
        await StartBackgroundService(_cancellationTokenSource.Token);

    }



    private async Task<int> StartBackgroundService(CancellationToken token)
    {
        StatusChecker statusCheckerObj = new StatusChecker();

        // Update visual to show fetch is running
        Debug.WriteLine($"Started Task running at {DateTime.Now}");
        await MainThread.InvokeOnMainThreadAsync(() => { serviceRunningStatusView.UpdateStatus(1); });


        // Work...
        Tuple<int, string> checklistStatus = await statusCheckerObj.FetchApiStatus("https://witeboard.com/");
        await MainThread.InvokeOnMainThreadAsync(() => { checklistStatusView.UpdateFull(checklistStatus.Item1, checklistStatus.Item2); });
        


        await Task.Delay(5000);


        // Show visual feedback that the task is stopped
        Debug.WriteLine("Stopped Task at {DateTime.Now}");
        await MainThread.InvokeOnMainThreadAsync(() => { serviceRunningStatusView.UpdateStatus(0); });
        //proper exit (1)
        return 1;
    }

    public void StopBackgroundService()
    {
        _cancellationTokenSource?.Cancel();
        Debug.WriteLine("Stopped Service");
    }

}

/*
// To run a task infinatley until canceld

Task.Run(async () =>
    {
        // Run ... until service is explicatley canceld
        while (!token.IsCancellationRequested)
        {
            // Perform background work
            await Task.Delay(5000);
            Debug.WriteLine($"Background task running at {DateTime.Now}");
        }
    }, token);
*/