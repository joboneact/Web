using System.Text.Json;

namespace BlazorAppCSharpLearn.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, (double Lat, double Lon)> _stateCoordinates;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _stateCoordinates = InitializeStateCoordinates();
    }

    public async Task<WeatherData?> GetWeatherForStateAsync(string stateName)
    {
        if (!_stateCoordinates.TryGetValue(stateName, out var coordinates))
        {
            return null;
        }

        try
        {
            // Using OpenWeatherMap API (you can also use other free APIs like weather.gov)
            // For demonstration, we'll use a mock service or weather.gov API
            var url = $"https://api.weather.gov/points/{coordinates.Lat},{coordinates.Lon}";
            
            // For now, we'll return mock data since we don't have API keys
            // In a real application, you'd make the actual API call
            return await GetMockWeatherData(stateName);
        }
        catch
        {
            return await GetMockWeatherData(stateName);
        }
    }

    private async Task<WeatherData> GetMockWeatherData(string stateName)
    {
        // Simulate API delay
        await Task.Delay(500);
        
        var random = new Random();
        var temperatures = new[] { 15, 18, 22, 25, 28, 32, 35, 12, 8, 5 };
        var conditions = new[] { "Sunny", "Partly Cloudy", "Cloudy", "Rainy", "Stormy", "Clear", "Overcast" };
        
        return new WeatherData
        {
            LocationName = stateName,
            Temperature = temperatures[random.Next(temperatures.Length)],
            Condition = conditions[random.Next(conditions.Length)],
            Humidity = random.Next(30, 90),
            WindSpeed = random.Next(5, 25),
            LastUpdated = DateTime.Now
        };
    }

    private Dictionary<string, (double Lat, double Lon)> InitializeStateCoordinates()
    {
        return new Dictionary<string, (double Lat, double Lon)>
        {
            { "Alabama", (32.3617, -86.2792) },
            { "Alaska", (64.0685, -152.2782) },
            { "Arizona", (34.2744, -111.2847) },
            { "Arkansas", (34.8938, -92.4426) },
            { "California", (36.7783, -119.4179) },
            { "Colorado", (39.5501, -105.7821) },
            { "Connecticut", (41.6219, -72.7273) },
            { "Delaware", (38.9108, -75.5277) },
            { "Florida", (27.7663, -82.6404) },
            { "Georgia", (32.1656, -82.9001) },
            { "Hawaii", (19.8968, -155.5828) },
            { "Idaho", (44.0682, -114.7420) },
            { "Illinois", (40.6331, -89.3985) },
            { "Indiana", (40.2732, -86.1349) },
            { "Iowa", (41.8780, -93.0977) },
            { "Kansas", (38.5266, -96.7265) },
            { "Kentucky", (37.8393, -84.2700) },
            { "Louisiana", (30.9843, -91.9623) },
            { "Maine", (45.2538, -69.4455) },
            { "Maryland", (39.0458, -76.6413) },
            { "Massachusetts", (42.2373, -71.5314) },
            { "Michigan", (44.3467, -85.4102) },
            { "Minnesota", (46.7296, -94.6859) },
            { "Mississippi", (32.3544, -89.3985) },
            { "Missouri", (37.9643, -91.8318) },
            { "Montana", (47.0527, -109.6333) },
            { "Nebraska", (41.4925, -99.9018) },
            { "Nevada", (38.8026, -116.4194) },
            { "New Hampshire", (43.1939, -71.5724) },
            { "New Jersey", (40.0583, -74.4057) },
            { "New Mexico", (34.8405, -106.2485) },
            { "New York", (43.2994, -74.2179) },
            { "North Carolina", (35.7596, -79.0193) },
            { "North Dakota", (47.5515, -101.0020) },
            { "Ohio", (40.4173, -82.9071) },
            { "Oklahoma", (35.5889, -97.5348) },
            { "Oregon", (43.8041, -120.5542) },
            { "Pennsylvania", (41.2033, -77.1945) },
            { "Rhode Island", (41.6809, -71.5118) },
            { "South Carolina", (33.8361, -81.1637) },
            { "South Dakota", (43.9695, -99.9018) },
            { "Tennessee", (35.5175, -86.5804) },
            { "Texas", (31.9686, -99.9018) },
            { "Utah", (39.3210, -111.0937) },
            { "Vermont", (44.2601, -72.5806) },
            { "Virginia", (37.4316, -78.6569) },
            { "Washington", (47.7511, -120.7401) },
            { "West Virginia", (38.3498, -80.6547) },
            { "Wisconsin", (43.7844, -88.7879) },
            { "Wyoming", (43.0760, -107.2903) }
        };
    }
}

public class WeatherData
{
    public string LocationName { get; set; } = string.Empty;
    public int Temperature { get; set; }
    public string Condition { get; set; } = string.Empty;
    public int Humidity { get; set; }
    public int WindSpeed { get; set; }
    public DateTime LastUpdated { get; set; }
}
