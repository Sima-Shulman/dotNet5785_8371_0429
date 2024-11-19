
namespace Dal;
internal static class DataSource
{
    internal static List<DO.Call?> Calls { get; } = new();
    internal static List<DO.Iassignment?> Assignments { get; } = new();
    internal static List<DO.Volunteer?> Volunteers { get; } = new();

}
