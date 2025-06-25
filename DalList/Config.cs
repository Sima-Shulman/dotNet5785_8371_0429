using System.Runtime.CompilerServices;

namespace Dal;
/// <summary>
/// The configuration class. Holds the system's variables.
/// </summary>
public class Config
{
    internal const int startCallId = 1;
    private static int nextCallId = startCallId;
    internal static int NextCallId { [MethodImpl(MethodImplOptions.Synchronized)]  get => nextCallId++; }
    internal const int startAssignmentId = 1;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { [MethodImpl(MethodImplOptions.Synchronized)] get => nextAssignmentId++; }
    internal static DateTime Clock { [MethodImpl(MethodImplOptions.Synchronized)]  get; [MethodImpl(MethodImplOptions.Synchronized)] set; } = DateTime.Now;
    internal static TimeSpan RiskRange { [MethodImpl(MethodImplOptions.Synchronized)]  get; [MethodImpl(MethodImplOptions.Synchronized)]  set; } = TimeSpan.FromHours(1);

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;  
        RiskRange = TimeSpan.FromHours(1);
    }

}
