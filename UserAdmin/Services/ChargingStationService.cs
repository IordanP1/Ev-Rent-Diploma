using System.Text.Json;

namespace EvRent.Services
{
    public class ChargingStationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ChargingStationService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetStationsAsync()
        {
            var apiKey = _config["OpenChargeMap:ApiKey"];

            var url =
                $"https://api.openchargemap.io/v3/poi/?output=json&countrycode=BG&maxresults=200&compact=true&verbose=false&key={apiKey}";

            return await _httpClient.GetStringAsync(url);
        }
    }
}