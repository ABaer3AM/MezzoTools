using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MauiApp1.Controls
{
    internal class StatusChecker
    {

        private static readonly HttpClient httpClient = new HttpClient();

        public StatusChecker() { 
        }

        public async Task<Tuple<int, string>> FetchApiStatus(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"API is reachable. Status Code: {response.StatusCode}");
                    return Tuple.Create(1,"Online");
                }
                else
                {
                    Debug.WriteLine($"API is not reachable. Status Code: {response.StatusCode}");

                    return Tuple.Create(0, "Offline");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking API connectivity: {ex.Message}");
                return Tuple.Create(-1, ex.Message);
            }

        }
    }
}
