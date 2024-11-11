namespace DO;
public record Volunteer
{
    private static DistanceTypes aerial_distance;

    public int Id { get; set; }
    public string fullName { get; set; }
    public string cellphoneNumber { get; set; }
    public string email { get; set; }
    public string? password { get; set; }
    public string? fullAdress { get; set; }
    public double? latitude { get; set; }
    public double? longitude { get; set; }
    public Role role { get; set; }
    public bool isActive { get; set; }
    public double? maxDistance { get; set; }
    public DistanceTypes DistanceTypes { get; set; } = DistanceTypes.aerial_distance;
}

