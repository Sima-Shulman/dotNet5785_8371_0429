

using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace Helpers;

internal static class Tools
{
    /// <summary>
    /// Converts an object of type T to a string containing all its property names and values.
    /// Skips the "Password" property and handles enumerable values.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="t">The object instance.</param>
    /// <returns>A string representation of the object properties.</returns>
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return "";

        var result = new StringBuilder();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.Name != "Password")
            {
                var value = property.GetValue(t);
                if (value is IEnumerable enumerable && value is not string)
                {
                    result.Append($"{property.Name}: [");
                    foreach (var item in enumerable)
                    {
                        result.Append($"{item}, ");
                    }
                    result.Append("], ");
                }
                else
                {
                    result.Append($"{property.Name}: {value}, ");
                }
            }
        }
        return result.ToString().TrimEnd(',', ' ');
    }
    private static readonly string apiKey = "pk.f24f6da7cdb2f8ae9a502c6d81376251";
    private static readonly string distanceApiKey = "GfUGNG16Gk0olnqAUkAZRizSgkclwyPt";

    /// <summary>
    /// Calculates the distance between two geographic coordinates using the specified type.
    /// </summary>
    /// <param name="latitudeV">Latitude of the first location (nullable).</param>
    /// <param name="longitudeV">Longitude of the first location (nullable).</param>
    /// <param name="latitudeC">Latitude of the second location.</param>
    /// <param name="longitudeC">Longitude of the second location.</param>
    /// <param name="type">Type of distance to calculate (Aerial, Walking, Driving).</param>
    /// <returns>The distance in kilometers.</returns>
    public static double CalculateDistance(double? latitudeV, double? longitudeV, double? latitudeC, double? longitudeC, DO.DistanceTypes type)
    {
        if (latitudeV == null || longitudeV == null || latitudeC == null || longitudeC == null || latitudeC == 0 || longitudeC == 0) return 0;
        double latitudeVNotNull = latitudeV.Value;
        double longitudeVNotNull = longitudeV.Value;
        double latitudeCNotNull = latitudeC.Value;
        double longitudeCNotNull = longitudeC.Value;
        return type switch
        {
            DO.DistanceTypes.AerialDistance => HaversineDistance(latitudeVNotNull, longitudeVNotNull, latitudeCNotNull, longitudeCNotNull),
            DO.DistanceTypes.WalkingDistance => GetRouteDistance(latitudeVNotNull, longitudeVNotNull, latitudeCNotNull, longitudeCNotNull, "pedestrian"),
            DO.DistanceTypes.DrivingDistance => GetRouteDistance(latitudeVNotNull, longitudeVNotNull, latitudeCNotNull, longitudeCNotNull, "car"),
            _ => throw new ArgumentException("Invalid distance type", nameof(type))
        };
    }
    /// <summary>
    /// Calls TomTom API to calculate route distance using a given travel mode.
    /// </summary>
    /// <param name="latitudeV">Starting latitude.</param>
    /// <param name="longitudeV">Starting longitude.</param>
    /// <param name="latitudeC">Destination latitude.</param>
    /// <param name="longitudeC">Destination longitude.</param>
    /// <param name="travelMode">Mode of travel (e.g., car, pedestrian).</param>
    /// <returns>Distance in kilometers. Returns double.MaxValue on failure.</returns>
    private static double GetRouteDistance(double latitudeV, double longitudeV, double latitudeC, double longitudeC, string travelMode)
    {
        using HttpClient client = new HttpClient();
        string url = $"https://api.tomtom.com/routing/1/calculateRoute/{latitudeV},{longitudeV}:{latitudeC},{longitudeC}/json?key={distanceApiKey}&travelMode={travelMode}";

        try
        {
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
                return double.MaxValue;

            string responseContent = response.Content.ReadAsStringAsync().Result;
            using JsonDocument doc = JsonDocument.Parse(responseContent);

            if (doc.RootElement.TryGetProperty("routes", out var routes) && routes.GetArrayLength() > 0)
            {
                var route = routes[0];
                if (route.TryGetProperty("summary", out var summary) && summary.TryGetProperty("lengthInMeters", out var length))
                    return length.GetDouble() / 1000.0; // המרה לקילומטרים
            }

            return double.MaxValue;
        }
        catch
        {
            return double.MaxValue;
        }
    }
    /// <summary>
    /// Calculates aerial distance (Haversine formula) between two points.
    /// </summary>
    /// <param name="lat1">Latitude of point 1.</param>
    /// <param name="lon1">Longitude of point 1.</param>
    /// <param name="lat2">Latitude of point 2.</param>
    /// <param name="lon2">Longitude of point 2.</param>
    /// <returns>Distance in kilometers.</returns>
    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    /// <summary>
    /// Convert from degree format to Radians.
    /// </summary>
    /// <param name="degrees">the degrees value</param>
    /// <returns></returns>
    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;

    /// <summary>
    /// Retrieves coordinates (latitude and longitude) for a given address using LocationIQ API.
    /// </summary>
    /// <param name="address">The address for which coordinates are requested.</param>
    /// <returns>A tuple containing latitude and longitude of the address.</returns>
    /// <exception cref="Exception">Thrown when the address is invalid, API call fails, or coordinates cannot be parsed.</exception>
    public static async Task<(double, double)> GetCoordinatesFromAddressAsync(string address)
    {
        using var client = new HttpClient();
        string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Invalid address or API error.");

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
            throw new Exception("Address not found.");

        var root = doc.RootElement[0];

        if (!root.TryGetProperty("lat", out var latProperty) ||
            !root.TryGetProperty("lon", out var lonProperty))
        {
            throw new Exception("Missing latitude or longitude in response.");
        }

        if (!double.TryParse(latProperty.GetString(), out double latitude) ||
            !double.TryParse(lonProperty.GetString(), out double longitude))
        {
            throw new Exception("Invalid latitude or longitude format.");
        }

        return (latitude, longitude);
    }

}
