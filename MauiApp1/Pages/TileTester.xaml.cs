using MauiApp1.Controls;
using System.Diagnostics;
using System.Threading;

namespace MauiApp1.Pages;

public partial class TileTester : ContentPage
{
    private CancellationTokenSource _cancellationTokenSource;
    public static Tuple<string, StateDisplay>[] tiles;


    public TileTester()
    {
        InitializeComponent();

        tiles = new[]
        {
            Tuple.Create("https://witeboard.com/", whiteboardStatusView),
            Tuple.Create("https://app.watchduty.org/", watchDutyStatusView),
            Tuple.Create("https://www.arcgis.com/apps/mapviewer/index.html?layers=10df2279f9684e4a9f6a7f08febac2a9", ezriSatStatusView),
            Tuple.Create("https://www.google.com/maps/", googleSatStatusView),
            Tuple.Create("https://www.google.com/", googleStatusView),
            Tuple.Create("https://www.windy.com/-Waves-waves", windyStatusView),
            Tuple.Create("https://earth.nullschool.net/", macroWeatherStatusView),
        };

        // display navbar
        NavigationPage.SetHasNavigationBar(this, true);
    }

    private async void fetchTileStatus(object sender, EventArgs e)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        Debug.WriteLine("Fetching Tile Statuses...");
        await StartBackgroundService(_cancellationTokenSource.Token);
        StopBackgroundService();
    }



    private async Task<int> StartBackgroundService(CancellationToken token)
    {
        StatusChecker statusCheckerObj = new StatusChecker();

        // Update visual to show fetch is running
        Debug.WriteLine($"Started Task running at {DateTime.Now}");
        await MainThread.InvokeOnMainThreadAsync(() => { serviceRunningStatusView.UpdateStatus(1); });


        // Through all 
        foreach (var tile in tiles)
        {
            string apiUrl = tile.Item1;
            StateDisplay stateDisplay = tile.Item2;

            Tuple<int, string> response = await statusCheckerObj.FetchApiStatus(apiUrl);
            await MainThread.InvokeOnMainThreadAsync(() => { stateDisplay.UpdateFull(response.Item1, response.Item2); });
        }


        // Show visual feedback that the task is stopped
        await MainThread.InvokeOnMainThreadAsync(() => { serviceRunningStatusView.UpdateStatus(0); });
        //proper exit (1)
        return 1;
    }

    public void StopBackgroundService()
    {
        _cancellationTokenSource?.Cancel();
        Debug.WriteLine("Stopped Service at {DateTime.Now}");
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