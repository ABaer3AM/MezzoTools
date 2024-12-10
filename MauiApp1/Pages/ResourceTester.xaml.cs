using System.Diagnostics;
using MauiApp1.Controls;
using MauiApp1.Constants;
using System.Threading;

namespace MauiApp1.Pages;

public partial class ResourceTester : ContentPage
{
	public ResourceTester()
	{
		InitializeComponent();
	}

    private async void fetchResourceStatus(object sender, EventArgs e)
    {
        CancellationTokenSource resourceCancellationTokenSource;
        resourceCancellationTokenSource = new CancellationTokenSource();

        await MainThread.InvokeOnMainThreadAsync(async () => {
            serviceRunningStatusView.UpdateFull(1, "Running");
            await Task.Run(async () =>
            {
                await ResourceStatusService(resourceCancellationTokenSource.Token);
                StopBackgroundService(resourceCancellationTokenSource);
            });
            serviceRunningStatusView.UpdateFull(0, "Stopped");
        });
    }

    private async Task<int> ResourceStatusService(CancellationToken token)
    {
        Debug.WriteLine($"Started Task running at {DateTime.Now}");

        ResourceChecker resourceCheckerObj = new ResourceChecker();
        // test something
        // Get and display battery percentage
        var battery = await resourceCheckerObj.FetchBattery();
        await MainThread.InvokeOnMainThreadAsync(() => {
            batteryStatusView.UpdateFull(
                battery.Item1,
                battery.Item2);
        });

        // Get and display free disk space
        var diskSpace = await resourceCheckerObj.FetchDiskSpace();
        await MainThread.InvokeOnMainThreadAsync(() => {
            diskSpaceStatusView.UpdateFull(
                diskSpace.Item1,
                diskSpace.Item2);
        });

        // Get and display availible RAM
        var ram = await resourceCheckerObj.FetchRamSpace();
        await MainThread.InvokeOnMainThreadAsync(() => {
            ramStatusView.UpdateFull(
                ram.Item1,
                ram.Item2);
        });

        // Get and display OS
        var os = await resourceCheckerObj.FetchOs();
        await MainThread.InvokeOnMainThreadAsync(() => {
            osStatusView.UpdateFull(
                os.Item1,
                os.Item2);
        });

        // Get and display Internet Speed
        var speed = await resourceCheckerObj.FetchInternetSpeed();
        await MainThread.InvokeOnMainThreadAsync(() => {
            internetStatusView.UpdateFull(
                speed.Item1,
                speed.Item2);
        });

        Debug.WriteLine($"Stopped Task at {DateTime.Now}");
        return 1;
    }
    public void StopBackgroundService(CancellationTokenSource cancelToken)
    {
        cancelToken?.Cancel();
        Debug.WriteLine("Stopped Service");
    }
}