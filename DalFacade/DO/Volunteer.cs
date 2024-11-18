using System.Text;

namespace DO;
/// <summary>
/// /// Volunteer Entity represents a Volunteer with all its props
/// </summary>
public record Volunteer
{
    private static DistanceTypes aerial_distance;

    public int Id { get; set; }
    public string fullName { get; set; }
    public string cellphoneNumber { get; set; }
    public string email { get; set; }
    public string? password { get; set; } = GenerateRandomPassword(10);//initialization the password with a random string.
    public string? fullAdress { get; set; }
    public double? latitude { get; set; }
    public double? longitude { get; set; }
    public Role role { get; set; }
    public bool isActive { get; set; }
    public double? maxDistance { get; set; }
    public DistanceTypes DistanceTypes { get; set; } = DistanceTypes.aerial_distance;

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

