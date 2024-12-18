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
        await MainThread.InvokeOnMainThreadAsync(() => { batteryStatusView.UpdateStatus(-2); });
        var battery = await resourceCheckerObj.FetchBattery();
        await MainThread.InvokeOnMainThreadAsync(() => {
            batteryStatusView.UpdateFull(
                battery.Item1,
                battery.Item2);
        });

        // Get and display free disk space
        await MainThread.InvokeOnMainThreadAsync(() => { diskSpaceStatusView.UpdateStatus(-2); });
        var diskSpace = await resourceCheckerObj.FetchDiskSpace();
        await MainThread.InvokeOnMainThreadAsync(() => {
            diskSpaceStatusView.UpdateFull(
                diskSpace.Item1,
                diskSpace.Item2);
        });

        // Get and display availible RAM
        await MainThread.InvokeOnMainThreadAsync(() => { ramStatusView.UpdateStatus(-2); });
        var ram = await resourceCheckerObj.FetchRamSpace();
        await MainThread.InvokeOnMainThreadAsync(() => {
            ramStatusView.UpdateFull(
                ram.Item1,
                ram.Item2);
        });

        // Get and display OS
        await MainThread.InvokeOnMainThreadAsync(() => { uploadStatusView.UpdateStatus(-2); });
        var os = await resourceCheckerObj.FetchOs();
        await MainThread.InvokeOnMainThreadAsync(() => {
            osStatusView.UpdateFull(
                os.Item1,
                os.Item2);
        });

        // Get and display Internet Speed
        await MainThread.InvokeOnMainThreadAsync(() => { uploadStatusView.UpdateStatus(-2); });
        var upSpeed = await resourceCheckerObj.FetchUploadSpeed();
        await MainThread.InvokeOnMainThreadAsync(() => {
            uploadStatusView.UpdateFull(
                upSpeed.Item1,
                upSpeed.Item2);
        });
        await MainThread.InvokeOnMainThreadAsync(() => { downloadStatusView.UpdateStatus(-2); });
        var downSpeed = await resourceCheckerObj.FetchDownloadSpeed();
        await MainThread.InvokeOnMainThreadAsync(() => {
            downloadStatusView.UpdateFull(
                downSpeed.Item1,
                downSpeed.Item2);
        });

        // Get and display cpu usage
        await MainThread.InvokeOnMainThreadAsync(() => { cpuStatusView.UpdateStatus(-2); });
        var cpuUsage = await resourceCheckerObj.FetchCpuUsage();
        await MainThread.InvokeOnMainThreadAsync(() => {
            cpuStatusView.UpdateFull(
                cpuUsage.Item1,
                cpuUsage.Item2);
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