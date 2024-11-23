namespace Dal;
/// <summary>
/// 
/// </summary>
public class Config
{
    internal const int startCallId = 1;
    private static int nextCallId = startCallId;
    public static int NextCallId { get => nextCallId++; }
    internal const int startAssignmentId = 1;
    private static int nextAssignmentId = startAssignmentId;
    public static int NextAssignmentId { get => nextAssignmentId++; }
    public static DateTime Clock { get; set; } = DateTime.Now;
    internal static TimeSpan RiskRange { get; set; }
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;  
    }

}
