

using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace Helpers;

internal static class Tools
{
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

    //public static double CalculateDistance(double? lat1, double? lon1, double lat2, double lon2)
    //{
    //    var lat1Value = lat1 ?? 0;
    //    var lon1Value = lon1 ?? 0;

    //    var r = 6371;
    //    var dLat = (lat2 - lat1Value) * Math.PI / 180;
    //    var dLon = (lon2 - lon1Value) * Math.PI / 180;
    //    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
    //            Math.Cos(lat1Value * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
    //            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
    //    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    //    return r * c;
    //}
    private static readonly string apiKey = "pk.f24f6da7cdb2f8ae9a502c6d81376251";
    private static readonly string distanceApiKey = "GfUGNG16Gk0olnqAUkAZRizSgkclwyPt";


    public static double CalculateDistance(double? latitudeV, double? longitudeV, double latitudeC, double longitudeC, DO.DistanceTypes type)
    {
        if (latitudeV == null || longitudeV == null) return 0;
        double latitudeVNotNull = latitudeV.Value;
        double longitudeVNotNull = longitudeV.Value;
        return type switch
        {
            DO.DistanceTypes.AerialDistance => HaversineDistance(latitudeVNotNull, longitudeVNotNull, latitudeC, longitudeC),
            DO.DistanceTypes.WalkingDistance => GetRouteDistance(latitudeVNotNull, longitudeVNotNull, latitudeC, longitudeC, "pedestrian"),
            DO.DistanceTypes.DrivingDistance => GetRouteDistance(latitudeVNotNull, longitudeVNotNull, latitudeC, longitudeC, "car"),
            _ => throw new ArgumentException("Invalid distance type", nameof(type))
        };
    }

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

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;


    //public static double CalculateDistance((double? lat, double? lon) coords1, (double, double) coords2, DO.DistanceTypes distanceType)
    //{
    //    if (coords1.lat == null || coords1.lon == null)
    //        return 0;
    //    (double,double) coords1NotNull = (coords1.lat.Value, coords1.lon.Value);
    //    return distanceType switch
    //    {
    //        DO.DistanceTypes.AerialDistance => CalculateHaversineDistance(coords1NotNull, coords2),
    //        DO.DistanceTypes.WalkingDistance => GetRouteDistance(coords1NotNull, coords2, "foot-walking"),
    //        DO.DistanceTypes.DrivingDistance => GetRouteDistance(coords1NotNull, coords2, "driving"),
    //        _ => throw new ArgumentException("Invalid distance type")
    //    };
    //}

    //// חישוב מרחק אווירי (Haversine Formula)
    //private static double CalculateHaversineDistance((double lat1, double lon1) coords1, (double lat2, double lon2) coords2)
    //{
    //    const double EarthRadiusKm = 6371; // רדיוס כדור הארץ בק"מ

    //    double dLat = ToRadians(coords2.lat2 - coords1.lat1);
    //    double dLon = ToRadians(coords2.lon2 - coords1.lon1);

    //    double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
    //               Math.Cos(ToRadians(coords1.lat1)) * Math.Cos(ToRadians(coords2.lat2)) *
    //               Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

    //    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    //    return EarthRadiusKm * c;
    //}

    //private static double ToRadians(double degrees) => degrees * (Math.PI / 180);

    //// חישוב מרחק הליכה/נסיעה עם קריאת API סינכרונית
    //private static double GetRouteDistance((double, double) coords1, (double, double) coords2, string mode)
    //{
    //    using var client = new HttpClient();
    //    string url = $"https://us1.locationiq.com/v1/directions/{mode}/{coords1.Item1},{coords1.Item2};{coords2.Item1},{coords2.Item2}?key={apiKey}&format=json";


    //    HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
    //    if (!response.IsSuccessStatusCode)
    //        throw new BO.BlGeneralException("API request failed");

    //    string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    //    using var doc = JsonDocument.Parse(json);

    //    if (!doc.RootElement.TryGetProperty("routes", out var routes) || routes.GetArrayLength() == 0)
    //        throw new BO.BlInvalidFormatException("No route found");

    //    var distanceMeters = routes[0].GetProperty("distance").GetDouble();
    //    return distanceMeters / 1000.0; // המרה לקילומטרים
    //}


    //public static double CalculateDistance(double? latitudeV, double? longitudeV, double latitudeC, double longitudeC, BO.DistanceType mode = BO.DistanceType.Air)
    //{


    //    if (mode != BO.DistanceType.Air)
    //    {
    //        using (HttpClient client = new HttpClient())
    //        {
    //            // הכנת המידע לשאילתה ב-JSON
    //            var requestData = new
    //            {
    //                coordinates = new[]
    //                {
    //                   new double[] { (double)longitudeV!, (double)latitudeV! }, // תחילת מסלול
    //                   new double[] { longitudeC, latitudeC }  // סוף מסלול
    //               }
    //            };

    //            // הגדרת כותרות הבקשה
    //            client.DefaultRequestHeaders.Add("Authorization", apiKey);
    //            string? modedistance = null;
    //            switch (mode)
    //            {
    //                case BO.DistanceType.Drive:
    //                    modedistance = "driving-car";
    //                    break;
    //                case BO.DistanceType.Walk:
    //                    modedistance = "walking";
    //                    break;


    //            }
    //            string url = string.Format(apiUrl, modedistance);


    //            HttpResponseMessage response = client.PostAsync(url,
    //             new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")).GetAwaiter().GetResult();


    //            // קבלת התשובה
    //            string responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    //            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent)!;


    //            // קבלת המרחק במטרים
    //            return jsonResponse.routes[0].segments[0].distance;

    //        }
    //    }
    //    else
    //    {
    //        const double EarthRadiusKm = 6371;

    //        double latVRad = (double)DegreesToRadians(latitudeV);
    //        double lonVRad = (double)DegreesToRadians(longitudeV);
    //        double latCRad = (double)DegreesToRadians(latitudeC);
    //        double lonCRad = (double)DegreesToRadians(longitudeC);

    //        double deltaLat = latCRad - latVRad;
    //        double deltaLon = lonCRad - lonVRad;

    //        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
    //                   Math.Cos(latVRad) * Math.Cos(latCRad) *
    //                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
    //        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

    //        double distance = EarthRadiusKm * c;
    //        return distance;
    //    }
    //}
    //private static readonly string apiUrl = "https://geocode.maps.co/search?q={0}&format=json&api_key={1}";
    /// <summary>
    /// Retrieves coordinates (latitude and longitude) for a given address.
    /// If the address is invalid or the API request fails, an appropriate exception is thrown.
    /// </summary>
    /// <param name="address">The address for which coordinates are requested.</param>
    /// <returns>A tuple containing latitude and longitude of the address.</returns>
    /// <exception cref="InvalidAddressException">Thrown when the address is invalid or cannot be processed.</exception>
    /// <exception cref="ApiRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="GeolocationNotFoundException">Thrown when no geolocation is found for the address.</exception>
    ///  private static string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";


    public static (double, double) GetCoordinatesFromAddress(string address)
    {
        using var client = new HttpClient();
        string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        var response = client.GetAsync(url).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
            throw new Exception("Invalid address or API error.");

        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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
    //public static (double latitude, double longitude) GetCoordinatesFromAddress(string address)
    //{
    //    if (string.IsNullOrEmpty(address))
    //    {
    //        throw new BO.BlInvalidFormatException($"Invalid address: {address}");
    //    }

    //    try
    //    {
    //        // יצירת חיבור ל-API
    //        using (HttpClient client = new HttpClient())
    //        {
    //            // מבצע קריאה סינכרונית (בלי async/await)
    //            string url = string.Format(apiUrl, Uri.EscapeDataString(address), apiKey);
    //            var response = client.GetStringAsync(url).Result;  // קריאה סינכרונית

    //            // ניתוח התשובה ב-JSON
    //            var jsonResponse = JArray.Parse(response);
    //            if (jsonResponse.Count > 0)
    //            {
    //                var firstResult = jsonResponse[0];
    //                double latitude = firstResult.Value<double>("lat");
    //                double longitude = firstResult.Value<double>("lon");

    //                return (latitude, longitude);
    //            }
    //            else
    //            {
    //                throw new BO.BlInvalidFormatException($"Invalid address: {address}");
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception($"שגיאה בקבלת קואורדינטות: {ex.Message}");
    //    }
    //}
}
