using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Net;

namespace MauiApp1.Controls
{
    internal class ResourceChecker
    {

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);


        public ResourceChecker()
        {
        }

        public async Task<Tuple<int, string>> FetchBattery()
        {
            double battery = Math.Round(Battery.ChargeLevel * 100, 1);
            await Task.Yield();

            if (battery > 30.0)
            {                        // Good     = 31+  
                return Tuple.Create(1, $"Good: {battery}");
            }
            else if (battery > 15)
            {                    // Warning  = 30-16
                return Tuple.Create(-1, $"Warning: {battery}");
            }
            else
            {                                      // Critical = 15-0
                return Tuple.Create(0, $"Critical: {battery}");
            }
        }

        public async Task<Tuple<int, string>> FetchDiskSpace()
        {
            await Task.Yield();
            try
            {
                // Get the drive information for the specified drive
                DriveInfo drive = new DriveInfo("C:");

                if (drive.IsReady)
                {
                    long availableSpace = drive.AvailableFreeSpace; // In bytes
                    long totalSpace = drive.TotalSize;

                    // Convert to GB for readability
                    double availableSpaceGB = availableSpace / (1024.0 * 1024 * 1024);
                    double totalSpaceGB = totalSpace / (1024.0 * 1024 * 1024);
                    double percentFree = (availableSpaceGB * 100) / totalSpaceGB;

                    int status = 0;

                    if (percentFree > 10)
                    {
                        return Tuple.Create(1, $"{availableSpaceGB:F2} GB  /  {totalSpaceGB:F2} GB  -> {percentFree:F1}%");
                    }
                    else if (percentFree < 5)
                    {
                        return Tuple.Create(-1, $"{availableSpaceGB:F2} GB  /  {totalSpaceGB:F2} GB  -> {percentFree:F1}% [>10% recommended]");
                    }
                    else
                    {
                        return Tuple.Create(-1, $"{availableSpaceGB:F2} GB  /  {totalSpaceGB:F2} GB  -> {percentFree:F1}% [>5% required]");
                    }
                }
                else
                {
                    return Tuple.Create(0, "Main is not ready.");
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(0, $"Error fetching disk space: {ex.Message}");
            }
        }

        public async Task<Tuple<int, string>> FetchRamSpace()
        {
            var memoryStatus = new MEMORYSTATUSEX();
            await Task.Yield();

            if (GlobalMemoryStatusEx(memoryStatus))
            {
                double availableRAMGB = memoryStatus.ullAvailPhys / (1024.0 * 1024 * 1024);
                double totalRAMGB = memoryStatus.ullTotalPhys / (1024.0 * 1024 * 1024);

                if (availableRAMGB > 1)
                {
                    return Tuple.Create(1, $"{availableRAMGB:F2} GB  available");
                }
                else if (availableRAMGB > .25)
                {
                    return Tuple.Create(-1, $"only {availableRAMGB:F2} GB  available  [>.7 GB recommended]");
                }
                else
                {
                    return Tuple.Create(0, $"only {availableRAMGB:F2} GB  available  [>.4 GB required]");
                }

            }
            else
            {
                return Tuple.Create(0, $"Error fetching RAM information.");
            }
        }
        public async Task<Tuple<int, string>> FetchOs()
        {
            string key = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            await Task.Yield();
            using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(key))
            {
                if (regKey != null)
                {
                    string productName = regKey.GetValue("ProductName") as string;
                    string releaseId = regKey.GetValue("ReleaseId") as string;
                    int buildNumber = int.Parse(regKey.GetValue("CurrentBuildNumber") as string);

                    //check status of OS
                    if (buildNumber > 19044)
                    {
                        return Tuple.Create(1, $"{productName} ({releaseId}, {buildNumber})");
                    }
                    else if (buildNumber > 17763)
                    {
                        return Tuple.Create(-1, $"{productName} ({releaseId}, {buildNumber}) [at least Windows 10 21H2 recommended]");
                    }
                    else
                    {
                        return Tuple.Create(0, $"{productName} ({releaseId}, {buildNumber}) [at least Windows 10 1809 required]");
                    }
                }
            }
            return Tuple.Create(0, "Unknown OS");
        }
        public async Task<Tuple<int, string>> FetchInternetSpeed()
        {
            var watch = new Stopwatch();

            // Test Download
            byte[] data;
            double upSpeed;
            using (var client = new System.Net.WebClient())
            {
                watch.Start();
                data = client.DownloadData("https://testfiles.ah-apps.de/100MB.bin");
                watch.Stop();
            }
            upSpeed = ((data.LongLength / watch.Elapsed.TotalSeconds) / (1024.0 * 1024))*8; // instead of [Seconds] property

            // Test Upload
            double downSpeed;
            long fileSize = 10 * 1024 * 1024; // 10 MB file size for testing
            byte[] uploadData = new byte[fileSize];
            new Random().NextBytes(uploadData); // Fill the array with random data
            using (var client = new System.Net.Http.HttpClient())
            {
                var content = new ByteArrayContent(uploadData);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                try
                {
                    watch.Start();
                    // Replace the URL below with an endpoint on your server to accept uploads
                    var response = await client.PostAsync("https://httpbin.org/post", content);
                    watch.Stop();

                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"Upload failed. Status Code: {response.StatusCode}");
                        return Tuple.Create(0, $"{upSpeed:F2} up  /  -- dn (Upload failed)");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Upload failed. Error: {ex.Message}");
                    return Tuple.Create(0, $"{upSpeed:F2} up  /  -- dn (Upload failed)");
                }
            }
            downSpeed = ((fileSize / watch.Elapsed.TotalSeconds) / (1024.0 * 1024))*8; // Bytes -> Megabytes


            if(upSpeed > 3 && downSpeed > 5)
            {
                return Tuple.Create(1, $"{upSpeed:F2} up  /  {downSpeed:F2} (Mbps)");
            }else if (upSpeed > 2 && downSpeed > 3)
            {
                return Tuple.Create(-1, $"{upSpeed:F2} up  /  {downSpeed:F2} dn (>3/>5 recommended)(Mbps)");
            }
            else
            {
                return Tuple.Create(0, $"{upSpeed:F2} up  /  {downSpeed:F2} dn (>2/>3 required)(Mbps)");
            }
        }

    }
}


