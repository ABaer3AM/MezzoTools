using System.Diagnostics;

namespace MauiApp1.Controls
{
    internal class UrlChecker
    {

        private static readonly HttpClient httpClient = new HttpClient();

        public UrlChecker()
        {
        }

        public async Task<Tuple<int, string>> FetchApiStatus(string apiUrl)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
                watch.Stop();

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"API is reachable. Status Code: {response.StatusCode} [{watch.ElapsedMilliseconds:F0}ms]");
                    return Tuple.Create(1, $"Online [{watch.ElapsedMilliseconds:F0}ms]");
                }
                else
                {
                    Debug.WriteLine($"API is not reachable. Status Code: {response.StatusCode} [{watch.ElapsedMilliseconds:F0}ms]");

                    if (response.ReasonPhrase != null)
                    {
                        if (response.ReasonPhrase.Length < 100){ return Tuple.Create(0, $"Offline, response: {response.ReasonPhrase} [{watch.ElapsedMilliseconds:F0}ms]"); }
                        else{
                            return Tuple.Create(0, $"Offline, response: {response.ReasonPhrase.Substring(0, 100)} [{watch.ElapsedMilliseconds:F0}ms]");
                        }
                    }else{
                        return Tuple.Create(0, $"Offline, response: NULL [{watch.ElapsedMilliseconds:F0}ms]");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking API connectivity: {ex.Message} [{watch.ElapsedMilliseconds:F0}ms]");
                return Tuple.Create(-1, ex.Message);
            }

        }
    }
}
