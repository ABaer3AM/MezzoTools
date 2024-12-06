using MauiApp1.Controls;
using System.Diagnostics;
using System.Threading;
using MauiApp1.Constants;

namespace MauiApp1.Pages;

public partial class TileTester : ContentPage
{
    private Tuple<string, StateDisplay>[] tiles;
    private Tuple<string, StateDisplay>[] misc;
    private static readonly Urls urlsObj = new Urls();


    public TileTester()
    {
        InitializeComponent();

        Dictionary<string, string> tileUrls = urlsObj.getTiles();

        tiles = new[]
        {
            Tuple.Create(tileUrls["witeboard"], whiteboardStatusView),
            Tuple.Create(tileUrls["watchDuty"], watchDutyStatusView),
            Tuple.Create(tileUrls["ezriSatellite"], ezriSatStatusView),
            Tuple.Create(tileUrls["googleSatellite"], googleSatStatusView),
            Tuple.Create(tileUrls["google"], googleStatusView),
            Tuple.Create(tileUrls["windy"], windyStatusView),
            Tuple.Create(tileUrls["macroWeather"], macroWeatherStatusView),
        };

        Dictionary<string, string> miscUrls = urlsObj.getMisc();
        misc = new[]
        {
            Tuple.Create(miscUrls["florianWebPhila"], florianWebPhilaStatusView),
            Tuple.Create(miscUrls["florianWebQa"], florianWebQaStatusView),
            Tuple.Create(miscUrls["florianWebDemo"], florianWebDemoStatusView),
            Tuple.Create(miscUrls["florianWebProd"], florianWebProdStatusView),
            Tuple.Create(miscUrls["bingMaps"], bingMapsStatusView),
            Tuple.Create(miscUrls["googlePlay"], googlePlayStatusView),
            Tuple.Create(miscUrls["nextNav"], nextNavStatusView),
        };

        // display navbar
        NavigationPage.SetHasNavigationBar(this, true);
    }

    private async void fetchUrlsStatus(object sender, EventArgs e)
    {
        CancellationTokenSource tileCancellationTokenSource, miscCancellationTokenSource;
        tileCancellationTokenSource = new CancellationTokenSource();
        miscCancellationTokenSource = new CancellationTokenSource();

        Debug.WriteLine("Fetching Tile Statuses...");
        await MainThread.InvokeOnMainThreadAsync(() => { serviceRunningStatusView.UpdateStatus(1); });


        await urlStatusService(tileCancellationTokenSource.Token, tiles);
        StopBackgroundService(tileCancellationTokenSource);

        await urlStatusService(miscCancellationTokenSource.Token, misc);
        StopBackgroundService(miscCancellationTokenSource);


        await MainThread.InvokeOnMainThreadAsync(() => { serviceRunningStatusView.UpdateStatus(0); });
    }


    private async Task<int> urlStatusService(CancellationToken token, Tuple<string, StateDisplay>[] statusList)
    {
        StatusChecker statusCheckerObj = new StatusChecker();

        // Update visual to show fetch is running
        Debug.WriteLine($"Started Task running at {DateTime.Now}");


        // Through all 
        foreach (var status in statusList)
        {
            string apiUrl = status.Item1;
            StateDisplay stateDisplay = status.Item2;

            Tuple<int, string> response = await statusCheckerObj.FetchApiStatus(apiUrl);
            await MainThread.InvokeOnMainThreadAsync(() => { stateDisplay.UpdateFull(response.Item1, response.Item2); });
        }

        //proper exit (1)
        return 1;
    }

    public void StopBackgroundService(CancellationTokenSource cancelToken)
    {
        cancelToken?.Cancel();
        Debug.WriteLine($"Stopped Task at {DateTime.Now}");
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