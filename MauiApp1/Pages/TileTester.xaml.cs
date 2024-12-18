using MauiApp1.Controls;
using System.Diagnostics;
using System.Threading;
using MauiApp1.Constants;

namespace MauiApp1.Pages;

public partial class TileTester : ContentPage
{
    private Tuple<string, StateDisplay>[] tiles;
    private Tuple<string, StateDisplay>[] coreDependencies;
    private static readonly Urls urlsObj = new Urls();


    public TileTester()
    {
        InitializeComponent();

        Dictionary<string, string> tileUrls = urlsObj.getTiles();

        tiles = new[]
        {
            Tuple.Create(tileUrls["witeboard"], whiteboardStatusView),
            Tuple.Create(tileUrls["watchDuty"], watchDutyStatusView),
            Tuple.Create(tileUrls["esriSatellite"], esriSatStatusView),
            Tuple.Create(tileUrls["googleSatellite"], googleSatStatusView),
            Tuple.Create(tileUrls["google"], googleStatusView),
            Tuple.Create(tileUrls["windy"], windyStatusView),
            Tuple.Create(tileUrls["macroWeather"], macroWeatherStatusView),
        };

        Dictionary<string, string> coreDependencyUrls = urlsObj.getCoreDependency();
        coreDependencies = new[]
        {
            Tuple.Create(coreDependencyUrls["florianWebPhila"], florianWebPhilaStatusView),
            Tuple.Create(coreDependencyUrls["florianWebQa"], florianWebQaStatusView),
            Tuple.Create(coreDependencyUrls["florianWebDemo"], florianWebDemoStatusView),
            Tuple.Create(coreDependencyUrls["florianWebProd"], florianWebProdStatusView),
            Tuple.Create(coreDependencyUrls["bingMaps"], vEarthStatusView),
            Tuple.Create(coreDependencyUrls["googlePlay"], googlePlayStatusView),
            Tuple.Create(coreDependencyUrls["nextNav"], nextNavStatusView),
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
        await MainThread.InvokeOnMainThreadAsync(() => { serviceRunningStatusView.UpdateFull(1, "Running"); });


        await urlStatusService(tileCancellationTokenSource.Token, tiles);
        StopBackgroundService(tileCancellationTokenSource);

        await urlStatusService(miscCancellationTokenSource.Token, coreDependencies);
        StopBackgroundService(miscCancellationTokenSource);


        await MainThread.InvokeOnMainThreadAsync(() => { serviceRunningStatusView.UpdateFull(0, "Stopped"); });
    }


    private async Task<int> urlStatusService(CancellationToken token, Tuple<string, StateDisplay>[] statusList)
    {
        UrlChecker statusCheckerObj = new UrlChecker();

        // Update visual to show fetch is running
        Debug.WriteLine($"Started Task running at {DateTime.Now}");


        // Through all 
        foreach (var status in statusList)
        {
            string apiUrl = status.Item1;
            StateDisplay stateDisplay = status.Item2;

            await MainThread.InvokeOnMainThreadAsync(() => { stateDisplay.UpdateStatus(-2); });                             // Set state as Running
            Tuple<int, string> response = await statusCheckerObj.FetchApiStatus(apiUrl);                                    // Fetch status
            await MainThread.InvokeOnMainThreadAsync(() => { stateDisplay.UpdateFull(response.Item1, response.Item2); });   // Set status to be fetched state
        }
        //proper exit (1)
        return 1;
    }

    public void StopBackgroundService(CancellationTokenSource cancelToken)
    {
        cancelToken?.Cancel();
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