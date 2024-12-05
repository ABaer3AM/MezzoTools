using System.Diagnostics;
using System.Threading;

namespace MauiApp1.Pages;

public partial class TileTester : ContentPage
{
    private CancellationTokenSource _cancellationTokenSource;
    public static string[] StringArray = new string[] { "testServiceStatusView"};
    private static readonly HttpClient httpClient = new HttpClient();

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
        // Update visual to show fetch is running
        Debug.WriteLine($"Started Task running at {DateTime.Now}");
        await MainThread.InvokeOnMainThreadAsync(() => { serviceRunningStatusView.UpdateStatus(1); });


        // Work...
        Debug.WriteLine("Checklist Fetch Success:/t" + (await FetchChecklistStatus() == 1));
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

    //- Get methods for tile statuses --------------------------------------------------
    private async Task<int> FetchChecklistStatus()
    {
        string apiUrl = "https://witeboard.com/";
        int status = 0;
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"API is reachable. Status Code: {response.StatusCode}");
                checklistStatusView.UpdateStatus(1);
            }
            else
            {
                Debug.WriteLine($"API is not reachable. Status Code: {response.StatusCode}");
                checklistStatusView.UpdateStatus(0);
            }
            return 1;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking API connectivity: {ex.Message}");
            checklistStatusView.UpdateStatus(0);
            checklistStatusView.UpdateFeedback(ex.Message);
            return 0;
        }

    }
    //----------------------------------------------------------------------------------
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