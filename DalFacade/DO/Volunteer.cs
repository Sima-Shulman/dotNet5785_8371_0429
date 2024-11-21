using System.Text;

namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="fullName"></param>
/// <param name="cellphoneNumber"></param>
/// <param name="email"></param>
/// <param name="fullAddress"></param>
/// <param name="latitude"></param>
/// <param name="longitude"></param>
/// <param name="role"></param>
/// <param name="isActive"></param>
/// <param name="DistanceTypes"></param>
/// <param name="maxDistance"></param>
/// <param name="password"></param>
public record Volunteer
(
    int Id,
    string FullName,
    string CellphoneNumber,
    string Email,
    string? FullAddress,
    double? Latitude,
    double? Longitude,
    Role Role,
    bool IsActive,
    DistanceTypes DistanceTypes,
    double? MaxDistance,
    string? Password
)
{
    public Volunteer() : this(0, "", "", "", null, null, null, Role.Volunteer, false, DistanceTypes.aerial_distance, null, GenerateRandomPassword(10)) { }
    private static string GenerateRandomPassword(int length)//generate a random password including letters and numbers.
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random random = new Random();
        StringBuilder password = new StringBuilder();


        for (int i = 0; i < length; i++)
        {
            int index = random.Next(chars.Length);
            password.Append(chars[index]);
        }

        return password.ToString();
    }
}

