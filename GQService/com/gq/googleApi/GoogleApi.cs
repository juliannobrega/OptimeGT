using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GQService.com.gq.googleApi
{
    public static class GoogleApi
    {
        public static string KeyAccess = "AIzaSyDk7RqOuOMnk-t5CO84dEU025-cLe0kpak";
        private static string URiApi = "https://maps.googleapis.com/";
        private static string UriMap = URiApi + "maps/api/";
        private static string UriTimeZone = UriMap + "timezone/";

        private static string GetPartkey { get { return string.IsNullOrWhiteSpace(KeyAccess) ? "" : "&key=" + KeyAccess; } }

        private static Int32 GetTimestamp { get { return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; } }

        public async static Task<dynamic> TimeZone(double lat, double lon)
        {
            var uri = new Uri(UriTimeZone + "json?location=" + lat.ToString().Replace(',', '.') + "," + lon.ToString().Replace(',', '.') + "&timestamp=" + GetTimestamp + GetPartkey);
            var data = await Get(uri);
            return JsonConvert.DeserializeObject(data);
        }

        private async static Task<string> Get(Uri uri)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            var read = await response.Content.ReadAsStringAsync();
            return read;
        }

    }
}
