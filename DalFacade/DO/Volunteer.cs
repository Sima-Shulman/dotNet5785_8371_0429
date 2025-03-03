using System.Text;
using System.Security.Cryptography;


namespace DO;
/// <summary>
/// Volunteer Entity represents a volunteer with all its props
/// </summary>
/// <param name="Id"> Personal unique ID of the student (as in national id card)</param>
/// <param name="fullName">First and last Name of the volunteer</param>
/// <param name="cellphoneNumber">The voluteer's cellphone number</param>
/// <param name="email">The voluteer's email</param>
/// <param name="fullAddress">The voluteer's address</param>
/// <param name="latitude"></param>
/// <param name="longitude"></param>
/// <param name="role">The voluteer's role: voluteer or manager</param>
/// <param name="isActive"> Whether the voluteer is active in the organization or whether he left</param>
/// <param name="DistanceTypes">The type of the maximum distance: Aerial distance, walking distance, driving distance</param>
/// <param name="DistanceTypes">The type of the maximum distance: Aerial distance, walking distance, driving distance</param>
/// <param name="maxDistance">Maximum distance for receiving a call. The default is air distance</param>
/// <param name="password">The volunteer's password foe entering the organization system, default a random string of 10 characters </param>
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
    //generate a random password including letters and numbers.
    private static string GenerateRandomPassword(int length)
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

    internal static string EncryptPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes!);
    }
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Volunteer() : this(0, "", "", "", null, null, null, Role.volunteer, false, DistanceTypes.aerial_distance, null, EncryptPassword(GenerateRandomPassword(10))) { }
}

